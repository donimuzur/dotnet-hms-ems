﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Math;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack2IndexPlantViewModel : BaseModel
    {
        public Lack2IndexPlantViewModel()
        {
            Details = new List<PlantData>();
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

        //public Enums.LACKType Lack2Type { get; set; }

        public List<PlantData> Details { get; set; }

    }
}