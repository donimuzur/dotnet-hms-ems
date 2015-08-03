namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class PrintHistoryDto
    {
        public long PRINT_HOSTORY_ID { get; set; }
        public Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public long FORM_ID { get; set; }
        public string FORM_NUMBER { get; set; }
        public System.DateTime PRINT_DATE { get; set; }
        public string PRINT_BY { get; set; }
    }
}
