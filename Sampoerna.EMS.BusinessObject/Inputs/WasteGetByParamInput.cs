using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WasteGetByParamInput
    {
        public string WasteProductionDate { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public string ShortOrderColumn { get; set; }
        public string UserId { get; set; }
    }
}
