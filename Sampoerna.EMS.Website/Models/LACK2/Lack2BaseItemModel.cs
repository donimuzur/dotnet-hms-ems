namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2BaseItemModel : BaseModel
    {
        public int Lack2Id { get; set; }
        public string Lack2Number { get; set; }
        public int? PeriodMonth { get; set; }
        public string PeriodMonthNameEn { get; set; }
        public string PeriodMonthNameId { get; set; }
        public int? PeriodYear { get; set; }
        public string SourcePlantId { get; set; }
        public string SourcePlantName { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string UserId { get; set; }
        public bool AllowPrintDocument { get; set; }
    }
}