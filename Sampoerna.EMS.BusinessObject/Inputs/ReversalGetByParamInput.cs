using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class ReversalGetByParamInput
    {
        public string DateProduction { get; set; }
        public string PlantId { get; set; }
        public string ShortOrderColumn { get; set; }
    }
}
