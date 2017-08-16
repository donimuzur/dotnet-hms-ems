using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment
{
    public class ProductDevFilterViewModel
    {
        public ProductDevFilterViewModel()
        {
            Creator = null;
            NextAction = null;
            BrandRegStatus = null;
        }
        public string Creator { get; set; }
        public int? NextAction { get; set; }
        public int? BrandRegStatus { get; set; }

        public SelectList CreatorList { get; set; }        
        public IEnumerable<SelectListItem> ListAction { get; set; }

        public string SortOrderColumn { get; set; }
    }

    public class PDGetSummaryReportByParamInput
    {
        //public string NppbkcId { get; set; }
        public string CompanyCode { get; set; }
        //public int? YearFrom { get; set; }
        //public int? YearTo { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public string PDNumber { get; set; }
        public Enums.UserRole UserRole { get; set; }
        //public List<string> ListNppbkc { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
    }
}