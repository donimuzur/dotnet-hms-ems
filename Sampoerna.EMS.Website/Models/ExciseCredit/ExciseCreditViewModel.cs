using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditViewModel : MasterViewModel
    {
        public ExciseCreditViewModel() : base()
        {
            
        }
        public List<ExciseCreditModel> ExciseCreditDocuments { set; get; }

        public SelectList NppbkcList { set; get; }
        public SelectList PoaList { set; get; }
        public SelectList CreatorList { set; get; }
        public SelectList YearList { set; get; }
        public SelectList TypeList { set; get; }

        public ExciseFilterModel Filter { set; get; }
        public ExciseCreditSummaryReportsViewModel ExportModel { get; set; }

    }

    public class ExciseFilterModel
    {
        public int Year { set; get; }
        public string POA { set; get; }
        public string Creator { set; get; }
        public string NPPBKC { set; get; }
        public int ExciseCreditType { set; get; }
    }

    public class ExciseCreditSummaryReportsViewModel
    {
        public int YearExport { set; get; }
        public string POAExport { set; get; }
        public string CreatorExport { set; get; }
        public string NPPBKCExport { set; get; }
        public int ExciseCreditTypeExport { set; get; }
        public bool Type { get; set; }
        public bool SubmitDate { get; set; }
        public bool Poa { get; set; }
        public bool NppbkcId { get; set; }
        public bool ExciseNumber { get; set; }
        public bool Amount { get; set; }
        public bool LastUpdate { get; set; }
        public bool Status { get; set; }
    }
}