using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck1ProdPlanInput
    {
        public string Month { get; set; }
        public string ProductCode { get; set; }
        public string Amount { get; set; }
        public string BkcRequired { get; set; }
        public string BkcRequiredUomId { get; set; }
    }

    public class ValidatePbck1ProdPlanUploadParamInput
    {
        public List<Pbck1ProdPlanInput> ProdPlanData { get; set; }
        public DateTime? ProdPlanPeriodFrom { get; set; }
        public DateTime? ProdPlanPeriodTo { get; set; }
        public string GoodType { get; set; }
    }
}
