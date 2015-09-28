using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck7AndPbck3Input
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string Pbck7Date { get; set; }
        public string Pbck3Date { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string ShortOrderColum { get; set; }
        public Enums.Pbck7Type Pbck7AndPvck3Type { get; set; }
      


    }


    public class Pbck7SummaryInput
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public DateTime? Pbck7DateFrom { get; set; }
        public DateTime? Pbck7DateTo { get; set; }
        public string Pbck7Number { get; set; }
        public string ShortOrderColum { get; set; }
       




    }

}
