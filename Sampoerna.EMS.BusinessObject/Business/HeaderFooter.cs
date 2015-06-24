using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Business
{

    public class HeaderFooter
    {
        public int HEADER_FOOTER_ID { get; set; }
        public string FORM_NAME { get; set; }
        public long? COMPANY_ID { get; set; }
        public string HEADER_IMAGE_PATH { get; set; }
        public string FOOTER_CONTENT { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public T1001 T1001 { get; set; }

    }

    public class HeaderFooterDetails
    {
        public HeaderFooter Detail { get; set; }
        public List<HeaderFooterMap> HeaderFooterList { get; set; }
    }
    
}
