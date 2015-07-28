namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1ProdPlanModel
    {
        public long Pbck1ProdPlanId { get; set; }
        public long Pbck1Id { get; set; }
        public string ProductCode { get; set; }
        public string ProdTypeName { get; set; }
        public string ProdTypeAlias { get; set; }
        public string Amount { get; set; }
        public string BkcRequired { get; set; }
        public string BkcRequiredUomId { get; set; }
        public string BkcRequiredUomName { get; set; }
        public string Month { get; set; }
        public string MonthName { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
}