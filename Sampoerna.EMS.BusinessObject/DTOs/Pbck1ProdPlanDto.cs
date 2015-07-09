namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1ProdPlanDto
    {
        public long Pbck1ProdPlanId { get; set; }
        public long Pbck1Id { get; set; }
        public int ProdTypeId { get; set; }
        public int? ProdTypeCode { get; set; }
        public string ProdTypeName { get; set; }
        public string ProdAlias { get; set; }
        public decimal? Amount { get; set; }
        public string BkcRequired { get; set; }
        public int MonthId { get; set; }
        public string MonthName { get; set; }
        
    }
}