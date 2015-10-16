using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2DetailViewModel : Lack2BaseItemModel
    {

        public Lack2DetailViewModel()
        {
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
            ChangesHistoryList = new List<ChangesHistoryItemModel>();
            PrintHistoryList = new List<PrintHistoryItemModel>();
        }

        #region Field
        
        public DateTime SubmissionDate { get; set; }

        public Enums.DocumentStatus Status { get; set; }
        public string StatusName { get; set; }
        public Enums.DocumentStatusGov? GovStatus { get; set; }
        public string GovStatusName { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Lack2ItemDto> Items { get; set; }
        public List<Lack2DocumentDto> Documents { get; set; }

        public DateTime? DecreeDate { get; set; }

        #endregion

        #region View Purpose
        
        public string PoaList { get; set; }
        public string PoaListHidden { get; set; }

        public string ControllerAction { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        public string MenuLack2OpenDocument { get; set; }
        public string MenuLack2CompletedDocument { get; set; }
        public string Comment { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }

        #endregion

    }
}