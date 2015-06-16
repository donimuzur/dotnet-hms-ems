namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class BLLBaseOutput
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; } // ok or error code
        public string ErrorMessage { get; set; } // error description
    }
}
