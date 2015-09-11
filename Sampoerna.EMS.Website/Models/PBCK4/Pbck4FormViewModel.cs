﻿using System;
using System.Collections.Generic;
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

        public string PlantDesc { get; set; }
        public string NppbkcId { get; set; }
        public string NppbkcDesc { get; set; }

        public string CompanyName { get; set; }
        public string Poa { get; set; }

        public DateTime ReportedOn { get; set; }


        public bool IsAllowPrint { get; set; }
    }


}