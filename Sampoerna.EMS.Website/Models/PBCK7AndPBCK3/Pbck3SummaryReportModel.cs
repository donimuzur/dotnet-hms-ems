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
    public class Pbck3SummaryReportModel : BaseModel
    {
        public Pbck3SummaryReportModel()
        {
            ReportItems = new List<Pbck3Dto>();
            ExportModel = new Pbck3ExportModel();
        }
        public string SelectedNumber { get; set; }
        public string SelectedNppbkc { get; set; }
        public string SelectedPlant { get; set; }
      
        public SelectList Pbck3List { get; set; }

        public SelectList PlantList { get; set; }

        public SelectList NppbkcList { get; set; }

        public SelectList FromYear { get; set; }

        public SelectList ToYear { get; set; }

        public int?  From { get; set; }

        public int? To { get; set; }

        public List<Pbck3Dto> ReportItems { get; set; }

        public Pbck3ExportModel ExportModel { get; set; }
    }

   

    public  class Pbck3ExportModel
    {
        public string NppbkcId { get; set; }

        public string Pbck7No { get; set; }

        public string Plant { get; set; }

        public Enums.DocumentTypePbck7AndPbck3? DocType { get; set; }

        public Enums.DocumentStatus Status { get; set; }

       

        public int? FromYear { get; set; }

        public int? ToYear { get; set; }

        public bool IsSelectNppbkc { get; set; }

        public bool IsSelectPbck3No { get; set; }

     
        public bool IsSelectPlant { get; set; }

        public bool IsSelectDocType { get; set; }
        public bool IsSelectPbck7 { get; set; }

        public bool IsSelectStatus { get; set; }

        public bool IsSelectDate { get; set; }
        public bool IsSelectBack3No { get; set; }
        public bool IsSelectBack3Date { get; set; }

        public bool IsSelectCk2No { get; set; }
        public bool IsSelectCk2Date { get; set; }
        public bool IsSelectCk2Value { get; set; }
    }

    public class Pbck3SummaryReportItem
    {
        public string Pbck3Number { get; set; }
        
        public  string Pbck7Number { get; set; }
        public DateTime? Pbck3Date { get; set; }

       
        public string PlantName { get; set; }

        public string DocumentType { get; set; }

        public string Nppbkc { get; set; }

        
      
        public string Pbck3Status { get; set; }

        public string Back3No { get; set; }

        public DateTime? Back3Date { get; set; }

        public string Ck2No { get; set; }

        public DateTime? Ck2Date { get; set; }

        public Decimal? Ck2Value { get; set; }
    }
}