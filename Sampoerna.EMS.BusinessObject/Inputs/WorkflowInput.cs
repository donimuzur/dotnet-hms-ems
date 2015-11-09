using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WorkflowAllowApproveAndRejectInput
    {
       public Enums.UserRole UserRole { get; set; }
       public Enums.FormViewType FormView { get; set; }
       public Enums.FormType FormType { get; set; }
       public Enums.DocumentStatus DocumentStatus { get; set; }
       public string CreatedUser { get; set; }
       public string CurrentUser { get; set; }
       public string CurrentUserGroup { get; set; }
       public string DocumentNumber { get; set; }
       public string NppbkcId { get; set; }
       public string ManagerApprove { get; set; }
       public string PoaApprove { get; set; }
       public Enums.Ck5ManualType Ck5ManualType { get; set; }
        //for modul pbck3
       public string DocumentNumberSource { get; set; }
    }

    public class WorkflowAllowEditAndSubmitInput
    {
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public string CreatedUser { get; set; }
        public string CurrentUser { get; set; }
    }
}
