using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1IndexPlantViewModel : BaseModel
    {
        public Lack1IndexPlantViewModel()
        {
            Details = new List<Lack1PlantData>();
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
        public SelectList CreatorList { get; set; }

        public List<Lack1PlantData> Details { get; set; }

    }
}