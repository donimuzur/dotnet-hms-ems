using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WorkflowAllowApproveAndRejectInput
    {
       public Enums.UserRole UserRole { get; set; }
       public Enums.FormViewType FormView { get; set; }
       public Enums.DocumentStatus DocumentStatus { get; set; }
       public int CreatedUser { get; set; }
       public int CurrentUser { get; set; }
       public int CurrentUserGroup { get; set; }
    }

    public class WorkflowAllowEditAndSubmitInput
    {
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public int? CreatedUser { get; set; }
        public int CurrentUser { get; set; }
    }
}
