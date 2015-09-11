using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck4GetByParamInput
    {
        public string NppbkcId { get; set; }

        public string PlantId { get; set; }

        public DateTime? ReportedOn { get; set; }

        public string Poa { get; set; }

        public string Creator { get; set; }

        public string SortOrderColumn { get; set; }

        public bool IsCompletedDocument { get; set; }
    }

    public class Pbck4SaveInput
    {
        public Pbck4Dto Pbck4Dto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<Pbck4ItemDto> Pbck4Items { get; set; }
    }
}
