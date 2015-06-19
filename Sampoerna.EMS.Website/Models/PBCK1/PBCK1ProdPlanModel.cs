namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class PBCK1ProdPlanModel
    {
        public int MonthId { get; set; }
        public string MonthName { get; set; }
        public int? ProductCode { get; set; }
        public string ProductTypeAlias { get; set; }
        public string ProductType { get; set; }
        public decimal? Amount { get; set; }
        public string BKCRequires { get; set; }
    }
}