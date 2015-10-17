using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2ItemModel
    {
        public int Lack2Id { get; set; }

        public string Lack2Number { get; set; }

        public string Burks { get; set; }

        public string Butxt { get; set; }

        public string NppbkcId { get; set; }

        public string LevelPlantId { get; set; }

        public string LevelPlantName { get; set; }

        public string LevelPlantCity { get; set; }

        public string ExGoodTyp { get; set; }

        public string ExGoodDesc { get; set; }

        public Enums.DocumentStatusGov? StatusGov { get; set; }

        public Enums.DocumentStatus Status { get; set; }
        
        public DateTime? DecreeDate { get; set; }
        
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public List<Lack2ItemDto> Items { get; set; }

        public string StatusName { get; set; }

        public string PeriodMonthName { get; set; }

        public string Comment { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedDateManager { get; set; }

        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }

        public List<Lack2DocumentDto> Documents { get; set; } 

    }
}