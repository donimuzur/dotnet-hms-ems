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

        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Ck5MarketReturnQty { get; set; }
        public string FiscalYear { get; set; }
        public string ExciseValue { get; set; }
        public string Pbck3Status { get; set; }
        public string Ck2Value { get; set; }


        public SelectList BrandList { get; set; }
        public SelectList ContentList { get; set; }
        public SelectList HjeList { get; set; }
        public SelectList TariffList { get; set; }
        public SelectList Ck5MarketReturnQtyList { get; set; }
        public SelectList FiscalYearList { get; set; }
        public SelectList ExciseValueList { get; set; }
        public SelectList Pbck3StatusList { get; set; }
        public SelectList Ck2ValueList { get; set; }
    }

    public class CK5MarketReturnSummaryReportsItem
    {
        public long Ck5Id { get; set; }

        public string Ck5Number { get; set; }
        public string PlantId { get; set; }
        public string PlantDesc { get; set; }
        public string Nppbkc { get; set; }
        public string Kppbc { get; set; }
        public string Date { get; set; }
        public string ReqType { get; set; }
        public string ExecDateFrom { get; set; }
        public string ExecDateTo { get; set; }
        public string Back1No { get; set; }
        public string Back1Date { get; set; }
        public string Status { get; set; }
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
        public string CompletedDate { get; set; }

    }

    public class CK5MarketReturnExportSummaryReportsViewModel : CK5MarketReturnSearchSummaryReportsViewModel
    {
        public bool NoRow { get; set; }
        public bool IsSelectCk5Number { get; set; }
        public bool IsSelectPlantId { get; set; }
        public bool IsSelectPlantDesc { get; set; }
        public bool IsSelectNppbkc { get; set; }
        public bool IsSelectKppbc { get; set; }
        public bool IsSelectDate { get; set; }
        public bool IsSelectReqType { get; set; }
        public bool IsSelectExecDateFrom { get; set; }
        public bool IsSelectExecDateTo { get; set; }
        public bool IsSelectBack1No { get; set; }
        public bool IsSelectBack1Date { get; set; }
        public bool IsSelectStatus { get; set; }
        public bool IsSelectFaCode { get; set; }
        public bool IsSelectBrand { get; set; }
        public bool IsSelectContent { get; set; }
        public bool IsSelectHje { get; set; }
        public bool IsSelectTariff { get; set; }
        public bool IsSelectCk5MarketReturnQty { get; set; }
        public bool IsSelectFiscalYear { get; set; }
        public bool IsSelectExciseValue { get; set; }
        public bool IsSelectPoa { get; set; }
        public bool IsSelectCreator { get; set; }
        public bool IsSelectPbck3No { get; set; }
        public bool IsSelectPbck3Status { get; set; }
        public bool IsSelectCk2Number { get; set; }
        public bool IsSelectCk2Value { get; set; }
        public bool IsSelectCompletedDate { get; set; }
    }

}