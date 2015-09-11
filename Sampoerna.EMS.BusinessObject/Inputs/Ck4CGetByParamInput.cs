using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Ck4CGetByParamInput
    {
        public string DateProduction { get; set; }
        public string CompanyId { get; set; }
        public string Company { get; set; }
        public string PlantId { get; set; }
        public string DocumentNumber { get; set; }
        public string NppbkcId { get; set; }
        public string ShortOrderColumn { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }

    }

    public class SaveCk4CInput
    {
        public Ck4CDto Lack1 { get; set; }
    }

    public class Ck4cWorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }
    }

}
