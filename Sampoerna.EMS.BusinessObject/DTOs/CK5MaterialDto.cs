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
        public string BRAND { get; set; }
        public string PLANT_ID { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> CONVERTION { get; set; }
        public Nullable<decimal> HJE { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public Nullable<decimal> EXCISE_VALUE { get; set; }
        public Nullable<decimal> USD_VALUE { get; set; }
        public string NOTE { get; set; }
        public string CONVERTED_UOM { get; set; }
        public string MATERIAL_DESC { get; set; }

    }
}
