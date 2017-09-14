using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.DAL;
using Sampoerna.EMS.MessagingService;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.XMLReader
{
    
    
    public class BrandCk1CheckService
    {
        public ILogger logger;
        public IUnitOfWork uow;

        private ICK1Services _ck1Services;
        private IBrandRegistrationService _brandRegistrationService;
        private MessageService _messageService;
        private IGenericRepository<USER> _repositoryUser;
        private IGenericRepository<POA_EXCISER> _repositoryPOAExciser;
        private IGenericRepository<POA_MAP> _repositoryPoaMapRepository;

        private static int xMonth = 5;
        private static int xCompesationDay = 2;

        public BrandCk1CheckService()
        {
            logger = new NLogLogger();
            uow = new SqlUnitOfWork(logger);

            _ck1Services = new CK1Services(uow, logger);
            _brandRegistrationService = new BrandRegistrationService(uow,logger);
            _messageService = new MessageService(logger);

            _repositoryUser = uow.GetGenericRepository<USER>();
            _repositoryPOAExciser = uow.GetGenericRepository<POA_EXCISER>();
            _repositoryPoaMapRepository = uow.GetGenericRepository<POA_MAP>();
        }

        public void BrandCheckProcessCk1()
        {
            var allActiveBrand = _brandRegistrationService.GetAllActiveBrand("01"); //domestic is "01" , export is "02"
            var allLastCk1ItemXmonths = _ck1Services.GetLastXMonthsCk1(xMonth,xCompesationDay);

            var allCk1ItemCode = allLastCk1ItemXmonths.Select(x => x.FA_CODE + "-" + x.WERKS + "-" + x.MATERIAL_ID).ToList();

            var brandSoonValidTemp =
                allActiveBrand.Where(x => allCk1ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.STICKER_CODE)))
                    .ToList();

            DeleteFlagSentForBrand(brandSoonValidTemp);

            var brandSoonInvalidTemp =
                allActiveBrand.Where(x => !allCk1ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.STICKER_CODE)))
                    .ToList();


            var brandNameSoonInvalid = brandSoonInvalidTemp.Select(x => x.BRAND_CE).Distinct().ToList();

            var brandSoonInvalid = brandSoonInvalidTemp.Where(x => brandNameSoonInvalid.Contains(x.BRAND_CE)).ToList();

            var brandSoonInvalidFinal =  brandSoonInvalid.Where(x => !allCk1ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.STICKER_CODE)));

            var allPreviousCk1ItemXmonths = _ck1Services.GetLastXMonthsCk1(xMonth, xCompesationDay, true);

            //Dictionary<ZAIDM_EX_BRAND,List<CK1>> dictBrandCk1 = new Dictionary<ZAIDM_EX_BRAND, List<CK1>>();
            List<InvalidBrandByCk1ForEmail> invalidCk1List = new List<InvalidBrandByCk1ForEmail>();
            
            var controllers = GetControllers();
            foreach (var brandInvalid in brandNameSoonInvalid)
            {


                var invalidCk1 = invalidCk1List.FirstOrDefault(x => x.BrandName == brandInvalid);
                

                if (invalidCk1 == null)
                {
                    var brandList = brandSoonInvalidFinal.Where(x => x.BRAND_CE == brandInvalid);
                    List<InvalidCk1Brand> invalidBrand = new List<InvalidCk1Brand>();
                    foreach (var zaidmExBrand in brandList)
                    {
                        var ck1IdList = allPreviousCk1ItemXmonths.Where(x => x.FA_CODE == zaidmExBrand.FA_CODE
                                                                             && x.WERKS == zaidmExBrand.WERKS
                                                                             && x.MATERIAL_ID == zaidmExBrand.STICKER_CODE
                                                                             && x.CK1_ID.HasValue
                                                                             )
                            .Select(x =>  x.CK1_ID.Value)
                            .ToList();
                        var lastCk1 = _ck1Services.GetCk1ByListContainIds(ck1IdList).OrderByDescending(x=> x.CK1_DATE).FirstOrDefault();
                        invalidBrand.Add(new InvalidCk1Brand()
                        {
                            FaCode = zaidmExBrand.FA_CODE,
                            Werks = zaidmExBrand.WERKS + " - " + zaidmExBrand.T001W.NAME1,
                            NppbkcId = zaidmExBrand.T001W.NPPBKC_ID,
                            PlantId = zaidmExBrand.WERKS,
                            StickerCode = zaidmExBrand.STICKER_CODE,
                            BrandCe = zaidmExBrand.BRAND_CE,
                            Hje = zaidmExBrand.HJE_IDR.Value.ToString("N2"),
                            Tariff = zaidmExBrand.TARIFF.Value.ToString("N2"),
                            SkepDate = zaidmExBrand.SKEP_DATE,
                            SkepNumber = zaidmExBrand.SKEP_NO,
                            SentFlag = zaidmExBrand.IS_SENT_CHECK_BRAND == null ? false : zaidmExBrand.IS_SENT_CHECK_BRAND.Value,
                            LastCk1 = lastCk1
                        });
                    }

                    invalidCk1 = new InvalidBrandByCk1ForEmail()
                    {
                        BrandName = brandInvalid,
                        BrandList = invalidBrand
                    };

                    invalidCk1List.Add(invalidCk1);
                    invalidCk1.userCc = controllers;
                    var listPlantBrand = brandList.Select(x => x.WERKS).Distinct().ToList();
                    invalidCk1.userTo = GetPoaExcisersByPlant(listPlantBrand);
                }
            }

            ProcessDataToEmail(invalidCk1List, allActiveBrand);
        }

        private void ProcessDataToEmail(List<InvalidBrandByCk1ForEmail> invalidCk1List, List<ZAIDM_EX_BRAND> allActiveBrand)
        {
            if (invalidCk1List.Count > 0)
            {
                //do email things here
                var success = false;
                var mailNotif = ProcessEmailQuotaNotification(invalidCk1List);
                if (mailNotif != null && mailNotif.IsDataExist)
                {
                    if (mailNotif.IsCCExist)
                    {
                        success = _messageService.SendEmailToListWithCC(mailNotif.To, mailNotif.CC, mailNotif.Subject, mailNotif.Body, false);
                    }
                    else
                    {
                        success = _messageService.SendEmailToList(mailNotif.To, mailNotif.Subject, mailNotif.Body, false);
                    }
                    var emailStatus = success ? Core.Enums.EmailStatus.Sent : Core.Enums.EmailStatus.NotSent;

                    if (success)
                    {
                        SetFlagSentForBrand(invalidCk1List, allActiveBrand);
                    }
                }
            }
        }

        private MailNotification ProcessEmailQuotaNotification(List<InvalidBrandByCk1ForEmail> invalidCk1List)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();
            var toList = new List<String>();

            rc.IsDataExist = false;

            foreach (var invalidCk1 in invalidCk1List)
            {
                toList.AddRange(invalidCk1.userTo.Where(x => x.EMAIL != "").Select(x => x.EMAIL).ToList());
            }

            rc.Subject = "Brands Never used on any CK-1 (domestic) in the last 5 months.";
            bodyMail.Append("Dear Team,<br />");

            bodyMail.Append("Kindly be informed, brands below are Never used on any CK-1 (domestic) in the last 5 months. <br />");

            bodyMail.Append(BuildBodyMailForQuotaNotification(invalidCk1List));

            rc.To.AddRange(toList.Distinct());

            rc.Body = bodyMail.ToString();
            foreach (var controller in invalidCk1List.FirstOrDefault().userCc)
            {
                rc.IsCCExist = true;
                rc.CC.Add(controller.EMAIL);
            }

            if (rc.Body.Contains("</td>"))
            {
                rc.IsDataExist = true;
            }

            return rc;
        }

        private string BuildBodyMailForQuotaNotification(List<InvalidBrandByCk1ForEmail> invalidCk1List)
        {
            var bodyMail = new StringBuilder();
            var bodyMailDeactivated = new StringBuilder();

            var tresHoldEndDate = DateTime.Today.AddMonths(-1 * (xMonth + 1));
            var tresHoldBeginDate = DateTime.Today.AddMonths(-1 * xMonth).AddDays(-1 * xCompesationDay);
            //var supplier = dataMonitoring.SUPPLIER_WERKS;
            bodyMail.Append("<table style='border-collapse: collapse; border: 1px solid black;'>" +
                            "<tr>" +
                            "<th style='border: 1px solid black; width : 320px'>Plant</th>" +
                            "<th style='border: 1px solid black;'>NPPBKC</th>" +
                            "<th style='border: 1px solid black; width : 150px'>FA Code </th>" +
                            "<th style='border: 1px solid black; width : 400px'>Brand Description</th>" +
                            "<th style='border: 1px solid black;'>HJE</th>" +
                            "<th style='border: 1px solid black;'>Tariff</th>" +
                            "<th style='border: 1px solid black;'>PO Number</th>" +
                            "<th style='border: 1px solid black;'>CK1 Number</th>" +
                            "<th style='border: 1px solid black; width : 120px'>CK1 Date</th>" +
                            "</tr>");

            bodyMailDeactivated.Append("<table style='border-collapse: collapse; border: 1px solid black;'>" +
                            "<tr>" +
                            "<th style='border: 1px solid black; width : 320px'>Plant</th>" +
                            "<th style='border: 1px solid black;'>NPPBKC</th>" +
                            "<th style='border: 1px solid black; width : 150px'>FA Code </th>" +
                            "<th style='border: 1px solid black; width : 400px'>Brand Description</th>" +
                            "<th style='border: 1px solid black;'>HJE</th>" +
                            "<th style='border: 1px solid black;'>Tariff</th>" +
                            "<th style='border: 1px solid black;'>PO Number</th>" +
                            "<th style='border: 1px solid black;'>CK1 Number</th>" +
                            "<th style='border: 1px solid black; width : 120px'>CK1 Date</th>" +
                            "</tr>");

            foreach (var invalidCk1 in invalidCk1List) { 
                foreach (var brand in invalidCk1.BrandList)
                {
                    if (!brand.SentFlag)
                    {
                        
                        var regDateTable = "";
                        brand.IsDeactivated = false;

                        //bodyMail.Append("<tr>" +
                        //            "<td style='border: 1px solid black;'>" + brand.Werks + "</td>" +
                        //            "<td style='border: 1px solid black; padding : 5px'>" + brand.NppbkcId + "</td>" +
                        //            "<td style='border: 1px solid black;'>" + brand.FaCode + "</td>" +
                        //            "<td style='border: 1px solid black;'>" + brand.BrandCe + "</td>" +
                        //            "<td style='border: 1px solid black; padding : 5px'>" + brand.Hje + "</td>" +
                        //            "<td style='border: 1px solid black; padding : 5px'>" + brand.Tariff + "</td>");
                        if (brand.LastCk1 != null)
                        {

                            if(brand.LastCk1.ORDER_DATE >= tresHoldBeginDate) continue;

                            regDateTable = "<td style='border: 1px solid black; padding : 5px'>" + brand.LastCk1.CK1_SAP_NUMBER + "</td>" +
                                "<td style='border: 1px solid black; padding : 5px'>" + brand.LastCk1.CK1_NUMBER + "</td>" +

                                "<td style='border: 1px solid black; padding : 5px'>" + brand.LastCk1.CK1_DATE.ToString("dd MMM yyyy") + "</td>" +
                                "</tr>";

                            if (brand.LastCk1.ORDER_DATE != null && brand.LastCk1.ORDER_DATE.Value <= tresHoldEndDate)
                            {
                                brand.IsDeactivated = true;
                            }
                            
                        }
                        else
                        {

                            if (brand.SkepDate != null)
                            {
                                if (brand.SkepDate >= tresHoldBeginDate) continue;

                                regDateTable = "<td style='border: 1px solid black;' colspan='2'> Skep : " +
                                               brand.SkepNumber + "</td>" +
                                               "<td style='border: 1px solid black; padding : 5px'>" +
                                               brand.SkepDate.Value.ToString("dd MMM yyyy") + "</td>" +
                                               "</tr>";

                                if (brand.SkepDate <= tresHoldEndDate)
                                {
                                    brand.IsDeactivated = true;
                                }
                            }
                            else
                            {
                                regDateTable = "<td style='border: 1px solid black;' colspan='3'>-</td>" +
                                               "</tr>";
                            }
                            
                        }

                        if (!brand.IsDeactivated)
                        {
                            bodyMail.Append("<tr>" +
                                            "<td style='border: 1px solid black;'>" + brand.Werks + "</td>" +
                                            "<td style='border: 1px solid black; padding : 5px'>" + brand.NppbkcId + "</td>" +
                                            "<td style='border: 1px solid black;'>" + brand.FaCode + "</td>" +
                                            "<td style='border: 1px solid black;'>" + brand.BrandCe + "</td>" +
                                            "<td style='border: 1px solid black; padding : 5px'>" + brand.Hje + "</td>" +
                                            "<td style='border: 1px solid black; padding : 5px'>" + brand.Tariff +
                                            "</td>");

                            bodyMail.Append(regDateTable);
                        }
                        else
                        {
                            bodyMailDeactivated.Append("<tr>" +
                                            "<td style='border: 1px solid black;'>" + brand.Werks + "</td>" +
                                            "<td style='border: 1px solid black; padding : 5px'>" + brand.NppbkcId + "</td>" +
                                            "<td style='border: 1px solid black;'>" + brand.FaCode + "</td>" +
                                            "<td style='border: 1px solid black;'>" + brand.BrandCe + "</td>" +
                                            "<td style='border: 1px solid black; padding : 5px'>" + brand.Hje + "</td>" +
                                            "<td style='border: 1px solid black; padding : 5px'>" + brand.Tariff +
                                            "</td>");

                            bodyMailDeactivated.Append(regDateTable);
                        }
                    }
                }
            }

            bodyMail.Append("</table>");
            bodyMail.AppendLine();

            bodyMail.Append("<br /><br />");
            bodyMail.Append("Brands below are Never used on any CK1  in more than 6 months, Brands listed below will be deactivated Automatically.");

            bodyMailDeactivated.Append("</table>");
            bodyMailDeactivated.AppendLine();


            bodyMail.Append(bodyMailDeactivated);

            bodyMail.Append("<br />Regards,<br />");

            return bodyMail.ToString();
        } 

        private List<USER> GetControllers()
        {
            var controllersList = new List<USER>();

            var data = _repositoryUser.Get(x => (!x.IS_ACTIVE.HasValue || x.IS_ACTIVE != 0) && x.BROLE_MAP.Any(y => y.ROLEID == Sampoerna.EMS.Core.Enums.UserRole.Controller && y.MSACCT.ToLower() == x.USER_ID.ToLower()), null, "BROLE_MAP").ToList();


            controllersList.AddRange(data);
            return controllersList;
        }

        private List<USER> GetPoaExcisersByPlant(List<string> werks)
        {
            var excisersList = new List<USER>();

            var data = _repositoryPOAExciser.Get(null, null, "").Select(x=> x.POA_ID.ToUpper()).ToList();
            var poaByPlant =
                _repositoryPoaMapRepository.Get(x => werks.Contains(x.WERKS) && data.Contains(x.POA_ID.ToUpper()), null, "")
                    .Select(x => x.POA_ID.ToUpper())
                    .ToList();
            excisersList = _repositoryUser.Get(x => poaByPlant.Contains(x.USER_ID), null, "").ToList();

            return excisersList;
        }

        private void SetFlagSentForBrand(List<InvalidBrandByCk1ForEmail> invalidCk1List, List<ZAIDM_EX_BRAND> allActiveBrand)
        {
            foreach (var invalidCk1 in invalidCk1List)
            {
                foreach (var brand in invalidCk1.BrandList)
                {
                    var item = allActiveBrand.Where(x => x.WERKS == brand.PlantId && x.FA_CODE == brand.FaCode && x.STICKER_CODE == brand.StickerCode).FirstOrDefault();

                    item.IS_SENT_CHECK_BRAND = true;
                    item.STATUS = !brand.IsDeactivated;
                    _brandRegistrationService.Save(item);
                }
            }

            uow.SaveChanges();
        }

        private void DeleteFlagSentForBrand(List<ZAIDM_EX_BRAND> allValidBrand)
        {
            foreach (var item in allValidBrand)
            {
                item.IS_SENT_CHECK_BRAND = false;

                _brandRegistrationService.Save(item);
            }

            uow.SaveChanges();
        }
    }
}
