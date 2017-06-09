using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ManufacturingLicense
{

    public class LicenseRequestModel : BaseModel
    {
        public LicenseRequestModel()
        {
            PCDetails = new List<ProductTypeDetails>();
            IRDetails = new List<InterviewRequestDetails>();
            interviewRequestDetails = new List<InterviewRequestDetailModel>();
            ProdCode = new List<string>();
            File_BA_Path = new List<string>();
            File_Other_Path = new List<string>();
            LicenseRequestFileOtherList = new List<LicenseRequestFileOtherModel>();
            RemovedFilesId = new List<long>();
            File_Other_Name = new List<string>();
            LicenseRequestFileBAList = new List<LicenseRequestFileOtherModel>();
            Confirmation = new List<ConfirmDialogModel>();
        }


        public long Id { get; set; }
        public string Status { get; set; }
        public string Status_Value { get; set; }
        public string MnfFormNum { get; set; }
        public string BANum { get; set; }
        public string InterviewFormNum { get; set; }
        public long InterviewId { get; set; }
        public DateTime RequestDate { get; set; }
        public string StrRequestDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public string LastApprovedBy { get; set; }
        public DateTime LastApprovedDate { get; set; }
        public List<string> ProdCode { get; set; }
        public long LastApprovedStatus { get; set; }
        public long MnfRequestID { get; set; }
        public string DecreeNo { get; set; }
        public DateTime? DecreeDate { get; set; }
        public bool? DecreeStatus { get; set; }
        public string NppbkcID { get; set; }
        public string KPBCName { get; set; }
        public string KPPBC { get; set; }
        public string KppbcAddress { get; set; }
        public string POAAddress { get; set; }        
        public string CompType { get; set; }
        public string Company { get; set; }
        public string CompanyId { get; set; }
        public string POAName { get; set; }
        public string POAPosition { get; set; }
        public string Npwp { get; set; }
        public string CompanyAddress { get; set; }
        public string ProdType { get; set; }
        public string Comment { get; set; }
        //public string Notes { get; set; } 
        public bool IsFormReadOnly { get; set; }
        public long File_Size { get; set; }
        public bool IsFormSkep { get; set; }
        public bool IsApprover { get; set; }
        public string StatusKey { get; set; }
        public bool IsCanEdit { get; set; }
        public string IsCanApprove { get; set; }
        public string IsPage { get; set; }
        public string link_BA { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsViewer { get; set; }
        public string IsRole { get; set; }

        public List<string> OtherProdCode { get; set; }

        public SelectList FormNumList { get; set; }
        
        public SelectList NPPBKCList { get; set; }
        public SelectList ProdTypeList { get; set; }
        public SelectList GovStatus_List { get; set; }
        public int CountDetailManufacture { get; set; }
        public List<ProductTypeDetails> PCDetails { get; set; }
        public List<InterviewRequestDetails> IRDetails { get; set; }
        public List<ProductTypeDetails> OtherPCDetails { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public List<InterviewRequestDetailModel> interviewRequestDetails { get; set; }
        public List<long> RemovedFilesId { set; get; }
        public List<HttpPostedFileBase> File_Other { get; set; }
        public List<string> File_Other_Path { set; get; }
        public List<string> File_Other_Name { set; get; }
        public List<HttpPostedFileBase> File_BA { get; set; }
        public List<string> File_BA_Path { set; get; }
        public List<long> RemovedDetailId { set; get; }
        public List<LicenseRequestSupportingDocModel> LicenseSupportingDocumentMaster { get; set; }
        public List<InterviewRequestDetails> LicenseRequestBoundCondition { get; set; }
        public List<ProductTypeDetails> LicenseRequestProductType { get; set; }
        public List<ProductTypeDetails> LicenseRequestOtherProductType { get; set; }
        public List<LicenseRequestFileOtherModel> LicenseRequestFileOtherList { get; set; }
        public List<LicenseRequestFileOtherModel> LicenseRequestFileBAList { get; set; }
        public IEnumerable<LicenseRequestSupportingDocModel> LicenseRequestSupportingDoc { get; set; }

        public List<string> List_ProdType { set; get; }
        public int Count_List_ProdType { set; get; }
        public int Lampiran_Count { get; set; }

        public string POA_ID { get; set; }
        public string KPPBC_ID { get; set; }
        public string KPPBC_Address { get; set; }
        public string Company_Address { get; set; }
        public string Company_ID { get; set; }
        public DateTime? BADate { get; set; }
        public string BANumber { get; set; }
        public bool? BAStatus { get; set; }
        public string Perihal { get; set; }
        public string Company_Type { get; set; }
        public string City { get; set; }
        public string City_Alias { get; set; }
        public List<ConfirmDialogModel> Confirmation { get; set; }        
    }

    public class ProductTypeDetails
    {
        public long ProdTypeId { get; set; }
        public string OtherProdCode { get; set; }
        public string ProdCode { get; set; }
        public string ProdAlias { get; set; }
        public string IsChecked { get; set; }
        public int ptIndex { get; set; }
        
    }
    
    public class InterviewRequestDetails
    {
        public long MnfDetailId { get; set; }
        public string MnfAddr { get; set; }
        public string MnfVillage { get; set; }
        public string MnfPhone { get; set; }
        public long MnfProvinceId { get; set; }
        public string MnfProvince { get; set; }
        public string MnfCity { get; set; }
        public long MnfCityId { get; set; }
        public string MnfSubDist { get; set; }
        public string MnfFax { get; set; }
        public int MnfCount { get; set; }
        public int Index { get; set; }
        public int NumList { get; set; }
        public string North { get; set; }
        public string East { get; set; }
        public string South { get; set; }
        public string West { get; set; }
        public decimal LandArea { get; set; }
        public decimal BuildingArea { get; set; }
        public string OwnershipStatus { get; set; }
        public long InterviewDetailId { get; set; }
        public long MnfReqId { get; set; }
        public string ActionName { get; set; }
    }

    public class LicenseRequestSupportingDocModel
    {
        public LicenseRequestSupportingDocModel()
        {

        }
        public long Id { set; get; }
        public long? DocId { set; get; }
        public long FileUploadId { set; get; }
        public string FileName { set; get; }
        public string Name { set; get; }
        public CompanyModel Company { set; get; }
        public HttpPostedFileBase File { set; get; }
        public string Path { set; get; }
        public bool IsBrowseFileEnable { set; get; }
        public bool IsReadonly { set; get; }
    }

    public class LicenseRequestFileOtherModel
    {
        public long FileId { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
    }

    public class LicenseRequestMailModel
    {
        public string Mail_MnfFormNum { get; set; }
        public string Mail_RequestDate { get; set; }
        public string Mail_Perihal { get; set; }
        public string Mail_KPPBC { get; set; }
        public string Mail_Creator { get; set; }
        public string Mail_BANo { get; set; }
        public string Mail_Company { get; set; }
        public string Mail_DecreeDate { get; set; }
        public string Mail_DecreeNo { get; set; }
        public string Mail_Comment { get; set; }
        public string Mail_Status { get; set; }
        public long Mail_MnfRequestId { get; set; }
        public string Mail_MailFor { get; set; }
        public string Mail_Approver { get; set; }
        public string Mail_ApprovedDate { get; set; }
        public string Mail_Remark { get; set; }
        public string Mail_Company_Detail { get; set; }
        public List<string> Mail_POA_ReceiverList { get; set; }
        public string Mail_Sub { get; set; }
        public string Mail_LastStatus { get; set; }
        public string Mail_Form { get; set; }
    }
    
    public class vwMLLicenseRequestModel
    {
        public string FormNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public string CompanyName { get; set; }
        public string CompanyType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string LastApprovedBy { get; set; }
        public DateTime? LastApprovedDate { get; set; }
        public long LastApprovedStatus { get; set; }

        public string LastApprovedStatusValue { get; set; }
        public string KPPBC { get; set; }
        public int MnfRequestId { get; set; }
        public string DecreeNo { get; set; }
        public DateTime? DecreeDate { get; set; }
        public bool? DecreeStatus { get; set; }
        public string NppbkcID { get; set; }
        public string ManufactureAddress { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string SubDistrict { get; set; }
        public string Village { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public string North { get; set; }
        public string East { get; set; }
        public string South { get; set; }
        public string West { get; set; }
        public string LandArea { get; set; }
        public string BuildingArea { get; set; }
        public string OwnershipStatus { get; set; }

      
    }

}