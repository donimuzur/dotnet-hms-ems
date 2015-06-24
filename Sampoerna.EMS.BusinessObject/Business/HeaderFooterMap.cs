
namespace Sampoerna.EMS.BusinessObject.Business
{
    public class HeaderFooterMap
    {
        public int HEADER_FOOTER_FORM_MAP_ID { get; set; }
        public int? FORM_TYPE_ID { get; set; }
        public bool? IS_HEADER_SET { get; set; }
        public bool? IS_FOOTER_SET { get; set; }
        public long COMPANY_ID { get; set; }
    }
}
