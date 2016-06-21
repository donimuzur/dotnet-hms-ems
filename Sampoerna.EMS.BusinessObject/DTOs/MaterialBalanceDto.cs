using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class MaterialBalanceDto
    {
        public string MaterialId { get; set; }
        public string Werks { get; set; }
        public string StorLoc { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public decimal OpenBalance { get; set; }
        public decimal CloseBalance { get; set; }
    }

    public class MaterialBalanceTotalDto
    {
        public decimal BeginningBalance { get; set; }
        public string BeginningBalanceUom { get; set; }
    }
}
