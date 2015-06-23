using System;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class PBCK1Item
    {
        public long PBCK1_ID { get; set; }

        [Required, Display(Name = "PBCK-1 No")]
        public string NUMBER { get; set; }

        [Required, Display(Name = "References")]
        public long? PBCK1_REF { get; set; }

        [Required, Display(Name = "PBCK Type")]
        public Enums.PBCK1Type PBCK1_TYPE { get; set; }
        
        public string PBCK1_TYPEText { get; set; }

        [Required, Display(Name = "Period From")]
        public DateTime? PERIOD_FROM { get; set; }

        [Required, Display(Name = "Period To")]
        public DateTime? PERIOD_TO { get; set; }
        public string Year { get; set; }

        [Required, Display(Name = "Reported On")]
        public DateTime? REPORTED_ON { get; set; }

        [Required, Display(Name = "NPPBKC ID")]
        public long? NPPBKC_ID { get; set; }

        public string CompanyName { get; set; }
        public string NPPBKC_NO { get; set; }

        [Required, Display(Name = "Exciseable Goods Description")]
        public int? GOODTYPE_ID { get; set; }

        public string GOODTYPE_DESC { get; set; }
        public string SUPPLIER_PLANT { get; set; }

        [Required, Display(Name = "Supplier Port")]
        public int? SUPPLIER_PORT_ID { get; set; }

        [Required, Display(Name = "Supplier Address")]
        public string SUPPLIER_ADDRESS { get; set; }

        [Required, Display(Name = "Supplier Phone")]
        public string SUPPLIER_PHONE { get; set; }

        [Required, Display(Name = "Plan Production From")]
        public DateTime? PLAN_PROD_FROM { get; set; }

        [Required, Display(Name = "Plan Production To")]
        public DateTime? PLAN_PROD_TO { get; set; }

        [UIHint("FormatQty")]
        [Required, Display(Name = "Request Qty")]
        public decimal? REQUEST_QTY { get; set; }

        [Required]
        public int? REQUEST_QTY_UOM { get; set; }
        public string REQUEST_QTY_UOM_NAME { get; set; }

        [Required, Display(Name = "LACK-1 From")]
        public int? LACK1_FROM_MONTH_ID { get; set; }

        public string LACK1_FROM_MONTH_NAME { get; set; }

        [Required]
        public int? LACK1_FROM_YEAR { get; set; }

        [Required, Display(Name = "LACK-1 To")]
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