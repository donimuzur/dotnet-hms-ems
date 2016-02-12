using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class POA_DELEGATIONDto
    {
        public int POA_DELEGATION_ID { get; set; }
        public string POA_FROM { get; set; }
        public string POA_TO { get; set; }
        public System.DateTime DATE_FROM { get; set; }
        public System.DateTime DATE_TO { get; set; }
        public string REASON { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
    }
}
