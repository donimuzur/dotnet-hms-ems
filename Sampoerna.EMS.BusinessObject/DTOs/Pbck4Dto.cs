using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck4Dto
    {
        public int PBCK4_ID { get; set; }
        public string PBCK4_NUMBER { get; set; }
        //public string PLANT_ID { get; set; }
        //public string PLANT_NAME { get; set; }
        //public string NPPBKC_ID { get; set; }
        public string NPPBKC_DESCRIPTION { get; set; }
        public string COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        //public Enums.DocumentStatus STATUS { get; set; }
        public Enums.DocumentStatusGov? GOV_STATUS { get; set; }
        //public DateTime? REPORTED_ON { get; set; }
        public string BACK1_NO { get; set; }
        public Nullable<System.DateTime> BACK1_DATE { get; set; }
        public string CK3_NO { get; set; }
        public Nullable<System.DateTime> CK3_DATE { get; set; }
        public string CK3_OFFICE_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string APPROVED_BY_POA { get; set; }
        public Nullable<System.DateTime> APPROVED_BY_POA_DATE { get; set; }
        public string REJECTED_BY { get; set; }
        public Nullable<System.DateTime> REJECTED_DATE { get; set; }
        public string APPROVED_BY_MANAGER { get; set; }
        public Nullable<System.DateTime> APPROVED_BY_MANAGER_DATE { get; set; }

        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string NppbkcId { get; set; }
        public DateTime? ReportedOn { get; set; }
        public string Poa { get; set; }
        public Enums.DocumentStatus Status { get; set; }

        public bool IsWaitingGovApproval { get; set; }

        public List<PBCK4_DOCUMENTDto> Pbck4DocumentDtos { get; set; }
    }
}
