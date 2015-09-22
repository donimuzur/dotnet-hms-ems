using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CIndexWasteProductionViewModel : BaseModel
    {
        public Ck4CIndexWasteProductionViewModel()
        {
            Detail = new List<DataWasteProduction>();    
        }
        public DateTime? ProductionDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }

        //selectlist
        public SelectList CompanyNameList { get; set; }
        public SelectList PlanIdList { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }
        public List<DataWasteProduction> Detail { get; set; }
    }
     public class DataWasteProduction
    {
        public DateTime? ReportedOn { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string FinishGoods { get; set; }
        public string Description { get; set; }
        public string WasteQty { get; set; }
        public string Uom { get; set; }
    }
}