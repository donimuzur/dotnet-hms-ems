using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class MonthClosingGetByParam
    {
        public string PlantId { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime? DisplayDate { get; set; }
    }
}
