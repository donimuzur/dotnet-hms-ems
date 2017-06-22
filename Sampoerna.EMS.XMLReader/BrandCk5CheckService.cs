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

        private ICK1Services _ck1Services;
        private ICK5Service _ck5Services;
        private IBrandRegistrationService _brandRegistrationService;
        private MessageService _messageService;
        private IGenericRepository<USER> _repositoryUser;
        private IGenericRepository<POA_EXCISER> _repositoryPOAExciser;
        private IGenericRepository<POA_MAP> _repositoryPoaMapRepository;

        private static int xMonth = 5;

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

        private void ProcessDataToEmail(List<InvalidBrandByCk5ForEmail> invalidCk5List)
        {
            //do email things here
            var success = false;
            var mailNotif = ProcessEmailQuotaNotification(invalidCk5List);
            if (mailNotif != null)
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

            }
        }

        private MailNotification ProcessEmailQuotaNotification(List<InvalidBrandByCk5ForEmail> invalidCk5List)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();
            var toList = new List<String>();

            foreach (var invalidCk1 in invalidCk5List)
            {
                toList.AddRange(invalidCk1.userTo.Where(x => x.EMAIL != "").Select(x => x.EMAIL).ToList());
            }

            rc.Subject = "Brands Never used on any CK-1 (domestic) in the last 5 months.";
            bodyMail.Append("Dear Team,<br />");

            bodyMail.Append("Kindly be informed, brands below are Never used on any CK-1 (domestic) in the last 5 months. <br />");

            bodyMail.Append(BuildBodyMailForQuotaNotification(invalidCk5List));

            rc.To.AddRange(toList.Distinct());

            rc.Body = bodyMail.ToString();
            foreach (var controller in invalidCk5List.FirstOrDefault().userCc)
            {
                rc.IsCCExist = true;
                rc.CC.Add(controller.EMAIL);
            }

            return rc;
        }

        private string BuildBodyMailForQuotaNotification(List<InvalidBrandByCk5ForEmail> invalidCk5List)
        {
            var bodyMail = new StringBuilder();
            //var supplier = dataMonitoring.SUPPLIER_WERKS;
            bodyMail.Append("<table style='border-collapse: collapse; border: 1px solid black;'>" +
                            "<tr>" +
                            "<th style='border: 1px solid black;'>Plant</th>" +
                            "<th style='border: 1px solid black;'>FA Code </th>" +
                            "<th style='border: 1px solid black;'>Brand Description</th>" +
                            "<th style='border: 1px solid black;'>HJE</th>" +
                            "<th style='border: 1px solid black;'>Tariff</th>" +
                            "<th style='border: 1px solid black;'>CK1 Number</th>" +
                            "<th style='border: 1px solid black;'>Ck1 Date</th>" +
                            "</tr>");

            foreach (var invalidCk5 in invalidCk5List)
            {
                foreach (var brand in invalidCk5.BrandList)
                {
                    bodyMail.Append("<tr>" +
                                    "<td style='border: 1px solid black;'>" + brand.Werks + "</td>" +
                                    "<td style='border: 1px solid black;'>" + brand.FaCode + "</td>" +
                                    "<td style='border: 1px solid black;'>" + brand.BrandCe + "</td>" +
                                    "<td style='border: 1px solid black;'>" + brand.Hje + "</td>" +
                                    "<td style='border: 1px solid black;'>" + brand.Tariff + "</td>");
                    if (brand.LastCk5 != null)
                    {
                        bodyMail.Append(
                            "<td style='border: 1px solid black;'>" + brand.LastCk5.SUBMISSION_NUMBER + "</td>" +
                            "<td style='border: 1px solid black;'>" + brand.LastCk5.REGISTRATION_DATE.Value.ToString("dd MMM yyyy") + "</td>" +
                            "</tr>");
                    }
                    else
                    {
                        bodyMail.Append(
                            "<td style='border: 1px solid black;' colspan='2'>-</td>" +

                            "</tr>");
                    }
                }
            }

            bodyMail.Append("</table>");
            bodyMail.AppendLine();
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


        public void BrandCheckProcessCk5()
        {
            var allActiveBrand = _brandRegistrationService.GetAllActiveBrand("02"); //domestic is "01" , export is "02"
            var allLastCk5ItemXmonths = _ck5Services.GetLastXMonthsCk5(xMonth);

            var allCk5ItemCode = allLastCk5ItemXmonths.Select(x => x.BRAND + "-" + x.PLANT_ID).ToList();

            var brandSoonInvalidTemp =
                allActiveBrand.Where(x => !allCk5ItemCode.Contains((x.FA_CODE + "-" + x.WERKS)))
                    .ToList();


            var brandNameSoonInvalid = brandSoonInvalidTemp.Select(x => x.BRAND_CE).Distinct().ToList();

            var brandSoonInvalid = brandSoonInvalidTemp.Where(x => brandNameSoonInvalid.Contains(x.BRAND_CE)).ToList();

            var brandSoonInvalidFinal = brandSoonInvalid.Where(x => !allCk5ItemCode.Contains((x.FA_CODE + "-" + x.WERKS)));

            var allPreviousCk5ItemXmonths = _ck5Services.GetLastXMonthsCk5(xMonth, true);

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
                                                                             && x.CK5_ID.HasValue)
                            .Select(x => x.CK5_ID.Value)
                            .ToList();
                        var lastCk5 = _ck5Services.GetCk5ByListContainIds(ck5IdList).OrderByDescending(x => x.REGISTRATION_DATE).FirstOrDefault();
                        invalidBrand.Add(new InvalidCk5Brand()
                        {
                            FaCode = zaidmExBrand.FA_CODE,
                            Werks = zaidmExBrand.WERKS,
                            BrandCe = zaidmExBrand.BRAND_CE,
                            Hje = zaidmExBrand.HJE_IDR.Value.ToString("N2"),
                            Tariff = zaidmExBrand.TARIFF.Value.ToString("N2"),
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

            ProcessDataToEmail(invalidCk5List);
        }
    }
}
