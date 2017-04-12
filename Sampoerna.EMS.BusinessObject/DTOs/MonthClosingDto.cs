using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class MonthClosingDto
    {
        public int MonthClosingId { get; set; }
        public string PlantId { get; set; }
        public string ClosingDay { get; set; }
        public string ClosingMonth { get; set; }
        public string ClosingYear { get; set; }
        public DateTime ClosingDate { get; set; }

        public List<MonthClosingDocDto> MonthClosingDoc { get; set; }
    }
}
