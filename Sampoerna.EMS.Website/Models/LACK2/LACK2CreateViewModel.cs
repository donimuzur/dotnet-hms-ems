using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class LACK2CreateViewModel : BaseModel
    {
        public SelectList CompanyCodesDDL { get; set; }
        [Required]
        [Display(Name = "Company Code")]
        public string SelectedCompanyCode { get; set; }
        public SelectList NPPBKCDDL { get; set; }
        [Required]
        [Display(Name = "NPPBKC")]
        public string SelectedNPPBKC { get; set; }
        public SelectList SendingPlantDDL { get; set; }
        [Required]
        [Display(Name = "Plant")]
        public string SelectedPlant { get; set; }
        public SelectList ExcisableGoodsTypeDDL { get; set; }
        [Required]
        [Display(Name="Ex Goods Type")]
        public string SelectedExGoodsType { get; set; }
        [Required]
        [Display(Name = "LACK2 Period")]
        public DateTime LACK2Period { get; set; }
        [Required]
        [Display(Name = "Submmision Date")]
        public DateTime SubmissionDate { get; set; }
    }
}