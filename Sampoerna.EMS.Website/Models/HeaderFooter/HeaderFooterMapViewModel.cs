using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.HeaderFooter
{
    public class HeaderFooterMapViewModel : BaseModel
    {
    }

    public class HeaderFooterMapItem
    {
        public int HEADER_FOOTER_FORM_MAP_ID { get; set; }
        public Enums.FormType FORM_TYPE_ID { get; set; }
        public string FORM_TYPE_DESC { get; set; }
        public bool IS_HEADER_SET { get; set; }
        public bool IS_FOOTER_SET { get; set; }
        public int HEADER_FOOTER_ID { get; set; }
    }

}