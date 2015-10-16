﻿using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.PBCK7AndPBCK3
{
    public class Pbck3ViewModel : BaseModel
    {
        public Pbck3ViewModel()
        {
            Pbck7UploadItems = new List<Pbck7UploadViewModel>();
            WorkflowHistoryPbck3 = new List<WorkflowHistoryViewModel>();
            WorkflowHistoryPbck7 = new List<WorkflowHistoryViewModel>();

            Back1Documents = new List<BACK1_DOCUMENT>();
            Ck2Documents = new List<CK2_DOCUMENT>();
        }
        public int Pbck3Id { get; set; }
        public string Pbck3Number { get; set; }
        public DateTime PBCK3_DATE { get; set; }

        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }

        
        public Enums.DocumentStatus Pbck3Status { get; set; }
        public string Pbck3StatusDescription { get; set; }
        public Enums.DocumentStatusGov Pbck3GovStatus { get; set; }
        public string Pbck3GovStatusDescription { get; set; }

        public List<WorkflowHistoryViewModel> WorkflowHistoryPbck3 { get; set; }


        public bool FromPbck7 { get; set; }

        //pbck7
        public int Pbck7Id { get; set; }
        public string Pbck7Number { get; set; }
        public DateTime Pbck7Date { get; set; }

        public Enums.DocumentStatus Pbck7Status { get; set; }
        public string Pbck7StatusDescription { get; set; }
        public Enums.DocumentStatusGov Pbck7GovStatus { get; set; }
        public string Pbck7GovStatusDescription { get; set; }

        public Enums.DocumentTypePbck7AndPbck3 DocumentType { get; set; }
        public string DocumentTypeDescription { get; set; }
       
        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
        public string NppbkcId { get; set; }
        public string Lampiran { get; set; }
        public string PlantId { get; set; }
        public string PoaList { get; set; }

        public List<Pbck7UploadViewModel> Pbck7UploadItems { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistoryPbck7 { get; set; }

        //back1
        public int Back1Id { get; set; }
        public string Back1Number { get; set; }
        public DateTime? Back1Date { get; set; }
        public List<BACK1_DOCUMENT> Back1Documents { get; set; } 

        //ck2
        public int Ck2Id { get; set; }
        public string Ck2Number { get; set; }
        public DateTime? Ck2Date { get; set; }
        public decimal? Ck2Value { get; set; }
        public List<CK2_DOCUMENT> Ck2Documents { get; set; }

        //back3
        public int Back3Id { get; set; }
        public string Back3Number { get; set; }
        public DateTime? Back3Date { get; set; }
        public List<BACK3_DOCUMENT> Back3Documents { get; set; } 

       

        //general
        public bool IsSaveSubmit { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }

        public string ActionType { get; set; }

        public string Comment { get; set; }
    }
}