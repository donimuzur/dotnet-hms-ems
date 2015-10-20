namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class Lack2SaveEditOutput : BLLBaseOutput
    {
        public int Id { get; set; }
        public string Lack2Number { get; set; }
        public bool IsModifiedHistory { get; set; }
    }
}
