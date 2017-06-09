using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP
{
    public class SKEPFilterViewModel
    {
        public SKEPFilterViewModel()
        {
            NppbkcId = string.Empty;
            Creator = null;
        }

        public string NppbkcId { get; set; }
        public string Creator { get; set; }
        public long Status { get; set; }
        public string SortOrderColumn { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public SelectList CreatorList { get; set; }

    }
}