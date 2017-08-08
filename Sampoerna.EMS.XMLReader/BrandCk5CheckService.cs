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


    public class BrandCk5CheckService
    {
        public ILogger logger;
        public IUnitOfWork uow;

        private ICK5Service _ck5Services;
        private IBrandRegistrationService _brandRegistrationService;
        private MessageService _messageService;
        private IGenericRepository<USER> _repositoryUser;
        private IGenericRepository<POA_EXCISER> _repositoryPOAExciser;
        private IGenericRepository<POA_MAP> _repositoryPoaMapRepository;

        private static int xMonth = 5;
        private static int xCompesationDay = 2;

        public BrandCk5CheckService()
        {
            logger = new NLogLogger();
            uow = new SqlUnitOfWork(logger);

            _ck5Services = new CK5Service(uow, logger);
            _brandRegistrationService = new BrandRegistrationService(uow, logger);
            _messageService = new MessageService(logger);

            _repositoryUser = uow.GetGenericRepository<USER>();
            _repositoryPOAExciser = uow.GetGenericRepository<POA_EXCISER>();
            _repositoryPoaMapRepository = uow.GetGenericRepository<POA_MAP>();
        }

        public void BrandCheckProcessCk5()
        {
            var allActiveBrand = _brandRegistrationService.GetAllActiveBrand("02"); //domestic is "01" , export is "02"
            var allLastCk5ItemXmonths = _ck5Services.GetLastXMonthsCk5(xMonth,xCompesationDay);

            var allCk5ItemCode = allLastCk5ItemXmonths.Select(x => x.BRAND + "-" + x.PLANT_ID + "-" + x.HJE + "-"  + x.TARIFF).ToList();

            var brandSoonValidTemp =
                allActiveBrand.Where(x => allCk5ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.HJE_IDR + "-" + x.TARIFF)))
                    .ToList();

            DeleteFlagSentForBrand(brandSoonValidTemp);

            var brandSoonInvalidTemp =
                allActiveBrand.Where(x => !allCk5ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.HJE_IDR + "-" + x.TARIFF)))
                    .ToList();


            var brandNameSoonInvalid = brandSoonInvalidTemp.Select(x => x.BRAND_CE).Distinct().ToList();

            var brandSoonInvalid = brandSoonInvalidTemp.Where(x => brandNameSoonInvalid.Contains(x.BRAND_CE)).ToList();

            var brandSoonInvalidFinal = brandSoonInvalid.Where(x => !allCk5ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.HJE_IDR + "-" + x.TARIFF)));

            var allPreviousCk5ItemXmonths = _ck5Services.GetLastXMonthsCk5(xMonth,xCompesationDay, true);

            //Dictionary<ZAIDM_EX_BRAND,List<CK1>> dictBrandCk1 = new Dictionary<ZAIDM_EX_BRAND, List<CK1>>();
            List<InvalidBrandByCk5ForEmail> invalidCk5List = new List<InvalidBrandByCk5ForEmail>();

            var controllers = GetControllers();
            foreach (var brandInvalid in brandNameSoonInvalid)
            {


                var invalidCk5 = invalidCk5List.FirstOrDefault(x => x.BrandName == brandInvalid);


                if (invalidCk5 == null)
                {
                    var brandList = brandSoonInvalidFinal.Where(x => x.BRAND_CE == brandInvalid);
                    List<InvalidCk5Brand> invalidBrand = new List<InvalidCk5Brand>();
                    foreach (var zaidmExBrand in brandList)
                    {
                        var ck5IdList = allPreviousCk5ItemXmonths.Where(x => x.BRAND == zaidmExBrand.FA_CODE
                                                                             && x.PLANT_ID == zaidmExBrand.WERKS
                                                                             && x.HJE == zaidmExBrand.HJE_IDR
                                                                             && x.TARIFF == zaidmExBrand.TARIFF
                                                                             && x.CK5_ID.HasValue)
                            .Select(x => x.CK5_ID.Value)
                            .ToList();
                        var lastCk5 = _ck5Services.GetCk5ByListContainIds(ck5IdList).OrderByDescending(x => x.REGISTRATION_DATE).FirstOrDefault();
                        invalidBrand.Add(new InvalidCk5Brand()
                        {
                            FaCode = zaidmExBrand.FA_CODE,
                            Werks = zaidmExBrand.WERKS + " - " + zaidmExBrand.T001W.NAME1,
                            PlantId = zaidmExBrand.WERKS,
                            StickerCode = zaidmExBrand.STICKER_CODE,
                            BrandCe = zaidmExBrand.BRAND_CE,
                            Hje = zaidmExBrand.HJE_IDR.Value.ToString("N2"),
                            Tariff = zaidmExBrand.TARIFF.Value.ToString("N2"),
                            SkepDate = zaidmExBrand.SKEP_DATE,
                            SkepNumber = zaidmExBrand.SKEP_NO,
                            SentFlag = zaidmExBrand.IS_SENT_CHECK_BRAND == null ? false : zaidmExBrand.IS_SENT_CHECK_BRAND.Value,
                            LastCk5 = lastCk5
                        });
                    }

                    invalidCk5 = new InvalidBrandByCk5ForEmail()
                    {
                        BrandName = brandInvalid,
                        BrandList = invalidBrand
                    };

                    invalidCk5List.Add(invalidCk5);
                    invalidCk5.userCc = controllers;
                    var listPlantBrand = brandList.Select(x => x.WERKS).Distinct().ToList();
                    invalidCk5.userTo = GetPoaExcisersByPlant(listPlantBrand);
                }
            }

            ProcessDataToEmail(invalidCk5List, allActiveBrand);
        }

        private void ProcessDataToEmail(List<InvalidBrandByCk5ForEmail> invalidCk5List, List<ZAIDM_EX_BRAND> allActiveBrand)
        {
            if (invalidCk5List.Count > 0)
            {
                //do email things here
                var success = false;
                var mailNotif = ProcessEmailQuotaNotification(invalidCk5List);
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
                        SetFlagSentForBrand(invalidCk5List, allActiveBrand);
                    }
                }
            }
        }

        private MailNotification ProcessEmailQuotaNotification(List<InvalidBrandByCk5ForEmail> invalidCk5List)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();
            var toList = new List<String>();

            List<ZAIDM_EX_BRAND> brandDeactivated = new List<ZAIDM_EX_BRAND>();

            foreach (var invalidCk5 in invalidCk5List)
            {
                toList.AddRange(invalidCk5.userTo.Where(x => x.EMAIL != "").Select(x => x.EMAIL).ToList());
            }

            rc.Subject = "Brands Never used on any CK-5 (export) in the last 5 months.";
            bodyMail.Append("Dear Team,<br />");

            bodyMail.Append("Kindly be informed, brands below are Never used on any CK-5 (export) in the last 5 months. <br />");

            bodyMail.Append(BuildBodyMailForQuotaNotification(invalidCk5List));

            rc.To.AddRange(toList.Distinct());

            rc.Body = bodyMail.ToString();
            foreach (var controller in invalidCk5List.FirstOrDefault().userCc)
            {
                rc.IsCCExist = true;
                rc.CC.Add(controller.EMAIL);
            }

            foreach (var invalidCk5 in invalidCk5List)
            {
                foreach (var brand in invalidCk5.BrandList)
                {
                    if (!brand.SentFlag)
                    {
                        rc.IsDataExist = true;
                    }
                }
            }

            return rc;
        }

        private string BuildBodyMailForQuotaNotification(List<InvalidBrandByCk5ForEmail> invalidCk5List)
        {
            var bodyMail = new StringBuilder();
            var bodyMailDeactivated = new StringBuilder();

            var tresHoldEndDate = DateTime.Today.AddMonths(-1*(xMonth + 1));
            var tresHoldBeginDate = DateTime.Today.AddMonths(-1 * xMonth).AddDays(-1 * xCompesationDay);
            
            bodyMail.Append("<table style='border-collapse: collapse; border: 1px solid black;'>" +
                            "<tr>" +
                            "<th style='border: 1px solid black; width : 320px'>Plant</th>" +
                            "<th style='border: 1px solid black; width : 150px'>FA Code </th>" +
                            "<th style='border: 1px solid black; width : 400px'>Brand Description</th>" +
                            "<th style='border: 1px solid black;'>HJE</th>" +
                            "<th style='border: 1px solid black;'>Tariff</th>" +
                            "<th style='border: 1px solid black;'>CK5 Number</th>" +
                            "<th style='border: 1px solid black;'>Registration Number</th>" +
                            "<th style='border: 1px solid black;'>Registration Date</th>" +
                            "</tr>");
            bodyMailDeactivated.Append("<table style='border-collapse: collapse; border: 1px solid black;'>" +
                                       "<tr>" +
                                       "<th style='border: 1px solid black; width : 320px'>Plant</th>" +
                                       "<th style='border: 1px solid black; width : 150px'>FA Code </th>" +
                                       "<th style='border: 1px solid black; width : 400px'>Brand Description</th>" +
                                       "<th style='border: 1px solid black;'>HJE</th>" +
                                       "<th style='border: 1px solid black;'>Tariff</th>" +
                                       "<th style='border: 1px solid black;'>CK5 Number</th>" +
                                       "<th style='border: 1px solid black;'>Registration Number</th>" +
                                       "<th style='border: 1px solid black;'>Registration Date</th>" +
                                       "</tr>");

            foreach (var invalidCk5 in invalidCk5List)
            {
                foreach (var brand in invalidCk5.BrandList)
                {
                    if (!brand.SentFlag)
                    {
                        var regDateTable = "";
                        brand.IsDeactivated = false;
                        

                        if (brand.LastCk5 != null)
                        {
                            if (brand.LastCk5.REGISTRATION_DATE >= tresHoldBeginDate) continue;

                            regDateTable = "<td style='border: 1px solid black; padding : 5px'>" +
                                           brand.LastCk5.SUBMISSION_NUMBER + "</td>" +
                                           "<td style='border: 1px solid black; padding : 5px'>" +
                                           brand.LastCk5.REGISTRATION_NUMBER + "</td>" +
                                           "<td style='border: 1px solid black; padding : 5px'>" +
                                           brand.LastCk5.REGISTRATION_DATE.Value.ToString("dd MMM yyyy") + "</td>" +
                                           "</tr>";

                            if (brand.LastCk5.REGISTRATION_DATE.Value <= tresHoldEndDate)
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
            bodyMail.Append("Brands below are Never used on any CK-5 (export) in more than 6 months, Brands listed below will be deactivated Automatically.");

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

            var data = _repositoryPOAExciser.Get(null, null, "").Select(x => x.POA_ID.ToUpper()).ToList();
            var poaByPlant =
                _repositoryPoaMapRepository.Get(x => werks.Contains(x.WERKS) && data.Contains(x.POA_ID.ToUpper()), null, "")
                    .Select(x => x.POA_ID.ToUpper())
                    .ToList();
            excisersList = _repositoryUser.Get(x => poaByPlant.Contains(x.USER_ID), null, "").ToList();

            return excisersList;
        }

        private void SetFlagSentForBrand(List<InvalidBrandByCk5ForEmail> invalidCk5List, List<ZAIDM_EX_BRAND> allActiveBrand)
        {
            foreach (var invalidCk5 in invalidCk5List)
            {
                foreach (var brand in invalidCk5.BrandList)
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
                try
                {
                    item.IS_SENT_CHECK_BRAND = false;

                    _brandRegistrationService.Save(item);
                }
                catch (Exception ex)
                {
                    var error = ex.InnerException;
                }
            }

            uow.SaveChanges();
        }
    }
}
