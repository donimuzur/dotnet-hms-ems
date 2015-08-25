using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CIndexViewModel
    {
        public DateTime? ProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }

        //selectlist
        public SelectList CompanyNameList { get; set; }
        public SelectList PlanIdList { get; set; }

    }

    public class DataIndecCk4C
    {
        public DateTime? ReportedOn { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string FinishGoods { get; set; }
        public string Description { get; set; }
        public string Quality { get; set; }
        public string Uom { get; set; }
    }
}