using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CIndexViewModel : BaseModel
    {
        public Ck4CIndexViewModel()
        {
          Detail = new List<DataIndecCk4C>();    
        }
        public DateTime? ProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }

        //selectlist
        public SelectList CompanyNameList { get; set; }
        public SelectList PlanIdList { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }
        public List<DataIndecCk4C> Detail { get; set; }
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
        public string Qty { get; set; }
        public string Uom { get; set; }
    }
}