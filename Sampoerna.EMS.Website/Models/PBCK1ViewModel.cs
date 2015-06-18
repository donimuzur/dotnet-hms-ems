using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models 
{
    public class PBCK1ViewModel : BaseModel
    {
        public PBCK1ViewModel()
        {
            Details = new List<PBCK1Item>();
            SearchInput = new PBCK1FilterViewModel();
        }
        public List<PBCK1Item> Details { get; set; }

        public PBCK1FilterViewModel SearchInput { get; set; }
        
    }

    public class PBCK1Item
    {
        

        public long PBCK1_ID { get; set; }
        public string NUMBER { get; set; }
        public long? PBCK1_REF { get; set; }
        public DateTime? PERIOD_FROM { get; set; }
        public DateTime? PERIOD_TO { get; set; }
        public DateTime? REPORTED_ON { get; set; }
        public long? NPPBKC_ID { get; set; }
        public int? GOODTYPE_ID { get; set; }
        public int? SUPPLIER_PORT_ID { get; set; }
        public string SUPPLIER_ADDRESS { get; set; }
        public string SUPPLIER_PHONE { get; set; }
        public DateTime? PLAN_PROD_FROM { get; set; }
        public DateTime? PLAN_PROD_TO { get; set; }

        [UIHint("FormatQty")]
        public decimal? REQUEST_QTY { get; set; }
        public int? REQUEST_QTY_UOM { get; set; }
        public int? LACK1_FROM_MONTH_ID { get; set; }
        public int? LACK1_FROM_YEAR { get; set; }
        public int? LACK1_TO_MONTH_ID { get; set; }
        public int? LACK1_TO_YEAR { get; set; }
        public int? STATUS { get; set; }
        public long? STATUS_GOV_ID { get; set; }
        public decimal? QTY_APPROVED { get; set; }
        public DateTime? DECREE_DATE { get; set; }

       
        [UIHint("FormatDateTime")]
        public DateTime? CREATED_DATE { get; set; }
        public int? CREATED_BY { get; set; }
        public int? APPROVED_BY { get; set; }
        public DateTime? APPROVED_DATE { get; set; }

        public ZAIDM_EX_GOODTYP GOODTYPE { get; set; }

        public USER APPROVED_USER { get; set; }

        public string STATUS_NAME { get; set; }

        public ZAIDM_EX_NPPBKC NPPBKC { get; set; }

        public STATUS_GOV STATUS_GOV { get; set; }

        public MONTH LACK1_FROM_MONTH { get; set; }
        public MONTH LACK1_TO_MONTH { get; set; }

    }

    public class PBCK1ItemViewModel : BaseModel
    {
        public PBCK1ItemViewModel()
        {
            ProductConversions = new List<PBCK1ProdConvModel>();
            ProductPlans = new List<PBCK1ProdPlanModel>();
        }
        public PBCK1Item Detail { get; set; }

        public Enums.PBCK1Type PBCK1Types { get; set; }

        public List<PBCK1ProdConvModel> ProductConversions { get; set; }
        public List<PBCK1ProdPlanModel> ProductPlans { get; set; }

    }

    public class PBCK1ProdConvModel
    {
        public int? ProductCode { get; set; }
        public string ProductTypeAlias { get; set; }
        public string ProductType { get; set; }
        public decimal? ConverterOutput { get; set; }
        public string ConverterUom { get; set; }
    }
    public class PBCK1ProdPlanModel
    {
        public int MonthId { get; set; }
        public string MonthName { get; set; }
        public int? ProductCode { get; set; }
        public string ProductTypeAlias { get; set; }
        public string ProductType { get; set; }
        public decimal? Amount { get; set; }
        public string BKCRequires { get; set; }
    }
}