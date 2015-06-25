namespace Sampoerna.EMS.BusinessObject.Business
{
    public class HeaderFooterMap
    {
        public int HEADER_FOOTER_FORM_MAP_ID { get; set; }
        public Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public bool? IS_HEADER_SET { get; set; }
        public bool? IS_FOOTER_SET { get; set; }
        public int HEADER_FOOTER_ID { get; set; }
    }
}
