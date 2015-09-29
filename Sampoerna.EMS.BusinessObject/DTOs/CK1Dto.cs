using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class CK1Dto
    {
        public long CK1_ID { get; set; }
        public string CK1_NUMBER { get; set; }
        public System.DateTime CK1_DATE { get; set; }
        public string FA_CODE { get; set; }
        public string MATERIAL_ID { get; set; }
        public string PLANT_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    }
}
