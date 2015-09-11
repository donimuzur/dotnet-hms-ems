using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Inputs
{

    public class InvMovementGetForLack1UsageMovementByParamInput
    {
        public InvMovementGetForLack1UsageMovementByParamInput()
        {
            PlantIdList = new List<string>();
            MvtCodeList = new List<string>();
            StoReceiverNumberList = new List<string>();
        }
        public List<string> PlantIdList { get; set; }
        public List<string> MvtCodeList { get; set; }
        public List<string> StoReceiverNumberList { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string NppbkcId { get; set; }
    }

}
