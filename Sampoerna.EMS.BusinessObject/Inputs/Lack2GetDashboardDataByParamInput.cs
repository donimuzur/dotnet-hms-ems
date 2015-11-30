using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack2GetDashboardDataByParamInput
    {
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<string> NppbkcList { get; set; }
        public List<string> DocumentNumberList { get; set; }
        public bool IsOpenDocList { get; set; }
    }
}
