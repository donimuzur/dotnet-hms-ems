using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1DetailEaViewModel : BaseModel
    {
        public Lack1DetailEaViewModel()
        {
            DetailList = new List<Lack1DetailEaItemModel>();
            SearchView = new Lack1SearchDetailEaViewModel();
        }
        public List<Lack1DetailEaItemModel> DetailList { get; set; }
        public Lack1SearchDetailEaViewModel SearchView { get; set; }

        public Lack1SearchDetailEaViewModel ExportSearchView { get; set; }
    }

    public class Lack1SearchDetailEaViewModel
    {
        public Lack1SearchDetailEaViewModel()
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string PlantReceiverFrom { get; set; }
        public string PlantReceiverTo { get; set; }

        public SelectList PlantReceiverFromList { get; set; }
        public SelectList PlantReceiverToList { get; set; }
    }

    public class Lack1DetailEaItemModel
    {
        public string PlantIdReceiver { get; set; }
        public string PlantDescReceiver { get; set; }
        public string PlantIdSupplier { get; set; }
        public string PlantDescSupplier { get; set; }
        public string EaCode { get; set; }
        public string EaDesc { get; set; }
        public string BeginingBalance { get; set; }
        public string BeginingBalanceUom { get; set; }
        public string Ck5EmsNo { get; set; }
        public string Ck5RegNo { get; set; }
        public string Ck5RegDate { get; set; }
        public decimal Ck5Qty { get; set; }
        public string Usage { get; set; }
        public string UsageUom { get; set; }
        public string UsagePostingDate { get; set; }
        public decimal EndingBalance { get; set; }
        public string EndingBalanceUom { get; set; }

        public List<string> UsageList { get; set; }
        public List<string> UsagePostingDateList { get; set; }

        public List<Lack1DetailLevelItemModel> LevelList { get; set; }
    }

    public class Lack1DetailLevelItemModel
    {
        public string Level { get; set; }
        public string FlavorCode { get; set; }
        public string FlavorDesc { get; set; }
        public string CfProdCode { get; set; }
        public string CfProdDesc { get; set; }
        public string CfProdQty { get; set; }
        public string CfProdUom { get; set; }
        public string ProdPostingDate { get; set; }
        public string ProdDate { get; set; }
    }
}