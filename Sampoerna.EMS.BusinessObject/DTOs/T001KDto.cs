using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class T001KDto
    {
        public string BWKEY { get; set; }
        public string BUKRS { get; set; }
        public string BUTXT { get; set; }
        public string NPWP { get; set; }
        public string ORT01 { get; set; }
        public string SPRAS { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public bool? IS_DELETED { get; set; }
        public string NPPBKC_ID { get; set; }
        public string NPPBKC_KPPBC_ID { get; set; }
    }
}
