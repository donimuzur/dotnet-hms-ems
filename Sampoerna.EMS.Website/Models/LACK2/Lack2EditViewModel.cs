using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2EditViewModel : Lack2BaseItemModel
    {

        public Lack2EditViewModel()
        {
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
        }

        #region Field
        
        [Required]
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

        public string ControllerAction { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        
        public string Comment { get; set; }

        public SelectList CompanyCodesDDL { get; set; }

        public SelectList NPPBKCDDL { get; set; }

        public SelectList SendingPlantDDL { get; set; }

        public SelectList ExcisableGoodsTypeDDL { get; set; }

        public SelectList MonthList { get; set; }

        public SelectList YearList { get; set; }

        public string PoaList { get; set; }
        public string PoaListHidden { get; set; }

        public Enums.DocumentStatusGov DocGovStatusList { get; set; }

        public List<HttpPostedFileBase> DecreeFiles { get; set; }
        public Enums.ActionType GovApprovalActionType { get; set; }
        public string IsSaveSubmit { get; set; }
        public bool? IsCreateNew { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }

        #endregion
    }
}