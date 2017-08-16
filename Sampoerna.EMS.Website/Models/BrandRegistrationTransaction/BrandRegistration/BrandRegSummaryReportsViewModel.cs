using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration
{
    public class BrandRegSummaryReportsViewModel : BaseModel
    {
        public BrandRegSummaryReportsViewModel()
        {
            SearchView = new BrandRegSearchSummaryReportsViewModel();
            DetailsList = new List<vwBrandRegistrationModel>();
            BrandRegDocuments = new List<BrandRegistrationReqModel>();
        }

        public BrandRegSearchSummaryReportsViewModel SearchView { get; set; }
        public List<vwBrandRegistrationModel> DetailsList { get; set; }
        public List<BrandRegistrationReqModel> BrandRegDocuments { set; get; }

        public BrandRegExportSummaryReportsViewModel ExportModel { get; set; }

        //public Enums.BrandRegType BrandRegType { get; set; }

        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }

        
    }

    public class BrandRegSearchSummaryReportsViewModel
    {
        //public Enums.BrandRegType BrandRegType { get; set; }
        //public Enums.BrandRegType BrandRegTypeList { get; set; }

        public int RegistrationTypeSource { get; set; }
        public IEnumerable<SelectListItem> ListRegistrationType { get; set; }


        public string CreatorSource { set; get; }

        public SelectList CreatorList { set; get; }

    }

    public class BrandRegExportSummaryReportsViewModel : BrandRegSearchSummaryReportsViewModel
    {
        public bool RegistrationId { get; set; }
        public bool RegistrationNo { get; set; }
        public bool SubmissionDate { get; set; }
        public bool RegistrationType { get; set; }
        public bool NppbkcId { get; set; }
        public bool CompanyName { get; set; }
        public bool EffectiveDate { get; set; }
        public bool Creator { get; set; }
        public bool CreatorDate { get; set; }
        public bool LastModifiedBy { get; set; }
        public bool LastModifiedDate { get; set; }
        public bool LastApprovedBy { get; set; }
        public bool LastApprovedDate { get; set; }
        public bool LastApprovedStatus { get; set; }
        public bool DecreeStatus { get; set; }
        public bool DecreeNumber { get; set; }
        public bool DecreeDate { get; set; }
        public bool DecreeStartDate { get; set; }

        public bool NoRow { get; set; }

        public BrandRegistrationDetailExportSummaryReportsViewModel DetailExportModel { get; set; }
    }

    public class BrandRegistrationDetailExportSummaryReportsViewModel
    {

        public bool BrandName { get; set; }
        public bool ProductType { get; set; }
        public bool CompanyTier { get; set; }
        public bool HJE { get; set; }
        public bool Unit { get; set; }
        public bool BrandContent { get; set; }
        public bool Tarif { get; set; }
        public bool MaterialPackage { get; set; }
        public bool MarketDesc { get; set; }
        public bool FrontSide { get; set; }

        public bool BackSide { get; set; }
        public bool LeftSide { get; set; }
        public bool RightSide { get; set; }
        public bool TopSide { get; set; }
        public bool BottomSide { get; set; }
    }


}