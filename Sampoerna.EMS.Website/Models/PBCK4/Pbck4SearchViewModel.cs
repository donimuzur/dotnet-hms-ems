using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PBCK4
{
    public class Pbck4SearchViewModel
    {
        public Pbck4SearchViewModel()
        {
            
        }

        [Display(Name = "NPPBKC ID")]
        public string NppbkcId { get; set; }

        [Display(Name = "POA")]
        public string Poa { get; set; }

        [Display(Name = "Plant ID")]
        public string PlantId { get; set; }
     
        [Display(Name = "Creator")]
        public string Creator { get; set; }

        [Display(Name = "Reported On")]
        public DateTime? ReportedOn { get; set; }


        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public SelectList NppbkcIdList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList PlantIdList { get; set; }
        public SelectList CreatorList { get; set; }

      
    }
}