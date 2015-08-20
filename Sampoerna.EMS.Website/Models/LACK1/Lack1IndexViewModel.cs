using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1IndexViewModel : BaseModel
    {
        public Lack1IndexViewModel()
        {
              Details = new List<NppbkcData>();
        }
        public string NppbkcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        public string ReportedOn { get; set; }
        public string Creator { get; set; }

        public SelectList NppbkcIdList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList PlantIdList { get; set; }
        public SelectList ReportedOnList { get; set; }
        public SelectList CreatorList { get; set;  }

        public Enums.LACK1Type Lack1Type { get; set; }

        public List<NppbkcData> Details { get; set; }
    }
}