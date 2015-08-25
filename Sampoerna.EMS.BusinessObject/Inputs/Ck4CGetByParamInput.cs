using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Ck4CGetByParamInput
    {
        public string DateProduction { get; set; }
        public string Company { get; set; }
        public string PlantId { get; set; }
        public string ShortOrderColumn { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }

    }
}
