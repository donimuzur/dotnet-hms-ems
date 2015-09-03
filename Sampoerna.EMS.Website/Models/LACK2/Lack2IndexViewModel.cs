using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2IndexViewModel : BaseModel
    {
        public Lack2IndexViewModel()
        {
              Details = new List<LACK2NppbkcData>();
              SearchInput = new LACK2FilterViewModel();
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

        //public Enums.LACK1Type Lack1Type { get; set; }

        public List<LACK2NppbkcData> Details { get; set; }

        // for the completed documents
        public LACK2FilterViewModel SearchInput { get; set; }

        
    }
}