using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1DetailTisViewModel : BaseModel
    {
        public Lack1DetailTisViewModel()
        {
            DetailList = new List<Lack1DetailTisItemModel>();
            SearchView = new Lack1SearchDetailTisViewModel();
        }
        public List<Lack1DetailTisItemModel> DetailList { get; set; }
        public Lack1SearchDetailTisViewModel SearchView { get; set; }

        public Lack1SearchDetailTisViewModel ExportSearchView { get; set; }
    }

    public class Lack1SearchDetailTisViewModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string PlantReceiverFrom { get; set; }
        public string PlantReceiverTo { get; set; }

        public SelectList PlantReceiverFromList { get; set; }
        public SelectList PlantReceiverToList { get; set; }
    }

    public class Lack1DetailTisItemModel
    {
        public string PlantIdReceiver { get; set; }
        public string PlantDescReceiver { get; set; }
        public string PlantIdSupplier { get; set; }
        public string PlantDescSupplier { get; set; }
        public string CfCode { get; set; }
        public string CfDesc { get; set; }
        public string BeginingBalance { get; set; }
        public string BeginingBalanceUom { get; set; }
        public string Ck5EmsNo { get; set; }
        public string Ck5RegNo { get; set; }
        public string Ck5RegDate { get; set; }
        public string Ck5GrDate { get; set; }
        public decimal Ck5Qty { get; set; }
        public string MvtType { get; set; }
        public string Usage { get; set; }
        public string UsageUom { get; set; }
        public string UsagePostingDate { get; set; }
        public string FaCode { get; set; }
        public string FaCodeDesc { get; set; }
        public decimal ProdQty { get; set; }
        public string ProdUom { get; set; }
        public string ProdPostingDate { get; set; }
        public string ProdDate { get; set; }
        public decimal EndingBalance { get; set; }
        public string EndingBalanceUom { get; set; }

        public List<string> MvtTypeList { get; set; }
        public List<string> UsageList { get; set; }
        public List<string> UsagePostingDateList { get; set; }
    }
}