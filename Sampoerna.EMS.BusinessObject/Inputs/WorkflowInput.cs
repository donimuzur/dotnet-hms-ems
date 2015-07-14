using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WorkflowIsAllowedInput
    {
       public Enums.UserRole UserRole { get; set; }
       public Enums.FormViewType FormView { get; set; }
       public Enums.DocumentStatus DocumentStatus { get; set; } 
    }
}
