using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class ProductionGetByIdOutput : BLLBaseOutput
    {
        public string CompanyCode { get; set; }
        public string FaCode { get; set; }
        public string PlantWerk { get; set; }
        public DateTime ProductionDate { get; set; }
    }

    public class SaveProductionOutput : BLLBaseOutput
    {
        public bool isNewData { get; set; }
        public bool isFromSap { get; set; }
    }
}
