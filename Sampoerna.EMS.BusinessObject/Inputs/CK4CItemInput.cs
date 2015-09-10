using System.Collections.Generic;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class CK4CItemGetByParamInput
    {
        public CK4CItemGetByParamInput()
        {
            MaterialIdList = new List<string>();
        }
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }
        public string ReceivedPlantId { get; set; }
        public Enums.Lack1Level? Lack1Level { get; set; }
        public bool IsHigherFromApproved { get; set; }
        public List<string> MaterialIdList { get; set; }
    }
}
