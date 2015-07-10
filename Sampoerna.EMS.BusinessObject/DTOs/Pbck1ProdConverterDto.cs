namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1ProdConverterDto
    {
        public long Pbck1ProdConvId { get; set; }
        public long Pbck1Id { get; set; }
        public string ProdTypeCode { get; set; }
        public string ProdTypeName { get; set; }
        public string ProdAlias { get; set; }
        public decimal? ConverterOutput { get; set; }
        public string ConverterOutputUomId { get; set; }
        public string ConverterOutputUomName { get; set; }
    }
}
