using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1PrimaryResultsViewModel : BaseModel
    {
        public Lack1PrimaryResultsViewModel()
        {
            SearchView = new Lack1SearchPrimaryResultsViewModel();
            Detail = new List<Lack1PrimaryResultsDetail>();
        }

        public Lack1SearchPrimaryResultsViewModel SearchView { get; set; }
        public Lack1SearchPrimaryResultsViewModel ExportSearchView { get; set; }
        public List<Lack1PrimaryResultsDetail> Detail { get; set; }
        
    }

      public class Lack1SearchPrimaryResultsViewModel
    {
        public Lack1SearchPrimaryResultsViewModel()
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

    public class Lack1PrimaryResultsDetail
    {
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string CfProducedProcessOrder { get; set; }
        public string CfCodeProduced { get; set; }
        public string CfProducedDescription { get; set; }
        public string CfProdDate { get; set; }
        public string CfProdQty { get; set; }
        public string CfProdUom { get; set; }
        public string BkcUsed { get; set; }
        public string BkcDescription { get; set; }
        public string BkcIssueQty { get; set; }
        public string BkcIssueUom { get; set; }
        public string Message { get; set; }
    }

}