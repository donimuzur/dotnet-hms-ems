using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class ReversalDto
    {
        public int ReversalId { get; set; }
        public int? ZaapShiftId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public Decimal ReversalQty { get; set; }
        public int? InventoryMovementId { get; set; }
    }
}
