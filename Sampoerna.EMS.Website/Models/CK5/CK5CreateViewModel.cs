using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5CreateViewModel : BaseModel
    {
        public CK5CreateViewModel()
        {
            
            UploadItemModels = new List<CK5UploadViewModel>();
        }

       
        public List<CK5UploadViewModel> UploadItemModels { get; set; }
       
        public Enums.DocumentStatus DocumentStatus { get; set; }

        //DETAIL INFORMATION
        public Enums.CK5Type Ck5Type { get; set; }

        [Required(ErrorMessage = "KPPBC City field is required")]
        public int KppBcCity { get; set; }
        public SelectList KppBcCityList { get; set; }
        public string CeOfficeCode { get; set; }

        public string SubmissionNumber { get; set; }

        [UIHint("FormatDateTime")]
        public DateTime? SubmissionDate { get; set; }

        public string RegistrationNumber { get; set; }

        [UIHint("FormatDateTime")]
        public DateTime? RegistrationDate { get; set; }

        public int GoodTypeId { get; set; }
        public SelectList GoodTypeList { get; set; }

        public int ExciseSettlement { get; set; }
        public SelectList ExciseSettlementList { get; set; }

        public int ExciseStatus { get; set; }
        public SelectList ExciseStatusList { get; set; }

        public int RequestType { get; set; }
        public SelectList RequestTypeList { get; set; }


        //ORIGIN PLANT
        [Required(ErrorMessage = "Origin Plant field is required")]
        public int SourcePlantId { get; set; }
        public SelectList SourcePlantList { get; set; }
        public string SourceNpwp { get; set; }
        public string SourceNppbkcId { get; set; }
        public string SourceCompanyName { get; set; }
        public string SourceAddress { get; set; }
        public string SourceKppbcName { get; set; }

        //DESTINATION PLANT
        [Required(ErrorMessage = "Destination Plant field is required")]
        public int DestPlantId { get; set; }
        public SelectList DestPlantList { get; set; }
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
        public DateTime? PbckDecreeDate { get; set; }

        public int? CarriageMethod { get; set; }
        public SelectList CarriageMethodList { get; set; }

        [Display(Name = "Grand Total Exciseable")]
        public decimal GrandTotalEx { get; set; }

        public int? PackageUomId { get; set; }
        public SelectList PackageUomList { get; set; }

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


        

    }
}