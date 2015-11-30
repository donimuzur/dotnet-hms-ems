using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1DetailReportDto
    {
        public Lack1DetailReportDto()
        {
            TrackingConsolidations = new List<Lack1TrackingConsolidationDetailReportDto>();
        }
        public int Lack1Id { get; set; }
        public string Lack1Number { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public List<Lack1TrackingConsolidationDetailReportDto> TrackingConsolidations { get; set; }
    }

    public class Lack1TrackingConsolidationDetailReportDto
    {

        #region -------------- Receiving Table on FS Doc ------------
        public long Ck5Id { get; set; }
        public string Ck5Number { get; set; }
        public string Ck5RegistrationNumber { get; set; }
        public DateTime? Ck5RegistrationDate { get; set; }
        public DateTime? Ck5GrDate { get; set; }
        public decimal Qty { get; set; }
        #endregion

        #region ----------- Usage Table on FS Doc ---------
        public DateTime? GiDate { get; set; }
        public string PurchaseDoc { get; set; }
        public string MaterialCode { get; set; }
        public decimal? UsageQty { get; set; }
        public string OriginalUomId { get; set; }
        public string ConvertedUomId { get; set; }
        public string Batch { get; set; }
        #endregion 
        public int MaterialCodeUsageRecCount { get; set; }

    }

    public class Lack1TrackingDetailReportDto
    {
        public string MaterialId { get; set; }
        public decimal SumQty { get; set; }
        public string Batch { get; set; }
        public int RecordCount { get; set; }
    }

    public class Lack1Ck5MaterialDetailReportDto
    {
        public long Ck5Id { get; set; }
        public string Ck5Number { get; set; }
        public string Ck5RegistrationNumber { get; set; }
        public DateTime? Ck5RegistrationDate { get; set; }
        public DateTime? Ck5GrDate { get; set; }
        public decimal Qty { get; set; }
        public string UomId { get; set; }
        public string StoNumber { get; set; }
        public DateTime? GiDate { get; set; }
        public string MaterialId { get; set; }
        public decimal ConvertedQty { get; set; }
        public string ConvertedUomId { get; set; }
    }

    public class Lack1DetailReportTempDto
    {
        public int LACK1_ID { get; set; }
        public string LACK1_NUMBER { get; set; }
        public Enums.Lack1Level LACK1_LEVEL { get; set; }
        public decimal BEGINING_BALANCE { get; set; }
        public decimal? WASTE_QTY { get; set; }
        public string WASTE_UOM { get; set; }
        public decimal? RETURN_QTY { get; set; }
        public string RETURN_UOM { get; set; }
        public decimal TOTAL_INCOME { get; set; }
        public decimal USAGE { get; set; }
        public ICollection<LACK1_INCOME_DETAIL> LACK1_INCOME_DETAIL { get; set; }
        public ICollection<LACK1_TRACKING> LACK1_TRACKING { get; set; }
        public DateTime PeriodDate { get; set; }
    }
    
}
