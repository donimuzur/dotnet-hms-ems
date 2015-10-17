namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2GenerateInputModel
    {
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string SourcePlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public bool IsCreateNew { get; set; }
    }
}