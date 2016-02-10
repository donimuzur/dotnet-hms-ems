using System.Collections.Generic;
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
       public string PlantId { get; set; }
       public string SourcePlant { get; set; }
       public string DestPlant { get; set; }
       public string DestNppbkcId { get; set; }
    }

    public class WorkflowAllowEditAndSubmitInput
    {
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public string CreatedUser { get; set; }
        public string CurrentUser { get; set; }
    }


    public class WorkflowAllowAccessDataInput
    {
        public List<string> UserPlant { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string UserId { get; set; }

        public string DataPlant { get; set; }
        public string DataUser { get; set; }
    }
}
