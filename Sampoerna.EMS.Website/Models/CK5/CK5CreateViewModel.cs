using System;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5CreateViewModel : BaseModel
    {
        
        public Enums.CK5Type Ck5Type { get; set; }

        public int KppBcCity { get; set; }

        public SelectList KppBcCityList { get; set; }

        public string SubmissionNumber { get; set; }
        public DateTime SubmissionDate { get; set; }

        public string RegistrationNumber { get; set; }
        public DateTime RegistrationDate { get; set; }

        public int GoodTypeId { get; set; }
        public SelectList GoodTypeList { get; set; }

        public int ExciseSettlement { get; set; }
        public SelectList ExciseSettlementList { get; set; }

        public int ExciseStatus { get; set; }
        public SelectList ExciseStatusList { get; set; }

        public int RequestType { get; set; }
        public SelectList RequestTypeList { get; set; }

        //[STO_SENDER_NUMBER]
        //[STO_RECEIVER_NUMBER]
        //[STOB_NUMBER]

        public int SourcePlantId { get; set; }
        public int DestPlantId { get; set; }

        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDateTime { get; set; }

        public int PbckDecreeId { get; set; }

        public int CarriageMethod { get; set; }
        public SelectList CarriageMethodList { get; set; }

        public decimal GrandTotalEx { get; set; }

        //[PACKAGE_UOM_ID]
        //[DEST_COUNTRY_ID]
        //[HARBOUR]
        //[OFFICE_HARBOUR]
        //[LAST_SHELTER_HARBOUR]
        //[OFFICE_SHELTER]

        public string DnNumber { get; set; }
        public string DnDate { get; set; } //..todo ask?
        public DateTime GiDate { get; set; }

        public string SealingNotifNumber { get; set; }
        public DateTime SealingNotifDate { get; set; }

        public string UnSealingNotifNumber { get; set; }
        public DateTime UnsealingNotifDate { get; set; }



        //[DN_NUMBER]
        //[GI_DATE]
        //[SEALING_NOTIF_NUMBER]
        //[UNSEALING_NOTIF_NUMBER]
        //[SEALING_NOTIF_DATE]
        //[UNSEALING_NOTIF_DATE]
        //[STATUS_ID]
        //[CREATED_BY]
        //[CREATED_DATE]
        //[APPROVED_BY]
        //[APPROVED_DATE]

    }
}