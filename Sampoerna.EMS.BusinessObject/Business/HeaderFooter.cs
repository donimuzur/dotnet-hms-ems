using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Business
{

    public class HeaderFooter
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

    public class HeaderFooterDetails : HeaderFooter
    {
        public List<HeaderFooterMap> HeaderFooterMapList { get; set; }
    }
    
}
