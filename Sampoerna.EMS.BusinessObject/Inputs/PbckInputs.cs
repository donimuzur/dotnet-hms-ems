using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
