using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class ZAIDM_EX_KPPBCDto
    {
        public string KPPBC_ID { get; set; }
        public string KPPBC_TYPE { get; set; }
        public string MENGETAHUI { get; set; }

         
        public string MENGETAHUI_DETAIL { get; set; }
        public string CK1_KEP_HEADER { get; set; }
        public string CK1_KEP_FOOTER { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
    }
}
