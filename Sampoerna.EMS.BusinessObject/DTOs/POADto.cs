using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class POADto
    {
        public string POA_ID { get; set; }
        public string ID_CARD { get; set; }
        public string POA_PHONE { get; set; }
        public string POA_ADDRESS { get; set; }
        public string POA_EMAIL { get; set; }
        public string PRINTED_NAME { get; set; }
        public string TITLE { get; set; }
        public string LOGIN_AS { get; set; }
        public string MANAGER_ID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<DateTime> MODIFIED_DATE { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
    }
}
