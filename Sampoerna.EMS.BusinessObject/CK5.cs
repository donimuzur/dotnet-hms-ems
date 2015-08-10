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
            this.CK5_FILE_UPLOAD = new HashSet<CK5_FILE_UPLOAD>();
            this.PBCK1_QUOTA = new HashSet<PBCK1_QUOTA>();
            this.LACK2_ITEM = new HashSet<LACK2_ITEM>();
        }
    
        public long CK5_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.CK5Type CK5_TYPE { get; set; }
        public string KPPBC_CITY { get; set; }
        public string CE_OFFICE_CODE { get; set; }
        public string SUBMISSION_NUMBER { get; set; }
        public Nullable<System.DateTime> SUBMISSION_DATE { get; set; }
        public string REGISTRATION_NUMBER { get; set; }
        public string EX_GOODS_TYPE_DESC { get; set; }
        public Sampoerna.EMS.Core.Enums.ExciseSettlement EX_SETTLEMENT_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.ExciseStatus EX_STATUS_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.RequestType REQUEST_TYPE_ID { get; set; }
        public string STO_SENDER_NUMBER { get; set; }
        public string STO_RECEIVER_NUMBER { get; set; }
        public string STOB_NUMBER { get; set; }
        public string SOURCE_PLANT_ID { get; set; }
        public string DEST_PLANT_ID { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public Nullable<System.DateTime> INVOICE_DATE { get; set; }
        public Nullable<int> PBCK1_DECREE_ID { get; set; }
        public Nullable<Sampoerna.EMS.Core.Enums.CarriageMethod> CARRIAGE_METHOD_ID { get; set; }
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
        public Sampoerna.EMS.Core.Enums.DocumentStatus STATUS_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string APPROVED_BY_POA { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE_POA { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string SOURCE_PLANT_NPWP { get; set; }
        public string SOURCE_PLANT_NPPBKC_ID { get; set; }
        public string SOURCE_PLANT_COMPANY_NAME { get; set; }
        public string SOURCE_PLANT_ADDRESS { get; set; }
        public string SOURCE_PLANT_KPPBC_NAME_OFFICE { get; set; }
        public string DEST_PLANT_NPWP { get; set; }
        public string DEST_PLANT_NPPBKC_ID { get; set; }
        public string DEST_PLANT_COMPANY_NAME { get; set; }
        public string DEST_PLANT_ADDRESS { get; set; }
        public string DEST_PLANT_KPPBC_NAME_OFFICE { get; set; }
        public Nullable<System.DateTime> REGISTRATION_DATE { get; set; }
        public Nullable<System.DateTime> DN_DATE { get; set; }
        public string SOURCE_PLANT_NAME { get; set; }
        public string DEST_PLANT_NAME { get; set; }
        public string FINAL_PORT_NAME { get; set; }
        public string APPROVED_BY_MANAGER { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE_MANAGER { get; set; }
        public string SOURCE_PLANT_COMPANY_CODE { get; set; }
        public string DEST_PLANT_COMPANY_CODE { get; set; }
        public Sampoerna.EMS.Core.Enums.ExGoodsType EX_GOODS_TYPE { get; set; }
        public string DEST_COUNTRY_CODE { get; set; }
        public string DEST_COUNTRY_NAME { get; set; }
    
        public virtual ICollection<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual PBCK1 PBCK1 { get; set; }
        public virtual ICollection<CK5_FILE_UPLOAD> CK5_FILE_UPLOAD { get; set; }
        public virtual UOM UOM { get; set; }
        public virtual ICollection<PBCK1_QUOTA> PBCK1_QUOTA { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual USER USER2 { get; set; }
        public virtual ICollection<LACK2_ITEM> LACK2_ITEM { get; set; }
    }
}
