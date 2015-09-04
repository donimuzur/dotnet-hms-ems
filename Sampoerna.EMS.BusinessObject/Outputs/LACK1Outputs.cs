namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class SaveLack1Output : BLLBaseOutput
    {
        public long Id { get; set; }
        public string Lack1Number { get; set; }
    }

    public class Lack1CreateOutput : BLLBaseOutput
    {
        public long? Id { get; set; }
        public string Lack1Number { get; set; }
    }

}
