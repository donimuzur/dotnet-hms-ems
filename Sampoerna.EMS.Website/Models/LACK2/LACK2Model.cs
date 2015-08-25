using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class LACK2Model
    {
        public int Lack2Id { get; set; }

        public string Lack2Number { get; set; }

        [Required]
        [Display(Name = "Company Code")]
        public string Burks { get; set; }

        public string Butxt { get; set; }

        [Required]
        [Display(Name = "NPPBKC")]
        public string NppbkcId { get; set; }

        [Required]
        [Display(Name = "Plant")]
        public string LevelPlantId { get; set; }

        public string LevelPlantName { get; set; }

        public string LevelPlantCity { get; set; }

        [Required]
        [Display(Name = "Ex Goods Type")]
        public string ExGoodTyp { get; set; }

        public string ExGoodDesc { get; set; }

        public int GovStatus { get; set; }

        public int Status { get; set; }

        [Display(Name = "Decree Date")]
        public DateTime? DecreeDate { get; set; }

        [Required]
        [Display(Name = "LACK2 Period")]
        public DateTime LACK2Period { get; set; }

        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }

        [Required]
        [Display(Name = "Submmision Date")]
        public DateTime SubmissionDate { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

    }
}