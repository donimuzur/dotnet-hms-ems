using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack2Dto
    {
        public int Lack2Id { get; set; }
        public string Burks { get; set; }
        public string Butxt { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string LevelPlantId { get; set; }
        public string LevelPlantName { get; set; }
        public string LevelPlantCity { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string ExGoodTyp { get; set; }
        public string ExTypDesc { get; set; }
        public Enums.DocumentStatus GovStatus { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public DateTime? DecreeDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
