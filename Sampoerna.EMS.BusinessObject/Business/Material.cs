using System;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class Material
    {

        public long MATERIAL_ID { get; set; }
        public string MATERIAL_NUMBER { get; set; }
        public string MATERIAL_DESC { get; set; }
        public string MATERIAL_GROUP { get; set; }
        public string PURCHASING_GROUP { get; set; }
        public Nullable<long> PLANT_ID { get; set; }
        public Nullable<int> EX_GOODTYP { get; set; }
        public string ISSUE_STORANGE_LOC { get; set; }
        public Nullable<int> BASE_UOM { get; set; }
        public Nullable<int> CREATED_BY { get; set; }
        public Nullable<bool> IS_FROM_SAP { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public Nullable<long> BRAND_ID { get; set; }
    }
}
