using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1ItemViewModel : BaseModel
    {
        public Pbck1ItemViewModel()
        {
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
            ActionType = "Edit";
            SupInfo = new Pbck1SupInfo();
        }

        public string SubmitType { get; set; }

        public Pbck1Item Detail { get; set; }

        public Enums.PBCK1Type Pbck1Types { get; set; }

        public SelectList PbckReferenceList { get; set; }

        public SelectList NppbkcList { get; set; }

        public SelectList SupplierPortList { get; set; }

        public SelectList SupplierPlantList { get; set; }

        public SelectList PoaList { get; set; }

        public SelectList GoodTypeList { get; set; }

        public SelectList MonthList { get; set; }

        public SelectList YearList { get; set; }

        public SelectList UomList { get; set; }

        public Enums.DocumentStatusGov StatusGovList { get; set; }

        public Enums.DocumentStatus? DocStatus { get; set; }
        
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }

        public bool AllowApproveAndReject { get; set; }
        
        public bool AllowGovApproveAndReject { get; set; }

        public bool AllowPrintDocument { get; set; }

        public string ActionType { get; set; }

        public bool AllowManagerReject { get; set; }

        public Pbck1SupInfo SupInfo { get; set; }
        
        public List<long> Pbck1OldDecreeFilesID { get; set; }
    }
}