using System;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class PBCK1Item
    {
        public long PBCK1_ID { get; set; }
        public string NUMBER { get; set; }
        public long? PBCK1_REF { get; set; }
        public Enums.PBCK1Type PBCK1_TYPE { get; set; }
        public string PBCK1_TYPEText { get; set; }
        public DateTime? PERIOD_FROM { get; set; }
        public DateTime? PERIOD_TO { get; set; }
        public string Year { get; set; }
        public DateTime? REPORTED_ON { get; set; }
        public long? NPPBKC_ID { get; set; }
        public string CompanyName { get; set; }
        public string NPPBKC_NO { get; set; }
        public int? GOODTYPE_ID { get; set; }
        public string GOODTYPE_DESC { get; set; }
        public string SUPPLIER_PLANT { get; set; }
        public int? SUPPLIER_PORT_ID { get; set; }
        public string SUPPLIER_ADDRESS { get; set; }
        public string SUPPLIER_PHONE { get; set; }
        public DateTime? PLAN_PROD_FROM { get; set; }
        public DateTime? PLAN_PROD_TO { get; set; }
        [UIHint("FormatQty")]
        public decimal? REQUEST_QTY { get; set; }
        public int? REQUEST_QTY_UOM { get; set; }
        public string REQUEST_QTY_UOM_NAME { get; set; }
        public int? LACK1_FROM_MONTH_ID { get; set; }
        public string LACK1_FROM_MONTH_NAME { get; set; }
        public int? LACK1_FROM_YEAR { get; set; }
        public int? LACK1_TO_MONTH_ID { get; set; }
        public string LACK1_TO_MONTH_NAME { get; set; }
        public int? LACK1_TO_YEAR { get; set; }
        public Enums.DocumentStatus STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public Enums.DocumentStatus STATUS_GOV { get; set; }
        public string STATUS_GOV_NAME { get; set; }
        [UIHint("FormatQty")]
        public decimal? QTY_APPROVED { get; set; }
        public DateTime? DECREE_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public int? CREATED_BY { get; set; }
        public string CREATED_USERNAME { get; set; }
        public int? APPROVED_BY { get; set; }
        public string APPROVED_USERNAME { get; set; }
        public DateTime? APPROVED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public decimal? LATEST_SALDO { get; set; }
        public int? LATEST_SALDO_UOM { get; set; }
        public string LATEST_SALDO_UOM_NAME { get; set; }
        
        public string SUPPLIER_PORT_NAME { get; set; }
        
    }
}