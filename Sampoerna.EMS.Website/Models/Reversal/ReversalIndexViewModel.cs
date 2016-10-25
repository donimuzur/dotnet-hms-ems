using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public SelectList ZaapShiftList { get; set; }
        public SelectList InventoryMovementList { get; set; }
        public SelectList FaCodeList { get; set; }
        public SelectList PlantWerksList { get; set; }
        public DataReversal Details { get; set; }

        public List<DataReversal> Detail { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
    }

    public class DataReversal
    {
        public int ReversalId { get; set; }
        public int? ZaapShiftId { get; set; }
        [Required]
        public DateTime? ProductionDate { get; set; }
        [Required]
        public string FaCode { get; set; }
        [Required]
        public string Werks { get; set; }
        [Required]
        public Decimal ReversalQty { get; set; }
        public string ProductionDateDisplay { get; set; }
        public Decimal ReversalRemaining { get; set; }
        public Decimal PackedQty { get; set; }
        public int? InventoryMovementId { get; set; }
    }
}