using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class XML_LOGS_DETAILSDto
    {
        public long XML_LOGS_DETAILS_ID { get; set; }
        public long XML_LOGS_ID { get; set; }
        public string XML_FILENAME { get; set; }
        public string LOGS { get; set; }
        public DateTime? ERROR_TIME { get; set; }
    }
}
