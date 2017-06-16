using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration
{
    public class BrandRegistrationReqModel : BaseModel
    {
        public BrandRegistrationReqModel() : base()
        {
            BrandRegFileOtherList = new List<BrandRegFileOtherModel>();
            SKEPFileList = new List<BrandRegFileOtherModel>();
            UserAccess = new BrandAccess();
            DocExport = false;

        }
        public long Registration_ID { get; set; }
        public string Registration_No { get; set; }
        public DateTime? Submission_Date { get; set; }
        public string strSubmission_Date { get; set; }
        public int Registration_Type { get; set; }
        public string strRegistration_Type { get; set; }
        public string Nppbkc_ID { get; set; }
        public DateTime Effective_Date { get; set; }
        public string strEffective_Date { get; set; }
        public string Created_By { get; set; }
        public string Created_By_Name { get; set; }
        public string Created_By_Email { get; set; }
        public DateTime Created_Date { get; set; }
        public string LastModified_By { get; set; }
        public DateTime LastModified_Date { get; set; }
        public string LastApproved_By { get; set; }
        public DateTime? LastApproved_Date { get; set; }
        public long LastApproved_Status { get; set; }
        public long NextStatus { get; set; }
        public string strLastApproved_Status { get; set; }
        public string Status { get; set; }
        public long PD_ID { get; set; }
        public bool Decree_Status { get; set; }
        public string Decree_No { get; set; }
        public DateTime? Decree_Date { get; set; }
        public DateTime? Decree_StartDate { get; set; }

        public CompanyModel Company { set; get; }
        public string CompanyName { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyAddress { get; set; }
        public UserModel Creator { set; get; }
        public UserModel Approver { set; get; }
        public UserModel LastEditor { set; get; }
        public ReferenceModel ApprovalStatusDescription { set; get; }

        public bool IsCreator { set; get; }
        public bool IsSubmitted { set; get; }
        public bool IsApproved { set; get; }
        public Shared.WorkflowHistory RevisionData { set; get; }

        public bool IsFormReadOnly { get; set; }
        public bool IsDetail { get; set; }
        public bool IsFormSkep { get; set; }
        public bool IsApprover { get; set; }
        public bool IsViewer { get; set; }
        public Enums.UserRole CurrentRole { get; set; }

        public List<BrandRegFileOtherModel> BrandRegFileOtherList { get; set; }
        public List<BrandRegFileOtherModel> SKEPFileList { get; set; }

        public string Text_To { get; set; }

        public BrandAccess UserAccess { get; set; }
        public bool DocExport { get; set; }
    }

    public class BrandRegSupportingDocumentModel
    {
        public BrandRegSupportingDocumentModel()
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


    public class BrandRegFileOtherModel
    {
        public long FileId { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }

    }


    public class vwBrandRegistrationModel
    {
        public string RegistrationNo { get; set; }
        public string SubmissionDate { get; set; }
        public string RegistrationType { get; set; }
        public string NPPBKCId { get; set; }
        public string CompanyName { get; set; }
        public string EffectiveDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public string ModifyDate { get; set; }
        public string LastApprovedBy { get; set; }
        public string LastApprovedDate { get; set; }
        public long LastApprovedStatus { get; set; }

        public string LastApprovedStatusValue { get; set; }
        public bool? DecreeStatus { get; set; }
        public string DecreeNo { get; set; }
        public string DecreeDate { get; set; }
        public string DecreeStartDate { get; set; }
        public string BrandName { get; set; }
        public string ProductType { get; set; }
        public string CompanyTier { get; set; }
        public decimal HJE { get; set; }
        public string Unit { get; set; }
        public string BrandContent { get; set; }
        public decimal Tarif { get; set; }

        public string MaterialPackage { get; set; }
        public string MarketDesc { get; set; }
        public string FrontSide { get; set; }
        public string BackSide { get; set; }
        public string LeftSide { get; set; }
        public string RightSide { get; set; }
        public string TopSide { get; set; }
        public string BottomSide { get; set; }


    }

    public class BRProductDevDetailModel
    {
        public long PD_DETAIL_ID { get; set; }
        public string Fa_Code_Old { get; set; }
        public string Fa_Code_New { get; set; }
        public string Hl_Code { get; set; }
        public string Market_Id { get; set; }
        public string Fa_Code_Old_Desc { get; set; }
        public string Fa_Code_New_Desc { get; set; }
        public string Werks { get; set; }
        public bool Is_Import { get; set; }
        public long PD_ID { get; set; }
        public string Request_No { get; set; }
        public string Bukrs { get; set; }
        public string Approved_By { get; set; }
        public DateTime? Approved_Date { get; set; }
        public long Approval_Status { get; set; }
        public UserModel Approver { set; get; }
        public ReferenceModel ApprovalStatusDescription { set; get; }

        public MarketModel Market { get; set; }
        public CompanyModel Company { set; get; }

        public bool IsSubmitted { set; get; }
        public bool IsApproved { set; get; }
        public Shared.WorkflowHistory RevisionData { set; get; }
        public string Modified_By { get; set; }
        public DateTime? Modified_Date { get; set; }

        public int ProdDevNextAction { get; set; }

        public string ProductionCenter { get; set; }

        public decimal HJE { get; set; }
        public decimal HJEperBatang { get; set; }
        public string Unit { get; set; }
        public int BrandContent { get; set; }
        public decimal Tariff { get; set; }
        public int CompanyTier { get; set; }
        public string PackagingMaterial { get; set; }
        public string BrandName { get; set; }
        public string ExciseGoodType { get; set; }
        public string FrontSide { get; set; }
        public string BackSide { get; set; }
        public string LeftSide { get; set; }
        public string RightSide { get; set; }
        public string TopSide { get; set; }
        public string BottomSide { get; set; }

    }

    public class vwProductDevDetailModel
    {
        public string PDNo { get; set; }
        public int NextAction { get; set; }
        public int PDDetailId { get; set; }
        public string FACodeOld { get; set; }
        public string FACodeNew { get; set; }
        public string HLCode { get; set; }
        public string MarketId { get; set; }
        public string MarketDesc { get; set; }
        public string FACodeOldDesc { get; set; }
        public string FACodeNewDesc { get; set; }
        public string WERKS { get; set; }
        public string ProductionCenter { get; set; }
        public bool IsImport { get; set; }
        public int PDId { get; set; }
        public string RequestNo { get; set; }
        public string BUKRS { get; set; }
        public string CompanyName { get; set; }

        public string LastApproved_By { get; set; }
        public DateTime? LastApproved_Date { get; set; }
        public long LastApproved_Status { get; set; }
        public string LastModified_By { get; set; }

    }

}