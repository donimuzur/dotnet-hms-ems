namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class XML_LOGSDto
    {
        public long XML_LOGS_ID { get; set; }
        public string XML_FILENAME { get; set; }
        public System.DateTime LAST_ERROR_TIME { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public byte[] MODIFIED_DATE { get; set; }
        public Core.Enums.XmlLogStatus STATUS { get; set; }
    }
}
