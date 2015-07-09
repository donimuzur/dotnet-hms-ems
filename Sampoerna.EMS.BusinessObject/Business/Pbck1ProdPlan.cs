
namespace Sampoerna.EMS.BusinessObject.Business
{
    public class Pbck1ProdPlan
    {
        public long Pbck1ProdPlanId { get; set; }
        public long? Pbck1Id { get; set; }
        public int? ProdTypeId { get; set; }
        public decimal? Amount { get; set; }
        public string BkcRequired { get; set; }
        public int? MonthId { get; set; }
    }
}
