using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class Material
    {
        public long MATERIAL_ID { get; set; }
        
        public string STICKER_ID { get; set; }

        
        public string STICKER_CODE { get; set; }

        
        public long PLANT_ID { get; set; }

        
        public string FA_CODE { get; set; }

        
        public string PURCHASING_GROUP { get; set; }

        
        public string ISSUE_STORAGE { get; set; }

        
        public Nullable<decimal> CONVERSION { get; set; }


        
        public string MATERIAL_DESC { get; set; }


        
        public int UOM_ID { get; set; }

        
        public Nullable<int> GOODTYP_ID { get; set; }

        
        public Nullable<System.DateTime> CREATED_ON { get; set; }

        
        public Nullable<System.DateTime> CHANGED_ON { get; set; }

        
        public string CREATED_DATE { get; set; }

        
        public string CHANGED_BY { get; set; }
    }
}
