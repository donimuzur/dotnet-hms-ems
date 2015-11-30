namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck1QuotaGetByParamInput
    {
        public string CompanyCode { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string NppbkcId { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }
    }
}
