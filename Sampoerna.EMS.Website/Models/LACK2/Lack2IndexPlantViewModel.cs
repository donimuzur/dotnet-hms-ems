using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2IndexPlantViewModel : BaseModel
    {
        public Lack2IndexPlantViewModel()
        {
            Details = new List<LACK2PlantData>();
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

        public List<LACK2PlantData> Details { get; set; }

    }
}