namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1GenerateInputModel
    {
        public int Lack1Id { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivedPlantId { get; set; }
        //public DateTime SubmissionDate { get; set; }
        public string SupplierPlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }
        public decimal? WasteAmount { get; set; }
        public string WasteAmountUom { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string ReturnAmountUom { get; set; }

        public int Lack1Level { get; set; }

        public string Noted { get; set; }

        public bool IsCreateNew { get; set; }
    }
}