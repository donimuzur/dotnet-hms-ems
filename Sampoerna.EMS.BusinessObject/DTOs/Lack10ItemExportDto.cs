using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack10ItemExportDto
    {
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public string Type { get; set; }
        public Decimal WasteValue { get; set; }
    }

    public class Lack10ExportDto
    {
        public string CompanyName { get; set; }
        public string Nppbkc { get; set; }
        public string CompanyAddress { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }

        public List<Lack10ItemExportDto> ItemList { get; set; }
    }
}
