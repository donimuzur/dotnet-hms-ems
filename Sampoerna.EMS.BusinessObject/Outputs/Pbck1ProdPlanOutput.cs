﻿using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class Pbck1ProdPlanOutput
    {
        public long Pbck1ProdPlanId { get; set; }
        public long Pbck1Id { get; set; }
        public string ProductCode { get; set; }
        public string ProdTypeName { get; set; }
        public string ProdTypeAlias { get; set; }
        public string Amount { get; set; }
        public string BkcRequired { get; set; }
        public string BkcRequiredUomId { get; set; }
        public string BkcRequiredUomName { get; set; }
        public string Month { get; set; }
        public string MonthName { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }

    public class ValidatePbck1ProdPlanUploadOutput : BLLBaseOutput
    {
        public List<Pbck1ProdPlanOutput> Data { get; set; }
    }
}
