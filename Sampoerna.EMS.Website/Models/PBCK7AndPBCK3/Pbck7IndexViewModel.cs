using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK7AndPBCK3
{
    public class Pbck7IndexViewModel : BaseModel
    {
        public Pbck7IndexViewModel()
        {
            Detail = new List<DataListIndexPbck7>();
        }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string ReportedOn { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }

        public SelectList NppbkcList { get; set; }
        public SelectList PlantList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList CreatorList { get; set; }
        public List<DataListIndexPbck7> Detail { get; set; }

        //Enums
        public Enums.Pbck7Type Pbck7Type { get; set; }
    }
    
    public class DataListIndexPbck7
    {
        public string Pbck7Number { get; set; }
        public string ReportedOn { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string Poa { get; set; }
        public int Status { get; set; }
    }

  
    
}