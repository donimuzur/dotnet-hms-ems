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
        public Nullable<int> CK5_TYPE { get; set; }
        public Nullable<long> KPPBC_CITY { get; set; }
        public string SUMBISSION_NUMBER { get; set; }
        public string REGISTRATION_NUMBER { get; set; }
        public Nullable<int> EX_GOODS_TYPE_ID { get; set; }
        public int EX_SETTLEMENT_ID { get; set; }
        public Nullable<int> EX_STATUS_ID { get; set; }
        public Nullable<int> REQUEST_TYPE_ID { get; set; }
        public string STO_SENDER_NUMBER { get; set; }
        public string STO_RECEIVER_NUMBER { get; set; }
        public string STOB_NUMBER { get; set; }
        public Nullable<long> SOURCE_PLANT_ID { get; set; }
        public Nullable<long> DEST_PLANT_ID { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public Nullable<System.DateTime> INVOICE_DATE { get; set; }
        public string PBCK1_DECREE_NUMBER { get; set; }
        public Nullable<System.DateTime> PBCK1_DECREE_DATE { get; set; }
        public Nullable<int> CARRIAGE_METHOD_ID { get; set; }
        public Nullable<decimal> GRAND_TOTAL_EX { get; set; }
        public Nullable<int> PACKAGE_UOM_ID { get; set; }
        public Nullable<int> DEST_COUNTRY_ID { get; set; }
        public string HARBOUR { get; set; }
        public string OFFICE_HARBOUR { get; set; }
        public string LAST_SHELTER_HARBOUR { get; set; }
        public string OFFICE_SHELTER { get; set; }
        public string DN_NUMBER { get; set; }
        public Nullable<System.DateTime> GI_DATE { get; set; }
        public string SEALING_NOTIF_NUMBER { get; set; }
        public string UNSEALING_NOTIF_NUMBER { get; set; }
        public Nullable<System.DateTime> SEALING_NOTIF_DATE { get; set; }
        public System.DateTime UNSEALING_NOTIF_DATE { get; set; }
        public Nullable<int> STATUS_ID { get; set; }
        public Nullable<int> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<int> APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
    
        public virtual CARRIAGE_METHOD CARRIAGE_METHOD { get; set; }
        public virtual COUNTRY COUNTRY { get; set; }
        public virtual ZAIDM_EX_KPPBC ZAIDM_EX_KPPBC { get; set; }
        public virtual CK5_TYPE CK5_TYPE1 { get; set; }
        public virtual EX_SETTLEMENT EX_SETTLEMENT { get; set; }
        public virtual EX_STATUS EX_STATUS { get; set; }
        public virtual ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }
        public virtual ICollection<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual REQUEST_TYPE REQUEST_TYPE { get; set; }
        public virtual STATUS STATUS { get; set; }
        public virtual T1001W T1001W { get; set; }
        public virtual T1001W T1001W1 { get; set; }
        public virtual UOM UOM { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
    }
}
