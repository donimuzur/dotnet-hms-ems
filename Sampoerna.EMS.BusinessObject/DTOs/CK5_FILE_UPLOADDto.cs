namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class CK5_FILE_UPLOADDto
    {
        public long CK5_FILE_UPLOAD_ID { get; set; }
        public long CK5_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    }
}
