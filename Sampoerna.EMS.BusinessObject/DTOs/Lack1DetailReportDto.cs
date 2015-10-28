using System;
using System.Collections.Generic;

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

    }

    public class Lack1TrackingDetailReportDto
    {
        public string MaterialId { get; set; }
        public decimal SumQty { get; set; }
        public string Batch { get; set; }
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
    
}
