using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class EMAIL_TEMPLATEDto
    {
        public int EMAIL_TEMPLATE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string SUBJECT { get; set; }
        public string BODY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
    }
}
