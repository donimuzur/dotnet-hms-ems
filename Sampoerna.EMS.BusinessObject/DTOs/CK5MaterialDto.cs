using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class CK5MaterialDto
    {
        public long CK5_MATERIAL_ID { get; set; }
        public Nullable<long> CK5_ID { get; set; }
        public Nullable<long> MATERIAL_ID { get; set; }
        public Nullable<int> LINE_ITEM { get; set; }
        public Nullable<decimal> QTY { get; set; }
        public Nullable<decimal> CONVERTED_QTY { get; set; }
        public Nullable<int> CONVERTED_UOM_ID { get; set; }
    }
}
