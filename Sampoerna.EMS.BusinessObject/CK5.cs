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
    
    public partial class CK5
    {
        public CK5()
        {
            this.CK5_MATERIAL = new HashSet<CK5_MATERIAL>();
        }
    
        public long CK5_ID { get; set; }
        public int CK5_TYPE { get; set; }
        public Nullable<long> KPPBC_CITY { get; set; }
        public string SUBMISSION_NUMBER { get; set; }
        public Nullable<System.DateTime> SUBMISSION_DATE { get; set; }
        public string REGISTRATION_NUMBER { get; set; }
        public Nullable<int> EX_GOODS_TYPE_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.ExciseSettlement EX_SETTLEMENT_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.ExciseStatus EX_STATUS_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.RequestType REQUEST_TYPE_ID { get; set; }
        public string STO_SENDER_NUMBER { get; set; }
        public string STO_RECEIVER_NUMBER { get; set; }
        public string STOB_NUMBER { get; set; }
        public Nullable<long> SOURCE_PLANT_ID { get; set; }
        public Nullable<long> DEST_PLANT_ID { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public Nullable<System.DateTime> INVOICE_DATE { get; set; }
        public Nullable<int> PBCK1_DECREE_ID { get; set; }
        public Nullable<int> CARRIAGE_METHOD_ID { get; set; }
        public Nullable<decimal> GRAND_TOTAL_EX { get; set; }
        public string PACKAGE_UOM_ID { get; set; }
        public Nullable<int> DEST_COUNTRY_ID { get; set; }
        public string HARBOUR { get; set; }
        public string OFFICE_HARBOUR { get; set; }
        public string LAST_SHELTER_HARBOUR { get; set; }
        public string OFFICE_SHELTER { get; set; }
        public string DN_NUMBER { get; set; }
        public Nullable<System.DateTime> GR_DATE { get; set; }
        public Nullable<System.DateTime> GI_DATE { get; set; }
        public string SEALING_NOTIF_NUMBER { get; set; }
        public string UNSEALING_NOTIF_NUMBER { get; set; }
        public Nullable<System.DateTime> SEALING_NOTIF_DATE { get; set; }
        public Nullable<System.DateTime> UNSEALING_NOTIF_DATE { get; set; }
        public string LOADING_PORT { get; set; }
        public string LOADING_PORT_NAME { get; set; }
        public string LOADING_PORT_ID { get; set; }
        public string FINAL_PORT { get; set; }
        public string FINAL_PORT_ID { get; set; }
        public int STATUS_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual ICollection<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual PBCK1 PBCK1 { get; set; }
        public virtual UOM UOM { get; set; }
        public virtual CARRIAGE_METHOD CARRIAGE_METHOD { get; set; }
    }
}
