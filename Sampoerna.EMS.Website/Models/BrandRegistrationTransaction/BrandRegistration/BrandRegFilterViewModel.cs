using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration
{
    public class BrandRegFilterViewModel
    {
        public BrandRegFilterViewModel()
        {
            NppbkcId = string.Empty;
            Creator = null;
            RegistrationType = null;
        }
        public int? RegistrationType { get; set; }
        public string NppbkcId { get; set; }
        public string Creator { get; set; }
        public string SortOrderColumn { get; set; }
        public SelectList CreatorList { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public IEnumerable<SelectListItem> ListRegistrationType { get; set; }
    }
}