using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;


namespace Sampoerna.EMS.Website.Models.ChangeRequest
{
    public class ChangeRequestModel : MasterModel
    {
        public ChangeRequestModel() : base()
        {
            File_BA_Path = new List<string>();
            File_BA_Path_Plus = new List<ChangeRequestFileOtherModel>();

            File_Other_Path = new List<string>();
            this.POA = new ChangeRequestPOA();
            this.NPPBKC = new ChangeRequestNppbkc();
            this.RequestDate = DateTime.Now;

            ChangeRequestFileOtherList = new List<ChangeRequestFileOtherModel>();
            ButtonCombination = "Create";

            //Details = new List<ChangeRequestCK1DetailModel>();
            //FinanceRatios = new List<FinanceRatioModel>();
            Confirmation = new List<ConfirmDialogModel>();
            RemovedDetailId = new List<long>();
            DetailCount = 0;
            RemovedFilesId = new List<long>();
        }
        public long Id { set; get; }
        public string DocumentNumber { set; get; }
        public DateTime RequestDate { set; get; }
        public string strRequestDate { set; get; }
        public string DocumentType { set; get; }
        public string NppbkcId { set; get; }
        public string CreatedBy { set; get; }
        public UserModel Creator { set; get; }
        public DateTime CreatedDate { set; get; }
        public string ModifiedBy { set; get; }
        public UserModel Editor { set; get; }
        public DateTime? ModifiedDate { set; get; }
        public string ApprovedBy { set; get; }
        public UserModel Approver { set; get; }
        public DateTime? ApprovedDate { set; get; }
        public bool? DecreeStatus { set; get; }
        
        public string DecreeNumber { set; get; }

        public string LastModifiedBy { set; get; }
        public DateTime LastModifiedDate { set; get; }
        public string LastApprovedBy { set; get; }
        public DateTime? LastApprovedDate { set; get; }
        public long ApprovalStatus { set; get; }

        public bool Disabled { get; set; }
        public bool IsFormReadOnly { get; set; }
        public bool IsDetail { get; set; }
        public bool IsFormSkep { get; set; }
        public bool IsApprover { get; set; }
        public bool IsCreator { get; set; }
        public bool IsViewer { get; set; }
        public Enums.UserRole CurrentRole { get; set; }

        public ChangeRequestPOA POA { set; get; }

        public string POA_Name { get; set; }
        public string POA_Role { get; set; }
        public string POA_Address { get; set; }

        public ChangeRequestNppbkc NPPBKC { set; get; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public List<ChangeRequestDetailModel> ListOfUpdateNotes { set; get; }

        public IEnumerable<ChangeRequestSupportingDocModel> changeRequestSupportingDoc { get; set; }
        public List<ChangeRequestFileOtherModel> ChangeRequestFileOtherList { get; set; }


        public List<HttpPostedFileBase> File_Other { get; set; }
        public List<string> File_Other_Path { set; get; }
        public List<string> File_Other_Name { get; set; }
        public long File_Size { get; set; }

        public List<HttpPostedFileBase> File_BA { get; set; }
        public List<string> File_BA_Path { set; get; }
        public List<ChangeRequestFileOtherModel> File_BA_Path_Plus { set; get; }

        public CompanyModel Company { set; get; }

        public string CompanyAlias { set; get; }
        public string CityAlias { set; get; }
        public string CompanyAddress { set; get; }
        public string TextTo { set; get; }
        public int Count_Lamp { set; get; }
        public string KPPBCAddress { set; get; }

        public string LastApprovedStatus { set; get; }
        public long LastApprovedStatusID { set; get; }

        public SelectList DocumentTypes { set; get; }
        public SelectList NppbkcList { set; get; }

        public List<string> UpdateNotes { set; get; }

        public string ButtonCombination { set; get; }

        public SelectList GovStatus_List { get; set; }

        public List<ConfirmDialogModel> Confirmation { set; get; }

        public List<long> RemovedDetailId { set; get; }
        public int DetailCount { get; set; }
        public List<long> RemovedFilesId { set; get; }

    }

    public class ChangeRequestSupportingDocModel
    {
        public ChangeRequestSupportingDocModel()
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


    public class ChangeRequestFileOtherModel
    {
        public long FileId { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }

    }
    public class ChangeRequestDetailModel : BaseModel
    {
        public long DetailId { get; set; }
        public long FormId { set; get; }
        public string UpdateNotes { set; get; }
        public int IsActive { get; set; }
    }


}