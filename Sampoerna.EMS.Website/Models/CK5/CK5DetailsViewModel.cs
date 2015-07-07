using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5DetailsViewModel : BaseModel
    {
      

        public long Ck5Id { get; set; }

        public string DocumentStatus { get; set; }

        //DETAIL INFORMATION
        public Enums.CK5Type Ck5Type { get; set; }

        public int KppBcCityId { get; set; }
        public string KppBcCity { get; set; }
        public string CeOfficeCode { get; set; }

        public string SubmissionNumber { get; set; }

        [UIHint("FormatDateTime")]
        public DateTime? SubmissionDate { get; set; }

        public string RegistrationNumber { get; set; }

        [UIHint("FormatDateTime")]
        public DateTime? RegistrationDate { get; set; }

        public string  GoodTypeName { get; set; }
        
        public string ExciseSettlement { get; set; }
        
        public string ExciseStatus { get; set; }
        
        public string RequestType { get; set; }
        
        //ORIGIN PLANT
        public int SourcePlantId { get; set; }
        public string SourcePlantName { get; set; }
        public string SourceNpwp { get; set; }
        public string SourceNppbkcId { get; set; }
        public string SourceCompanyName { get; set; }
        public string SourceAddress { get; set; }
        public string SourceKppbcName { get; set; }

        //DESTINATION PLANT
        public int DestPlantId { get; set; }
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

        public string PbckDecreeNumber { get; set; }
        public string PbckDecreeDate { get; set; }

        public string CarriageMethod { get; set; }
        
        [Display(Name = "Grand Total Exciseable")]
        public decimal GrandTotalEx { get; set; }

        public string PackageUom { get; set; }

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