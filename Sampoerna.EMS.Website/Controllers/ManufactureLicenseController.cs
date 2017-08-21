using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ManufactureLicense;
using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ManufactureLicenseController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        //private ManufactureLicenseService service;
        private IChangesHistoryBLL chBLL;
        private IWorkflowHistoryBLL whBLL;
        public ManufactureLicenseController(IPageBLL pageBLL, IChangesHistoryBLL changeHistoryBLL, IWorkflowHistoryBLL workflowHistoryBLL) : base(pageBLL, Enums.MenuList.ManufactureLicense)
        {
            this.mainMenu = Enums.MenuList.ManufactureLicense;
            //this.service = new InterviewRequestModel();
            this.refService = new SystemReferenceService();
            this.chBLL = changeHistoryBLL;
            this.whBLL = workflowHistoryBLL;
        }

        public ActionResult Index()
        {
            var users = refService.GetAllUser();
            var poaList = refService.GetAllPOA();
            //var documents = service.GetAll();
            var model = new ManufactureLicenseViewModel()
            {
                MainMenu = mainMenu,
                CurrentMenu = PageInfo,
                //Filter = new ExciseFilterModel(),
                //CreatorList = GetUserList(users),
                //ManufactureLicenseDocuments = documents,
                //NppbkcList = GetNppbkcList(refService.GetAllNppbkc()),
                //PoaList = GetPoaList(refService.GetAllPOA()),
                //TypeList = GetManufactureLicenseTypeList(service.GetManufactureLicenseTypes()),
                //YearList = GetYearList(documents),
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Controller && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator)
            };

            return View("Index", model);
            
        }

        //public ActionResult Create()
        //{
        //    var model = new ManufactureLicenseFormModel()
        //    {
        //        NppbkcList = GetNppbkcList(refService.GetAllNppbkc()),
        //        GuaranteeTypes = GetManufactureLicenseGuaranteeList(service.GetManufactureLicenseGuarantees()),
        //        POA = MapPoaModel(refService.GetPOA(CurrentUser.USER_ID))
        //    };
        //    return View("Index", model);
        //}

        //#region Helpers
        //private SelectList GetYearList(IEnumerable<ManufactureLicenseModel> ManufactureLicenses)
        //{
        //    var query = from x in ManufactureLicenses
        //                select new SelectItemModel()
        //                {
        //                    ValueField = x.SubmissionDate.Year,
        //                    TextField = x.SubmissionDate.Year.ToString()
        //                };
        //    return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        //}

        //private SelectList GetManufactureLicenseTypeList(Dictionary<int, string> types)
        //{
        //    var query = from x in types
        //                select new SelectItemModel()
        //                {
        //                    ValueField = x.Key,
        //                    TextField = x.Value
        //                };
        //    return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        //}

        //private SelectList GetManufactureLicenseGuaranteeList(Dictionary<int, string> guarantees)
        //{
        //    var query = from x in guarantees
        //                select new SelectItemModel()
        //                {
        //                    ValueField = x.Key,
        //                    TextField = x.Value
        //                };
        //    return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        //}

        //private SelectList GetPoaList(IEnumerable<CustomService.Data.POA> poaList)
        //{
        //    var query = from x in poaList
        //                select new SelectItemModel()
        //                {
        //                    ValueField = x.POA_ID,
        //                    TextField = String.Format("{0} {1}", x.USER_LOGIN.FIRST_NAME, x.USER_LOGIN.LAST_NAME)
        //                };
        //    return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        //}

        //private SelectList GetUserList(IEnumerable<CustomService.Data.USER> userList)
        //{
        //    var query = from x in userList
        //                select new SelectItemModel()
        //                {
        //                    ValueField = x.USER_ID,
        //                    TextField = String.Format("{0} {1}", x.FIRST_NAME, x.LAST_NAME)
        //                };
        //    return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        //}

        //private SelectList GetNppbkcList(IEnumerable<CustomService.Data.MASTER_NPPBKC> nppbkcList)
        //{
        //    var query = from x in nppbkcList
        //                select new SelectItemModel()
        //                {
        //                    ValueField = x.NPPBKC_ID,
        //                    TextField = x.NPPBKC_ID
        //                };
        //    return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        //}

        //#region Mappings

        //private UserModel MapToUserModel(CustomService.Data.USER user)
        //{
        //    try
        //    {
        //        return new UserModel()
        //        {
        //            UserId = user.USER_ID,
        //            FirstName = user.FIRST_NAME, 
        //            LastName = user.LAST_NAME
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
            
        //}
        //private ManufactureLicensePOA MapPoaModel(CustomService.Data.POA poa)
        //{
        //    try
        //    {
        //        return new ManufactureLicensePOA()
        //        {
        //            Id = poa.POA_ID,
        //            Name = String.Format("{0} {1}", poa.USER_LOGIN.FIRST_NAME, poa.USER_LOGIN.LAST_NAME),
        //            Address = poa.POA_ADDRESS,
        //            Position = poa.TITLE
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private ManufactureLicenseNppbkc MapNppbkcModel(CustomService.Data.MASTER_NPPBKC nppbkc)
        //{
        //    try
        //    {
        //        return new ManufactureLicenseNppbkc()
        //        {
        //            Id = nppbkc.NPPBKC_ID,
        //            Region = nppbkc.REGION,
        //            Address = String.Format("{0}, {1}", nppbkc.ADDR1, nppbkc.ADDR2),
        //            City = nppbkc.CITY,
        //            KppbcId = nppbkc.KPPBC_ID,
        //            Company = new CompanyModel()
        //            {
        //                Id = nppbkc.COMPANY.BUKRS,
        //                Name = nppbkc.COMPANY.BUTXT
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //#endregion

        //private ManufactureLicenseModel MapManufactureLicenseModel(CustomService.Data.EXCISE_CREDIT entity)
        //{
        //    try
        //    {
        //        List<FinanceRatioModel> financialRatio = new List<FinanceRatioModel>();
        //        financialRatio.Add(new FinanceRatioModel()
        //        {
        //            LiquidityRatio = entity.LIQUIDITY_RATIO_1,
        //            SolvencyRatio = entity.SOLVENCY_RATIO_1,
        //            RentabilityRatio = entity.RENTABILITY_RATIO_1
        //        });
        //        financialRatio.Add(new FinanceRatioModel()
        //        {
        //            LiquidityRatio = entity.LIQUIDITY_RATIO_2,
        //            SolvencyRatio = entity.SOLVENCY_RATIO_2,
        //            RentabilityRatio = entity.RENTABILITY_RATIO_2
        //        });
        //        return new ManufactureLicenseModel()
        //        {
        //            Id = entity.EXSICE_CREDIT_ID,
        //            DocumentNumber = entity.EXCISE_CREDIT_NO,
        //            SubmissionDate = entity.SUBMISSION_DATE,
        //            RequestTypeID = entity.REQUEST_TYPE,
        //            RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
        //            NppbkcId = entity.NPPBKC_ID,
        //            Guarantee = entity.GUARANTEE,
        //            FinanceRatios = financialRatio,
        //            //SkepLastStatus = entity.SKEP_STATUS,
        //            //SkepStatus = AutoMapper.Mapper.Map<ReferenceModel>(entity.s)
        //            DecreeNumber = entity.DECREE_NO,
        //            DecreeDate = entity.DECREE_DATE,
        //            DecreeStartDate = entity.DECREE_STARTDATE,
        //            BpjNumber = entity.BPJ_NO,
        //            BpjDate = entity.BPJ_DATE,
        //            BpjAttachmentUrl = entity.BPJ_ATTACH,
        //            CalculatedAdjustment = entity.ADJUSTMENT_CALCULATED,
        //            CalculatedAdjustmentDisplay = entity.ADJUSTMENT_CALCULATED.ToString("C2"),
        //            Notes = entity.NOTES,
        //            Amount = entity.EXCISE_CREDIT_AMOUNT,
        //            AmountDisplay = entity.EXCISE_CREDIT_AMOUNT.ToString("C2"),
        //            CreatedBy = entity.CREATED_BY

                    

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        //var msg = String.Format("Message: {0}\nStack Trace: {1}\nInner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException);
        //        //AddMessageInfo(msg, Enums.MessageInfoType.Error);
        //        throw ex;
        //    }
        //}

        //private string GetRequestTypeName(int id)
        //{
        //    return service.GetManufactureLicenseTypeName(id);
        //}
        //#endregion
    }
}
