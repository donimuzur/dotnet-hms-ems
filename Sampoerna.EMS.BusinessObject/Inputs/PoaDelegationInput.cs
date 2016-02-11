using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class PoaDelegationSaveInput
    {
        public POA_DELEGATIONDto PoaDelegationDto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class GetCommentDelegateForWorkflowInput
    {
        public long FormId { get; set; }
        public Enums.FormType FormType { get; set; }

    }

    public class GetEmailDelegateUserInput
    {
        public Enums.FormType FormType { get; set; }
        public long FormId { get; set; }
        public string FormNumber { get; set; }
        public Enums.ActionType ActionType { get; set; }

        public string CurrentUser { get; set; }
        public string CreatedUser { get; set; }
        public DateTime Date { get; set; }

        public WorkflowHistoryDto WorkflowHistoryDto { get; set; }
        public List<string> UserApprovedPoa { get; set; }

    }
}
