using System;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class WorkflowHistoryDto
    {
        public long WORKFLOW_HISTORY_ID { get; set; }
        public Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public long FORM_ID { get; set; }
        public string FORM_NUMBER { get; set; }
        public Core.Enums.ActionType ACTION { get; set; }
        public string ACTION_BY { get; set; }
        public DateTime? ACTION_DATE { get; set; }
        public string COMMENT { get; set; }
        public Enums.UserRole ROLE { get; set; }
        //public USER USER { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
