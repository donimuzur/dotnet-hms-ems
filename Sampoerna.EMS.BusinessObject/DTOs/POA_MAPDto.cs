using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class POA_MAPDto
    {
        public int POA_MAP_ID { get; set; }
        public string NPPBKC_ID { get; set; }
        public string WERKS { get; set; }
        public string PLANT_NAME { get; set; }
        public string POA_ID { get; set; }

        public string POA_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }

    }
}
