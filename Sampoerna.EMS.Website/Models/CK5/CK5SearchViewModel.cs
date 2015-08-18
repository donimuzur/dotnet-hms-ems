using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5SearchViewModel
    {


        public CK5SearchViewModel()
        {
            
        }

        [Display(Name = "Doc. No")]
        public string DocumentNumber { get; set; }

        [Display(Name = "POA")]
        public string POA { get; set; }

        [Display(Name = "NPPBKC Origin")]
        public string NPPBKCOrigin { get; set; }

        [Display(Name = "NPPBKC Destination")]
        public string NPPBKCDestination { get; set; }

        [Display(Name = "Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public SelectList DocumentNumberList { get; set; }
        public SelectList POAList { get; set; }
        public SelectList NPPBKCOriginList { get; set; }
        public SelectList NPPBKCDestinationList { get; set; }
        public SelectList CreatorList { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }

    }
}