﻿namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1ProdPlanDto
    {
        public long Pbck1ProdPlanId { get; set; }
        public int Pbck1Id { get; set; }
        public string ProdTypeCode { get; set; }
        public string ProdTypeName { get; set; }
        public string ProdAlias { get; set; }
        public decimal? Amount { get; set; }
        public decimal? BkcRequired { get; set; }
        public string BkcRequiredUomId { get; set; }
        public string BkcRequiredUomName { get; set; }
        public int MonthId { get; set; }
        public string MonthName { get; set; }
        
    }
}