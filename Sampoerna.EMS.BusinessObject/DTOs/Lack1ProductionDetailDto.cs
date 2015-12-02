namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1ProductionDetailDto
    {
        public long LACK1_PRODUCTION_ID { get; set; }
        public int LACK1_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public string PROD_CODE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_ALIAS { get; set; }
        public string UOM_ID { get; set; }
        public string UOM_DESC { get; set; }
        public string FA_CODE { get; set; }
        public string ORDR { get; set; }
    }

    public class Lack1ProductionSummaryByProdTypeDto
    {
        public string ProdCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public decimal TotalAmount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
    }

}
