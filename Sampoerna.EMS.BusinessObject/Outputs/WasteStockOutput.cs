using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class GetListMaterialByPlantOutput
    {
        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }
    }

    public class GetListMaterialUomByMaterialAndPlantOutput
    {
        public string Uom { get; set; }
        public string UomDescription { get; set; }
    }
}
