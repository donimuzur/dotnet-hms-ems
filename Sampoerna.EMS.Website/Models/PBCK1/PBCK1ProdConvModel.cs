namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1ProdConvModel
    {
        public long Pbck1ProdConvId { get; set; }
        public long Pbck1Id { get; set; }
        public string ProductCode { get; set; }
        public string ProdTypeName { get; set; }
        public string ProdTypeAlias { get; set; }
        public string ConverterOutput { get; set; }
        public string ConverterUomId { get; set; }
        public string ConverterUom { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
}