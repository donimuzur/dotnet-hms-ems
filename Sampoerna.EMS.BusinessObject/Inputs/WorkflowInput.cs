using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WorkflowAllowApproveAndRejectInput
    {
       public Enums.UserRole UserRole { get; set; }
       public Enums.FormViewType FormView { get; set; }
       public Enums.DocumentStatus DocumentStatus { get; set; }
       public string CreatedUser { get; set; }
       public string CurrentUser { get; set; }
       public string CurrentUserGroup { get; set; }
    }

    public class WorkflowAllowEditAndSubmitInput
    {
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public string CreatedUser { get; set; }
        public string CurrentUser { get; set; }
    }
}
