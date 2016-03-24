using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class ProductionGetByParamInput
    {
        public string ProoductionDate { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public string ShortOrderColumn { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<string> ListUserPlants { get; set; }
        public string UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
