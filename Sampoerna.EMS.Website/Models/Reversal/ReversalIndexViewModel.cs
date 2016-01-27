using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.Reversal
{
    public class ReversalIndexViewModel : BaseModel
    {
        public ReversalIndexViewModel()
        {
            Detail = new List<DataReversal>();
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
        }

        public string ProductionDate { get; set; }
        public string PlantWerks { get; set; }

        public Enums.CK4CType Ck4CType { get; set; }

        public SelectList PlantWerksList { get; set; }

        public List<DataReversal> Detail { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
    }

    public class DataReversal
    {
        public int ReversalId { get; set; }
        public int ZaapShiftId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public Decimal ReversalQty { get; set; }
    }
}