using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1MonitoringMutasiViewModel : BaseModel
    {
        public Pbck1MonitoringMutasiViewModel()
        {
            DetailsList = new List<Pbck1MonitoringMutasiItem>();
            SearchView = new Pbck1FilterMonitoringMutasiViewModel();
        }
        public string pbck1Number { get; set; }
        public SelectList pbck1NumberList { get; set; }
        public List<Pbck1MonitoringMutasiItem> DetailsList { get; set; }
        public Pbck1FilterMonitoringMutasiViewModel SearchView { get; set; }
    }

    public class Pbck1MonitoringMutasiItem
    {
        public string Pbck1Number { get; set; }

        //include docnumber, GrandtotalExciseable from summaryReport
        public List<CK5.CK5Item> Ck5List { get; set; }

        //From Monitoring Usage
        [UIHint("FormatQty")]
        public decimal QuotaRemaining { get; set; }

        [UIHint("FormatQty")]
        public decimal TotalPbck1Quota { get; set; }

        public string pbck1Number { get; set; }
        public SelectList pbck1NumberList { get; set; }
    }

    public class Pbck1FilterMonitoringMutasiViewModel
    {
        public string pbck1Number { get; set; }
        public SelectList pbck1NumberList { get; set; }
    }
}