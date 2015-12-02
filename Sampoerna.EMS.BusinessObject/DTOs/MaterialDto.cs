using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class MaterialDto
    {
        public string STICKER_CODE { get; set; }
        public string MATERIAL_DESC { get; set; }
        public string MATERIAL_GROUP { get; set; }
        public string PURCHASING_GROUP { get; set; }
        public string WERKS { get; set; }
        public string EXC_GOOD_TYP { get; set; }
        public string ISSUE_STORANGE_LOC { get; set; }
        public string BASE_UOM_ID { get; set; }

        public string EXT_TYP_DESC { get; set; }
        public Nullable<decimal> HJE { get; set; }
        public string HJE_CURR { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public string TARIFF_CURR { get; set; }
        public bool IS_FROM_SAP { get; set; }
        public Nullable<bool> CLIENT_DELETION { get; set; }
        public Nullable<bool> PLANT_DELETION { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }

        public ICollection<MATERIAL_UOM> MATERIAL_UOM { get; set; }
        public T001WDto T001W { get; set; }
        public UOM UOM { get; set; }
        public ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }

        public string GoodTypeDescription { get; set; }
    }
}
