using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Validations;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5FormViewModel : BaseModel
    {
        public CK5FormViewModel()
        {
            UploadItemModels = new List<CK5UploadViewModel>();
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
        }

        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }

        public long Ck5Id { get; set; }

        public List<CK5UploadViewModel> UploadItemModels { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }

        public Enums.DocumentStatus DocumentStatus { get; set; }
        public string DocumentStatusDescription { get; set; }
        //DETAIL INFORMATION
        public Enums.CK5Type Ck5Type { get; set; }

       
        //public SelectList KppBcCityList { get; set; }
        //[Required(ErrorMessage = "KPPBC City field is required")]
        public string KppBcCity { get; set; }

         [Display(Name = "KPPBC code")]
        public string CeOfficeCode { get; set; }

        //public string KppBcCityName { get; set; }
        
        public string SubmissionNumber { get; set; }

        [UIHint("DateTime")]
        public DateTime? SubmissionDate { get; set; }

        //[RequiredIf("DocumentStatus", Enums.DocumentStatus.WaitingGovApproval), Display(Name = "RegistrationNumber")]
        [Display(Name = "RegistrationNumber")]
        public string RegistrationNumber { get; set; }

       // [RequiredIf("DocumentStatus", Enums.DocumentStatus.WaitingGovApproval), Display(Name = "RegistrationDate")]
        [Display(Name = "RegistrationDate")]
        [UIHint("DateTime")]
        public DateTime? RegistrationDate { get; set; }

        public Enums.ExGoodsType GoodType { get; set; }
        public Enums.ExGoodsType GoodTypeList { get; set; }
      
        public string GoodTypeName { get; set; }


        [Required]
        public Enums.ExciseSettlement ExciseSettlement { get; set; }
        public Enums.ExciseSettlement ExciseSettlementList { get; set; }
        public string ExciseSettlementDesc { get; set; }

        [Required]
        public Enums.ExciseStatus ExciseStatus { get; set; }
        public Enums.ExciseStatus ExciseStatusList { get; set; }
        public string ExciseStatusDesc { get; set; }

        //[Required]
        //public int? RequestTypeId { get; set; }
        public Enums.RequestType RequestType { get; set; }
        public Enums.RequestType RequestTypeList { get; set; }
        public string RequestTypeDesc { get; set; }

        //ORIGIN PLANT
        //[Required(ErrorMessage = "Origin Plant field is required")]
        public string SourcePlantId { get; set; }
        public SelectList SourcePlantList { get; set; }
        public string SourcePlantName { get; set; }
        public string SourceNpwp { get; set; }
        public string SourceNppbkcId { get; set; }
        public string SourceCompanyCode { get; set; }
        public string SourceCompanyName { get; set; }
        public string SourceAddress { get; set; }
        public string SourceKppbcName { get; set; }

        //DESTINATION PLANT
        //[Required(ErrorMessage = "Destination Plant field is required")]
        public string DestPlantId { get; set; }
        public SelectList DestPlantList { get; set; }
        public string DestPlantName { get; set; }
        public string DestNpwp { get; set; }
        public string DestNppbkcId { get; set; }
        public string DestCompanyCode { get; set; }
        public string DestCompanyName { get; set; }
        public string DestAddress { get; set; }
        public string DestKppbcName { get; set; }

        [RequiredIf("Ck5Type", Enums.CK5Type.Export), Display(Name = "Loading Port")]
        [StringLength(50, ErrorMessage = "Max Length : 50")]
        public string LoadingPort { get; set; }

        [RequiredIf("Ck5Type", Enums.CK5Type.Export), Display(Name = "Loading Port Name")]
        [StringLength(50, ErrorMessage = "Max Length : 50")]
        public string LoadingPortName { get; set; }

        [RequiredIf("Ck5Type", Enums.CK5Type.Export), Display(Name = "Loading Port Id")]
        [StringLength(10, ErrorMessage = "Max Length : 10")]
        public string LoadingPortId { get; set; }

        [RequiredIf("Ck5Type", Enums.CK5Type.Export), Display(Name = "Final Port")]
        [StringLength(50, ErrorMessage = "Max Length : 50")]
        public string FinalPort { get; set; }

        [RequiredIf("Ck5Type", Enums.CK5Type.Export), Display(Name = "Final Port Name")]
        [StringLength(50, ErrorMessage = "Max Length : 50")]
        public string FinalPortName { get; set; }

        [RequiredIf("Ck5Type", Enums.CK5Type.Export), Display(Name = "Final Port Id")]
        [StringLength(10, ErrorMessage = "Max Length : 10")]
        public string FinalPortId { get; set; }


        //NOTIF DATA

        [StringLength(50, ErrorMessage = "Max Length : 50")]
        public string InvoiceNumber { get; set; }
       [UIHint("DateTime")]
        public DateTime? InvoiceDate { get; set; }

        public int? PbckDecreeId { get; set; }
        //public string PbckDecreeId { get; set; }
        public SelectList PbckDecreeList { get; set; }
        public string PbckDecreeNumber { get; set; }
        public string PbckUom { get; set; }

        [UIHint("DateTime")]
        public DateTime? PbckDecreeDate { get; set; }

        //public int? CarriageMethodId { get; set; }
        public Enums.CarriageMethod? CarriageMethod { get; set; }
        public Enums.CarriageMethod CarriageMethodList { get; set; }
        public string CarriageMethodDesc { get; set; }

        [Display(Name = "Grand Total Exciseable")]
        public decimal GrandTotalEx { get; set; }

        //public int? PackageUomId { get; set; }
        public SelectList PackageUomList { get; set; }
        public string PackageUomName { get; set; }

        //additional information
        [Display(Name = "DN Number")]
        public string DnNumber { get; set; }

         [UIHint("DateTime")]
        [Display(Name = "DN Date")]
        public string DnDate { get; set; }

        [Display(Name = "STO Sender Number")]
        public string StoSenderNumber { get; set; }

        [Display(Name = "STO Receiver Number")]
        public string StoReceiverNumber { get; set; }

        [Display(Name = "STOB Number")]
        public string StobNumber { get; set; }

        [UIHint("DateTime")]
        [Display(Name = "GI Date")]
        public DateTime? GiDate { get; set; }

        [UIHint("DateTime")]
        [Display(Name = "GR Date")]
        public DateTime? GrDate { get; set; }

        [Display(Name = "Sealing Notification Number")]
        public string SealingNotifNumber { get; set; }

       
        [Display(Name = "Sealing Notification Date")]
        [UIHint("DateTime")]
        public DateTime? SealingNotifDate { get; set; }

        //GRDate -- todo ask

        [Display(Name = "Unsealing Notification Number")]
        public string UnSealingNotifNumber { get; set; }

         [UIHint("DateTime")]
        [Display(Name = "Unsealing Notification Date")]
        public DateTime? UnsealingNotifDate { get; set; }

        public bool IsCk5Export { get; set; }
        public bool IsCk5PortToImporter { get; set; }
        public bool IsCk5Manual { get; set; }
        public bool IsWaitingGovApproval { get; set; }

        public List<HttpPostedFileBase> Ck5FileUploadFileList { get; set; }
        public List<CK5FileUploadViewModel> Ck5FileUploadModelList { get; set; }

        public bool IsAllowPrint { get; set; }

       [RequiredIf("Ck5Type", Enums.CK5Type.Export), Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        public SelectList CountryCodeList { get; set; }
        public string CountryName { get; set; }
        public string DisplayDetailsDestinationCountry { get; set; }

        public string Ck5TypeString { get; set; }
        public string MesssageUploadFileDocuments { get; set; }

         [RequiredIf("Status", Enums.DocumentStatus.WaitingGovApproval), Display(Name = "Status Gov")]
        public Enums.CK5GovStatus GovStatus { get; set; }
        public string GovStatusDesc { get; set; }
        public Enums.CK5GovStatus GovStatusList { get; set; }
        public string CommentGov { get; set; }

        public string Pbck1QtyApproved { get; set; }
        public string Ck5TotalExciseable { get; set; }
        public string RemainQuota { get; set; }

        public bool AllowManagerReject { get; set; }

        public string Command { get; set; }

        public bool AllowGiCreated { get; set; }

        public bool AllowGrCreated { get; set; }

        public bool AllowTfPostedPortToImporter { get; set; }

        public string ActionType { get; set; }

        public bool AllowCancelSAP { get; set; }

        public decimal MaterialQty { get; set; }

        public SelectList PackageConvertedUomList { get; set; }

        public Enums.Ck5ManualType Ck5ManualTypeList { get; set; }
        public Enums.Ck5ManualType Ck5ManualType { get; set; }

        public string Ck5ManualTypeString { get; set; }

        public long Ck5RefId { get; set; }

        public string Ck5RefNumber { get; set; }
        public SelectList Ck5RefList { get; set; }
        public bool IsCk5ImporterToPlant { get; set; }
        public bool IsMarketReturn { get; set; }
        public bool IsCompleted { get; set; }

        [Display(Name = "BACK-1 Number")]
        public string Back1Number { get; set; }

        [Display(Name = "BACK-1 Date")]
        [UIHint("DateTime")]
        public DateTime? Back1Date { get; set; }

        public bool AllowAttachmentCompleted { get; set; }
    }
}