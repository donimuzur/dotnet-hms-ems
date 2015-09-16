using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK7AndPBCK3
{
    public class Pbck7Pbck3CreateViewModel : BaseModel
    {

        public int Id { get; set; }
        public string Pbck7Number { get; set; }

        public string PlantId { get; set; }

        public Enums.DocumentStatus Pbck7Status { get; set; }
        public string Pbck7StatusName { get; set; }

        public Enums.DocumentStatus Pbck3Status
        {
            get;
            set;
        }

        public string Pbck3StatusName { get; set; }
       
        public string PlantName { get; set; }
        public DateTime? Pbck7Date { get; set; }
        public string Pbck3Number { get; set; }
        public DateTime? Pbck3Date { get; set; }
        public Enums.DocumentTypePbck7AndPbck3 DocumentType { get; set; }
        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
        public string NppbkcId { get; set; }
        public string Lampiran { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string RejectedBy { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public string ApprovedByManager { get; set; }

        public DateTime? ApprovedDateManager { get; set; }


        //selectList
        public SelectList NppbkIdList { get; set; }
        public SelectList PlantList { get; set; }

        public Enums.DocumentTypePbck7AndPbck3 DocumentTypeList { get; set; }

        public List<Pbck7ItemUpload> UploadItems { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistoryPbck7 { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistoryPbck3 { get; set; }

        public bool AllowApproveAndReject { get; set; }

        public bool AllowEditAndSubmit { get; set; }

        public bool AllowGovApproveAndReject { get; set; }

        public bool AllowManagerReject { get; set; }

        public bool AllowPrintDocument { get; set; }

        public bool IsSaveSubmit { get; set; }

        public string ActionType { get; set; }

        public string Back1Number { get; set; }

        public DateTime? Back1Date { get; set; }

        public string Back1Lampiran { get; set; }

        public Enums.DocumentStatusGov? Pbck7GovStatus { get; set; }

        public Enums.DocumentStatusGov Pbck7GovStatusList { get; set; }

        public Enums.DocumentStatusGov? Back1GovStatus { get; set; }

    }
}