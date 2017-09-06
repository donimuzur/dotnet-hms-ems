using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack10SummaryReportDto
    {
        public string Lack10No { get; set; }
        public string CeOffice { get; set; }
        public string BasedOn { get; set; }
        public string LicenseNumber { get; set; }
        public string Kppbc { get; set; }
        public string SubmissionDate { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string PoaApproved { get; set; }
        public string ManagerApproved { get; set; }
        public string Status { get; set; }
        public string CompletedDate { get; set; }
        public string Creator { get; set; }

        public List<string> FaCode { get; set; }
        public List<string> BrandDesc { get; set; }
        public List<string> Werks { get; set; }
        public List<string> PlantName { get; set; }
        public List<string> Type { get; set; }
        public List<string> WasteValue { get; set; }
        public List<string> Uom { get; set; }
    }
}
