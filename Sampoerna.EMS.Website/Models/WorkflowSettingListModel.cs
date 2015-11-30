using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.WorkflowSetting
{
    public class WorkflowSettingListModel : BaseModel
    {
        public WorkflowSettingListModel() {
            Details = new List<WorkflowDetails>();
            
        }
        public List<WorkflowDetails> Details { get; set; }
        
    }

    public class WorkflowDetails : BaseModel {

        public WorkflowDetails() {
            Details = new List<WorkflowMappingDetails>();
        }

        public string Modul { get; set; }

        public int Form_Id { get; set; }

        public List<WorkflowMappingDetails> Details { get; set; }
    }

    public class WorkflowMappingDetails {
        public string StateMappingId { get; set; }
        public string State { get; set; }

        public int EmailTemplateId { get; set; }
        public string EmailTemplateName { get; set; }

        public List<WorkflowUsers> ListUser { get; set; }
        public string SentTo { get; set; }

        public string Modul { get; set; }

        public int Form_Id { get; set; }

        public SelectList EmailTemplateList { get; set; }
        public SelectList UserSelectList { get; set; }
    }

    public class WorkflowUsers{
        public string User_Id;
        public string Email;
    }
}