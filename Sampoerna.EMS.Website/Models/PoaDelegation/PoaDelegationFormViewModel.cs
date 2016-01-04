using System;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PoaDelegation
{
    public class PoaDelegationFormViewModel : BaseModel
    {
        public int POA_DELEGATION_ID { get; set; }

        public string PoaFrom { get; set; }
        public string PoaTo { get; set; }

        public SelectList ListPoaFrom { get; set; }
        public SelectList ListPoaTo { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string DateFromDisplay { get; set; }
        public string DateToDisplay { get; set; }

        public string Reason { get; set; }

    }
}