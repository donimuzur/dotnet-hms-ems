using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK7AndPBCK3
{
    public class Pbck7SummaryReportModel : BaseModel
    {
        public Pbck7SummaryReportModel()
        {
            ReportItems = new List<Pbck7AndPbck3Dto>();
        }
        public string SelectedNumber { get; set; }
        public string SelectedNppbkc { get; set; }
        public string SelectedPlant { get; set; }
      
        public SelectList Pbck7List { get; set; }

        public SelectList PlantList { get; set; }

        public SelectList NppbkcList { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public List<Pbck7AndPbck3Dto> ReportItems { get; set; }
    }

    public class Pbck3Pbck7SummaryReportDetailModel
    {
    }

    public class Pbck7SummaryReportItem
    {
        public  string Pbck7Number { get; set; }
        public DateTime? Pbck7Date { get; set; }

       
        public string PlantName { get; set; }

        public Enums.DocumentTypePbck7AndPbck3 DocumentType { get; set; }

        public string Nppbkc { get; set; }

      
        public Enums.DocumentStatus Pbck3Status { get; set; }
    }
}