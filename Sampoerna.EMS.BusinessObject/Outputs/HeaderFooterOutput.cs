namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class SaveHeaderFooterOutput : BLLBaseOutput
    {
        public int HeaderFooterId { get; set; }

        public string MessageExist { get; set; }
    }
}
