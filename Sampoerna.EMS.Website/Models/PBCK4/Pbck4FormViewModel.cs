﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.PBCK4
{
    public class Pbck4FormViewModel : BaseModel
    {
        public Pbck4FormViewModel()
        {
            UploadItemModels = new List<Pbck4UploadViewModel>();
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
        }

        public List<Pbck4UploadViewModel> UploadItemModels { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }

        public Enums.DocumentStatus DocumentStatus { get; set; }
        public string DocumentStatusDescription { get; set; }

        public int Pbck4Id { get; set; }

        public string Pbck4Number { get; set; }

        public string PlantId { get; set; }
        public SelectList PlantList { get; set; }
        public SelectList NppbkcList { get; set; }

        public string PlantDesc { get; set; }
        public string NppbkcId { get; set; }
        public string NppbkcDesc { get; set; }

        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string Poa { get; set; }

        public string APPROVED_BY_MANAGER { get; set; }
        public DateTime? APPROVED_BY_MANAGER_DATE { get; set; }


        public string BACK1_NO { get; set; }
        public DateTime? BACK1_DATE { get; set; }
        public string CK3_NO { get; set; }
        public DateTime? CK3_DATE { get; set; }
        public string CK3_OFFICE_VALUE { get; set; }

        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public DateTime ReportedOn { get; set; }

        public string Comment { get; set; }

        public string Command { get; set; }
        public bool IsAllowPrint { get; set; }
        public string ActionType { get; set; }

        public bool AllowApproveAndReject { get; set; }

        public bool AllowGovApproveAndReject { get; set; }

        public bool AllowManagerReject { get; set; }

        public bool IsWaitingGovApproval { get; set; }

        public Enums.DocumentStatusGov GovStatus { get; set; }
        public string GovStatusDesc { get; set; }
        public Enums.DocumentStatusGov GovStatusList { get; set; }
        public string CommentGov { get; set; }

        public List<HttpPostedFileBase> Pbck4FileUploadFileList { get; set; }

        //back-1
        public List<Pbck4FileUploadViewModel> Pbck4FileUploadModelList { get; set; }

        public List<HttpPostedFileBase> Pbck4FileUploadFileList2 { get; set; }
        //ck-3
        public List<Pbck4FileUploadViewModel> Pbck4FileUploadModelList2 { get; set; }

        public decimal RequestedQty { get; set; }
        public decimal ApprovedQty { get; set; }
        public string NoPengawas { get; set; }
        public string Remarks { get; set; }
        public string Ck1Date { get; set; }

        public bool AllowEditCompletedDocument { get; set; }
    }


}