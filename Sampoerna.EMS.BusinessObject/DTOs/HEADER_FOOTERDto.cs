using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class HEADER_FOOTERDto
    {
        public int HEADER_FOOTER_ID { get; set; }
        public string FORM_NAME { get; set; }
        public string COMPANY_ID { get; set; }
        public string HEADER_IMAGE_PATH { get; set; }
        public string FOOTER_CONTENT { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_NPWP { get; set; }
        public bool? IS_DELETED { get; set; }
    }
}
