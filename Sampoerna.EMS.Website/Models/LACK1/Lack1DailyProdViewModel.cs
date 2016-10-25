using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1DailyProdViewModel : BaseModel
    {
        public Lack1DailyProdViewModel()
        {
            SearchView = new Lack1SearchDailyProdViewModel();
            Detail = new List<Lack1DailyProdDetail>();
        }

        public Lack1SearchDailyProdViewModel SearchView { get; set; }
        public Lack1SearchDailyProdViewModel ExportSearchView { get; set; }
        public List<Lack1DailyProdDetail> Detail { get; set; }

       
    }

    public class Lack1SearchDailyProdViewModel
    {
        public Lack1SearchDailyProdViewModel()
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
        }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }

        public SelectList PlantFromList { get; set; }
        public SelectList PlantToList { get; set; }

    }

    public class Lack1DailyProdDetail
    {
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string FaCode { get; set; }
        public string FaCodeDescription { get; set; }
        public string ProductionDate { get; set; }
        public string ProdQty { get; set; }
        public string ProdUom { get; set; }
        public string RejectParkerQty { get; set; }
        public string RejectParkerUom { get; set; }

        public string Zb { get; set; }
        public string PackedAdjusted { get; set; }


        public string ZbUom { get; set; }
        
        public string SapPackedQty { get; set; }

        public string SapPackedUom { get; set; }
        
        public string PackedAdjustedUom { get; set; }
        
        public string Remark { get; set; }
        
        public string SapReversalQty { get; set; }
        public string SapReversalQtyUom { get; set; }

        
    }
}