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

    }
}