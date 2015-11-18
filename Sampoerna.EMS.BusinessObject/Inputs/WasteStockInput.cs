using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WasteStockSaveInput
    {
        public WasteStockDto WasteStockDto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }
}
