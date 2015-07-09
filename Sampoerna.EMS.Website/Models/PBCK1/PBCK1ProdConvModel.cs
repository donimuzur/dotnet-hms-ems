namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1ProdConvModel
    {
        public int Pbck1Id { get; set; }
        public int? ProductCode { get; set; }
        public string ProductTypeAlias { get; set; }
        public string ProductType { get; set; }
        public decimal? ConverterOutput { get; set; }
        public string ConverterUom { get; set; }
    }
}