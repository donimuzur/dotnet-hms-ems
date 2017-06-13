using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.MonthClosing
{
    public class MonthClosingIndexViewModel : BaseModel
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public SelectList MonthList { get; set; }

        public List<MonthClosingDetail> MonthClosingList { get; set; }

        public MonthClosingDetail Details { get; set; }
    }

    public class MonthClosingDetail
    {
        public string MonthClosingId { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string ClosingDay { get; set; }
        public string ClosingMonth { get; set; }
        public string ClosingYear { get; set; }

        [Required]
        public DateTime ClosingDate { get; set; }
        public string DisplayDate { get; set; }
        public string IsActive { get; set; }

        public List<HttpPostedFileBase> MonthClosingFiles { get; set; }
        public List<MonthClosingDocModel> MonthClosingDoc { get; set; }
    }
}