//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sampoerna.EMS.BusinessObject
{
    using System;
    using System.Collections.Generic;
    
    public partial class VIRTUAL_PLANT_MAP
    {
        public long VIRTUAL_PLANT_MAP_ID { get; set; }
        public string IMPORT_PLANT_ID { get; set; }
        public string EXPORT_PLANT_ID { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
    
        public virtual T001 T001 { get; set; }
        public virtual T001W T001W { get; set; }
        public virtual T001W T001W1 { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
    }
}
