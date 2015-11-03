using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class ZaapShiftRptGetForLack1ByParamInput
    {
        public ZaapShiftRptGetForLack1ByParamInput()
        {
            FaCodeList = new List<string>();
        }
        public string CompanyCode { get; set; }
        public string Werks { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public List<string> FaCodeList { get; set; }
    }
}
