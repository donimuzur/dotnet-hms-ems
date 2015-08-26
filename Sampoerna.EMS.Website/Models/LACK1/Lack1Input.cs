using System;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1Input
    {
        public string NppbKcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        public string ReportedOn { get; set; }
        //public int? PeriodMonth { get; set; }
        //public int? PeriodYear { get; set; }
        public string Creator { get; set; }
        public DateTime? SubmissionDate { get; set; }

    }

}