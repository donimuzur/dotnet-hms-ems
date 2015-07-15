using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5FormViewModel : BaseModel
    {
        public CK5FormViewModel()
        {
            UploadItemModels = new List<CK5UploadViewModel>();
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
        }

        public bool IsAllowed { get; set; }
        public string Comment { get; set; }
        public long Ck5Id { get; set; }

        public List<CK5UploadViewModel> UploadItemModels { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }

        public Enums.DocumentStatus DocumentStatus { get; set; }
        public string DocumentStatusDescription { get; set; }
        //DETAIL INFORMATION
        public Enums.CK5Type Ck5Type { get; set; }

        [Required(ErrorMessage = "KPPBC City field is required")]
        public long? KppBcCityId { get; set; }
        public SelectList KppBcCityList { get; set; }
        public string KppBcCityName { get; set; }
        public string CeOfficeCode { get; set; }

        public string SubmissionNumber { get; set; }

        [UIHint("FormatDateTime")]
        public DateTime? SubmissionDate { get; set; }

        public string RegistrationNumber { get; set; }

        [UIHint("FormatDateTime")]
        public DateTime? RegistrationDate { get; set; }

        [Required]
        public int? GoodTypeId { get; set; }
        public SelectList GoodTypeList { get; set; }
        public string GoodTypeName { get; set; }

        [Required]
        public int? ExciseSettlementId { get; set; }
        public SelectList ExciseSettlementList { get; set; }
        public string ExciseSettlementName { get; set; }

        [Required]
        public int? ExciseStatusId { get; set; }
        public SelectList ExciseStatusList { get; set; }
        public string ExciseStatusName { get; set; }

        [Required]
        public int? RequestTypeId { get; set; }
        public SelectList RequestTypeList { get; set; }
        public string RequestTypeName { get; set; }

        //ORIGIN PLANT
        [Required(ErrorMessage = "Origin Plant field is required")]
        public int? SourcePlantId { get; set; }
        public SelectList SourcePlantList { get; set; }
        public string SourcePlantName { get; set; }
        public string SourceNpwp { get; set; }
        public string SourceNppbkcId { get; set; }
        public string SourceCompanyName { get; set; }
        public string SourceAddress { get; set; }
        public string SourceKppbcName { get; set; }

        //DESTINATION PLANT
        [Required(ErrorMessage = "Destination Plant field is required")]
        public int? DestPlantId { get; set; }
        public SelectList DestPlantList { get; set; }
        public string DestPlantName { get; set; }
        public string DestNpwp { get; set; }
        public string DestNppbkcId { get; set; }
        public string DestCompanyName { get; set; }
        public string DestAddress { get; set; }
        public string DestKppbcName { get; set; }


        //NOTIF DATA

        public string InvoiceNumber { get; set; }
        [UIHint("FormatDateTime")]
        public DateTime? InvoiceDate { get; set; }

        public int? PbckDecreeId { get; set; }
        public SelectList PbckDecreeList { get; set; }
        public string PbckDecreeNumber { get; set; }

        public DateTime? PbckDecreeDate { get; set; }

        public int? CarriageMethodId { get; set; }
        public SelectList CarriageMethodList { get; set; }
        public string CarriageMethodName { get; set; }

        [Display(Name = "Grand Total Exciseable")]
        public decimal GrandTotalEx { get; set; }

        public int? PackageUomId { get; set; }
        public SelectList PackageUomList { get; set; }
        public string PackageUomName { get; set; }

        //additional information
        [Display(Name = "DN Number")]
        public string DnNumber { get; set; }

        [Display(Name = "DN Date")]
        public string DnDate { get; set; }

        [Display(Name = "STO Sender Number")]
        public string StoSenderNumber { get; set; }

        [Display(Name = "STO Receiver Number")]
        public string StoReceiverNumber { get; set; }

        [Display(Name = "STOB Number")]
        public string StobNumber { get; set; }

        [UIHint("FormatDateTime")]
        [Display(Name = "GI Date")]
        public DateTime? GiDate { get; set; }

        [Display(Name = "Sealing Notification Number")]
        public string SealingNotifNumber { get; set; }

        [UIHint("FormatDateTime")]
        [Display(Name = "Sealing Notification Date")]
        public DateTime? SealingNotifDate { get; set; }

        //GRDate -- todo ask

        [Display(Name = "Unsealing Notification Number")]
        public string UnSealingNotifNumber { get; set; }

        [UIHint("FormatDateTime")]
        [Display(Name = "Unsealing Notification Date")]
        public DateTime? UnsealingNotifDate { get; set; }


        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (UploadItemModels.Count == 0)
        //        yield return new ValidationResult("Upload ck5 item required");


        //}
    }
}