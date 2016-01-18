using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5MarketReturnSummaryReportsViewModel : BaseModel
    {
        public CK5MarketReturnSummaryReportsViewModel()
        {
            SearchView = new CK5MarketReturnSearchSummaryReportsViewModel();
            DetailsList = new List<CK5MarketReturnSummaryReportsItem>();
        }

        public CK5MarketReturnSearchSummaryReportsViewModel SearchView { get; set; }
        public List<CK5MarketReturnSummaryReportsItem> DetailsList { get; set; }

        public CK5MarketReturnExportSummaryReportsViewModel ExportModel { get; set; }

       
    }

    public class CK5MarketReturnSearchSummaryReportsViewModel
    {

        public string FaCode { get; set; }
        public SelectList FaCodeList { get; set; }

        public string Poa { get; set; }
        public SelectList PoaList { get; set; }

        public string Creator { get; set; }
        public SelectList CreatorList { get; set; }

        public string Pbck3No { get; set; }
        public SelectList Pbck3NoList { get; set; }

        public string Ck2No { get; set; }
        public SelectList Ck2NoList { get; set; }

    }

    public class CK5MarketReturnSummaryReportsItem
    {
        public long Ck5Id { get; set; }

        public string FaCode { get; set; }
        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Ck5MarketReturnQty { get; set; }
        public string FiscalYear { get; set; }
        public string ExciseValue { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string Pbck3No { get; set; }
        public string Pbck3Status { get; set; }
        public string Ck2Number { get; set; }
        public string Ck2Value { get; set; }
        public string Status { get; set; }

    }

    public class CK5MarketReturnExportSummaryReportsViewModel : CK5MarketReturnSearchSummaryReportsViewModel
    {
        public bool NoRow { get; set; }
        public bool FaCode { get; set; }
        public bool Brand { get; set; }
        public bool Content { get; set; }
        public bool Hje { get; set; }
        public bool Tariff { get; set; }
        public bool Ck5MarketReturnQty { get; set; }
        public bool FiscalYear { get; set; }
        public bool ExciseValue { get; set; }
        public bool Poa { get; set; }
        public bool Creator { get; set; }
        public bool Pbck3No { get; set; }
        public bool Pbck3Status { get; set; }
        public bool Ck2Number { get; set; }
        public bool Ck2Value { get; set; }
        public bool Status { get; set; }

    }

}