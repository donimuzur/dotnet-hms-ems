using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class LACK2CreateViewModel
    {
        public SelectList CompanyCodesDDL { get; set; }
        public string SelectedCompanyCode { get; set; }
        public SelectList NPPBKCDDL { get; set; }
        public string SelectedNPPBKC { get; set; }
        public SelectList SendingPlantDDL { get; set; }
        public string SelectedPlant { get; set; }
        public SelectList ExcisableGoodsTypeDDL { get; set; }
        public string SelectedExGoodsType { get; set; }
        public SelectList LACK1PeriodsDDL { get; set; }
        public string SelectedLack1Period { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}