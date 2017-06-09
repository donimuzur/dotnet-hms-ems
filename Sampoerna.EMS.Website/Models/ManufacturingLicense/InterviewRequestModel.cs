using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ManufacturingLicense
{
    public class InterviewRequestModel : BaseModel
    {
        public InterviewRequestModel()
        {
            File_Other_Path = new List<string>();
            File_BA_Path = new List<string>();
            File_BA_Path_Plus = new List<InterviewRequestFileOtherModel>();
            interviewRequestDetail = new List<InterviewRequestDetailModel>();
            CompanyList = new List<InterviewRequestCompanyModel>();
            RemovedFilesId = new List<long>();
            InterviewRequestFileOtherList = new List<InterviewRequestFileOtherModel>();
            Confirmation = new List<ConfirmDialogModel>();
            RemovedDetailId = new List<long>();
            ManufactureID = "";
            ManufactureNO = "";
            ManufactureSTATUS = "";
        }
        public long Id { get; set; }

        //[Required]
        public DateTime RequestDate { get; set; }
        public string StrRequestDate { get; set; }
        public DateTime SubmissionDate { set; get; }
        public string Status { get; set; }
        public string StatusKey { get; set; }
        public string ApprovalStatus { get; set; }
        public string FormNumber { get; set; }

        //[Required]
        public string Perihal { get; set; }
        public string Company_Type { get; set; }
        public bool? BAStatus { get; set; }
        public string BANumber { get; set; }
        public DateTime? BADate { get; set; }
        public string POAID { get; set; }
        public string POAName { get; set; }
        public string POAPosition { get; set; }
        public string POAAddress { get; set; }
        public string NPPBKC_ID { get; set; }

        //[Required]
        public string KPPBC_ID { get; set; }

        //[Required]
        public string KPPBC_Address { get; set; }

        //[Required]
        public string Company_ID { get; set; }
        public string Company_Name { get; set; }
        public string Company_Address { get; set; }
        public string Npwp { get; set; }
        public Int32 Index { get; set; }
        public long File_Size { get; set; }
        public Int32 Count_Lamp { get; set; }  
        public bool Disabled { get; set; }
        public bool IsFormReadOnly { get; set; }
        public bool IsDetail { get; set; }
        public bool IsFormSkep { get; set; }
        public bool IsCanEdit { get; set; }
        public bool IsApprover { get; set; }
        public bool IsCanEditPrintout { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastApprovedBy { get; set; }
        public string City { get; set; }
        public string City_Alias { get; set; }
        public string Text_To { get; set; }
        public string ManufactureNO { get; set; }
        public string ManufactureID { get; set; }
        public string ManufactureSTATUS { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<InterviewRequestCompanyModel> CompanyList { get; set; }
        public SelectList Company_TypeList { get; set; }
        public SelectList Perihal_List { get; set; }
        public SelectList GovStatus_List { get; set; }
        public List<CityModel> City_List { get; set; }
        public List<HttpPostedFileBase> File_Other { get; set; }
        public List<string> File_Other_Name { get; set; }
        public List<string> File_Other_Path { set; get; }
        public List<HttpPostedFileBase> File_BA { get; set; }
        public List<string> File_BA_Path { set; get; }
        public List<InterviewRequestFileOtherModel> File_BA_Path_Plus { set; get; }
        public List<long> RemovedFilesId { set; get; }
        public List<long> RemovedDetailId { set; get; }
        public List<InterviewRequestDetailModel> interviewRequestDetail { get; set; }
        public List<InterviewRequestFileOtherModel> InterviewRequestFileOtherList { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public IEnumerable<InterviewRequestSupportingDocModel> interviewRequestSupportingDoc { get; set; }
        public List<ConfirmDialogModel> Confirmation { set; get; }
    }

    public class InterviewRequestDetailModel : BaseModel
    {
        public long DetId { get; set; }
        public string Manufacture_Address { get; set; }
        public long City_Id { get; set; }
        public string City_Name { get; set; }
        public long Province_Id { get; set; }
        public string Province_Name { get; set; }
        public string Sub_District { get; set; }
        public string Village { get; set; }
        public string Phone_Area_Code { get; set; }
        public string Phone { get; set; }
        public string Fax_Area_Code { get; set; }
        public string Fax { get; set; }
        public Int32 Index { get; set; }
        public bool IsFormReadOnly { get; set; }
        public List<CityModel> City_List { get; set; }
    }

    public class InterviewRequestSupportingDocModel
    {
        public InterviewRequestSupportingDocModel()
        {
            
        }
        public long Id { set; get; }
        public long? DocId { set; get; }
        public long FileUploadId { set; get; }
        public string Name { set; get; }
        public string FileName { set; get; }
        public CompanyModel Company { set; get; }
        public HttpPostedFileBase File { set; get; }
        public string Path { set; get; }
        public bool IsBrowseFileEnable { set; get; }
        public bool IsReadonly { set; get; }
    }

    public class InterviewRequestCompanyModel : BaseModel
    {
        public string Company_ID { get; set; }
        public string Company_Name { get; set; }
        public string Company_Address { get; set; }
        public string Npwp { get; set; }
    }

    public class InterviewRequestFileOtherModel
    {
        public long FileId { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
    }

    public class ManufacturingLicensePOAModel
    {
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public string User_Mail { get; set; }
    }

    public class vwMLInterviewRequestModel
    {
        public long FormId { get; set; }
        public string StrRequestDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Perihal { get; set; }
        public string Company_Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifyBy { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public string LastApprovedBy { get; set; }
        public DateTime? LastApprovedDate { get; set; }
        public long LastApprovedStatus { get; set; }
        public string ApprovalStatus { get; set; }
        public string FormNumber { get; set; }
        public string BANumber { get; set; }
        public DateTime? BADate { get; set; }
        public string NppbkcID { get; set; }
        public string Company_Name { get; set; }
        public string Company_Address { get; set; }
        public string Npwp { get; set; }

        public string BAStatus { get; set; }
        public string POAID { get; set; }
        public string POAName { get; set; }
        public string POAPosition { get; set; }
        public string POAAddress { get; set; }

        public string KPPBC_ID { get; set; }
        public string KPPBC_Address { get; set; }

        public string ManufactureAddress { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string SubDistrict { get; set; }
        public string Village { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }


}