using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK7
{
    public class Pbck3IndexViewModel : BaseModel
    {
        public Pbck3IndexViewModel()
        {
            Detail = new List<DataListIndexPbck3>();
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
        public List<DataListIndexPbck3> Detail { get; set; }

        //Enums
        public Enums.Pbck7Type Pbck7Type { get; set; }
    }

    public class DataListIndexPbck3
    {
        public string Pbck3Number { get; set; }
        public DateTime ReportedOn { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string Poa { get; set; }
        public int Status { get; set; }
    }
}