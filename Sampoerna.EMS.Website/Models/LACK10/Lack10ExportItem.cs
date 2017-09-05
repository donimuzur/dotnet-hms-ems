using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.LACK10
{
    public class Lack10ExportItem : BaseModel
    {
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public string Type { get; set; }
        public Decimal WasteValue { get; set; }
    }

    public class Lack10Export
    {
        public string CompanyName { get; set; }
        public string Nppbkc { get; set; }
        public string CompanyAddress { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }

        public List<Lack10ExportItem> ItemList { get; set; }
    }
}