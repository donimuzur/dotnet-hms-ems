namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1ProdPlanModel
    {
        public int MonthId { get; set; }
        public string MonthName { get; set; }
        public string ProductCode { get; set; }
        public string ProductTypeAlias { get; set; }
        public string ProductType { get; set; }
        public decimal? Amount { get; set; }
        public string BkcRequires { get; set; }
    }
}