using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PBCK7
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
    }


    public class DataListIndexPbck7
    {
        public string Pbck7Number { get; set; }
        public DateTime ReportedOn { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string Poa { get; set; }
        public int Status { get; set; }
    }
    
}