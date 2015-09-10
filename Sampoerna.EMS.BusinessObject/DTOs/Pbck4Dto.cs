using System;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck4Dto
    {
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string NppbkcId { get; set; }
        public DateTime? ReportedOn { get; set; }
        public string Poa { get; set; }
        public Enums.DocumentStatus Status { get; set; }
    }
}
