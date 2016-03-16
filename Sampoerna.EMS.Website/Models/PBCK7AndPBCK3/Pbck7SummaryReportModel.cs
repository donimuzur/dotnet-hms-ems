using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK7AndPBCK3
{
    public class Pbck7SummaryReportModel : BaseModel
    {
        public Pbck7SummaryReportModel()
        {
            ReportItems = new List<Pbck7SummaryReportItem>();
            //ExportModel = new Pbck7ExportModel();
        }
        public string SelectedNumber { get; set; }
        public string SelectedNppbkc { get; set; }
        public string SelectedPlant { get; set; }
      
        public SelectList Pbck7List { get; set; }

        public SelectList PlantList { get; set; }

        public SelectList NppbkcList { get; set; }

        public SelectList FromYear { get; set; }

        public SelectList ToYear { get; set; }

        public int?  From { get; set; }

        public int? To { get; set; }


        public string FaCode { get; set; }
        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Pbck7Qty { get; set; }
        public string FiscalYear { get; set; }
        public string ExciseValue { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string Pbck3No { get; set; }
        public string Pbck3Status { get; set; }
        public string Ck2No { get; set; }
        public string Ck2Value { get; set; }

        public SelectList FaCodeList { get; set; }
        public SelectList BrandList { get; set; }
        public SelectList ContentList { get; set; }
        public SelectList HjeList { get; set; }
        public SelectList TariffList { get; set; }
        public SelectList Pbck7QtyList { get; set; }
        public SelectList FiscalYearList { get; set; }
        public SelectList ExciseValueList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList CreatorList { get; set; }
        public SelectList Pbck3NoList { get; set; }
        public SelectList Pbck3StatusList { get; set; }
        public SelectList Ck2NoList { get; set; }
        public SelectList Ck2ValueList { get; set; }

        public List<Pbck7SummaryReportItem> ReportItems { get; set; }

        public Pbck7ExportModel ExportModel { get; set; }
    }

    public class Pbck3Pbck7SummaryReportDetailModel
    {
    }

    public  class Pbck7ExportModel
    {
        public string NppbkcId { get; set; }

        public string Pbck7No { get; set; }

        public string Plant { get; set; }

        public Enums.DocumentTypePbck7AndPbck3? DocType { get; set; }

        public Enums.DocumentStatus Status { get; set; }
        
        public int? FromYear { get; set; }

        public int? ToYear { get; set; }

        public string FaCode { get; set; }
        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Pbck7Qty { get; set; }
        public string FiscalYear { get; set; }
        public string ExciseValue { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string Pbck3No { get; set; }
        public string Pbck3Status { get; set; }
        public string Ck2No { get; set; }
        public string Ck2Value { get; set; }

        public bool IsSelectNppbkc { get; set; }

        public bool IsSelectPbck7No { get; set; }

        public bool IsSelectPlant { get; set; }

        public bool IsSelectDocType { get; set; }

        public bool IsSelectStatus { get; set; }

        public bool IsSelectDate { get; set; }
        public bool IsSelectExecFrom { get; set; }
        public bool IsSelectExecTo { get; set; }

        public bool IsSelectBack1No { get; set; }
        public bool IsSelectBack1Date { get; set; }

        public bool IsSelectFaCode { get; set; }
        public bool IsSelectBrand { get; set; }
        public bool IsSelectContent { get; set; }
        public bool IsSelectHje { get; set; }
        public bool IsSelectTariff { get; set; }
        public bool IsSelectPbck7Qty { get; set; }
        public bool IsSelectFiscalYear { get; set; }
        public bool IsSelectExciseValue { get; set; }
        public bool IsSelectPoa { get; set; }
        public bool IsSelectCreator { get; set; }
        public bool IsSelectPbck3No { get; set; }
        public bool IsSelectPbck3Status { get; set; }
        public bool IsSelectCk2No { get; set; }
        public bool IsSelectCk2Value { get; set; }
        public bool IsSelectCompletedDate { get; set; }
    }

    public class Pbck7SummaryReportItem
    {
        public string Pbck7Number { get; set; }
        public string Pbck7Date { get; set; }
        public string PlantName { get; set; }
        public string DocumentType { get; set; }
        public string Nppbkc { get; set; }
        public string ExecFrom { get; set; }
        public string ExecTo { get; set; }
        public string Back1No { get; set; }
        public string Back1Date { get; set; }
        public string Pbck7Status { get; set; }

        public string FaCode { get; set; }
        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Pbck7Qty { get; set; }
        public string FiscalYear { get; set; }
        public string ExciseValue { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string Pbck3No { get; set; }
        public string Pbck3Status { get; set; }
        public string Ck2No { get; set; }
        public string Ck2Value { get; set; }
        public string CompletedDate { get; set; }
    }
}