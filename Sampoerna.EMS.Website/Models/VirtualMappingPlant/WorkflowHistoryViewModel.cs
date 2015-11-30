using System;
using System.ComponentModel.DataAnnotations;

namespace Sampoerna.EMS.Website.Models.WorkflowHistory
{
    public class WorkflowHistoryViewModel
    {
        public long WORKFLOW_HISTORY_ID { get; set; }
        public Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public string FORM_NUMBER { get; set; }
        public string ACTION { get; set; }
        public string ACTION_BY { get; set; }
        [UIHint("FormatDateTime")]
        public DateTime? ACTION_DATE { get; set; }
        public string COMMENT { get; set; }

        public string Role { get; set; }

        public string USERNAME { get; set; }
        public string USER_FIRST_NAME { get; set; }
        public string USER_LAST_NAME { get; set; }
    }
}