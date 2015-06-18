using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class PBCK1CreateViewModel
    {
        public PBCK1Detail Detail { get; set; }

        //for views
        public Enums.PBCK1Type PBCK1Types { get; set; }

        public SelectList NppbkcList { get; set; }

        public SelectList SupplierPortList { get; set; }

        public SelectList PoaList { get; set; }

        public SelectList GoodTypeList { get; set; }

        public SelectList MonthList { get; set; }

        public SelectList YearList { get; set; }

        public SelectList UOMList { get; set; }

    }

    public class PBCK1Detail
    {

        #region Tab PBCK-1 Details

        public long PBCK1_ID { get; set; }
        public string NUMBER { get; set; }
        public long? PBCK1_REF { get; set; }
        public string PBCK1_TYPE { get; set; }
        public DateTime? PERIOD_FROM { get; set; }
        public DateTime? PERIOD_TO { get; set; }
        public DateTime? REPORTED_ON { get; set; }

        #endregion
        
        public long? NPPBKC_ID { get; set; }
        public int? SUPPLIER_PORT_ID { get; set; }
        public int? GOODTYPE_ID { get; set; }
        public string SUPPLIER_PLANT { get; set; }
        
        public string SUPPLIER_ADDRESS { get; set; }
        public string SUPPLIER_PHONE { get; set; }
        public DateTime? PLAN_PROD_FROM { get; set; }
        public DateTime? PLAN_PROD_TO { get; set; }
        public decimal? REQUEST_QTY { get; set; }
        public int? REQUEST_QTY_UOM { get; set; }
        public int? LACK1_FROM_MONTH { get; set; }
        public int? LACK1_FROM_YEAR { get; set; }
        public int? LACK1_TO_MONTH { get; set; }
        public int? LACK1_TO_YEAR { get; set; }
        public int? STATUS { get; set; }
        public int? STATUS_GOV { get; set; }
        public decimal? QTY_APPROVED { get; set; }
        public DateTime? DECREE_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public int? CREATED_BY { get; set; }
        public int? APPROVED_BY { get; set; }
        public DateTime? APPROVED_DATE { get; set; }

        public MONTH MONTH { get; set; }
        public MONTH MONTH1 { get; set; }
        public List<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER { get; set; }
        public List<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER1 { get; set; }
        public List<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public STATUS STATUS1 { get; set; }
        public STATUS_GOV STATUS_GOV1 { get; set; }
        public SUPPLIER_PORT SUPPLIER_PORT { get; set; }
        public UOM UOM { get; set; }
        public USER USER { get; set; }
        public ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }
        public ZAIDM_EX_NPPBKC ZAIDM_EX_NPPBKC { get; set; }
        public ZAIDM_EX_POA ZAIDM_EX_POA { get; set; }
        public List<REALISASI_PEMASUKAN> REALISASI_PEMASUKAN { get; set; }
        public List<RENCANA_PRODUKSI> RENCANA_PRODUKSI { get; set; }

    }

}