namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class HEADER_FOOTER_MAPDto
    {
        public int HEADER_FOOTER_FORM_MAP_ID { get; set; }
        public Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public bool? IS_HEADER_SET { get; set; }
        public bool? IS_FOOTER_SET { get; set; }
        public int HEADER_FOOTER_ID { get; set; }

        public string HEADER_IMAGE_PATH { get; set; }
        public string FOOTER_CONTENT { get; set; }

    }
}
