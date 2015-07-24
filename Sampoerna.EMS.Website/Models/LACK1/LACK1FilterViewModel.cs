using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class LACK1FilterViewModel
    {
        public LACK1FilterViewModel()
        {
            NppbkcId = string.Empty;
            Poa = null;
            PlantId = null;
            ReportedOn = null;
            Creator = null;
        }

        public string NppbkcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        public int? ReportedOn { get; set; }
        public string Creator { get; set; }

        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public SelectList NppbkcIdList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList PlantIdList { get; set; }
        public SelectList ReportedOnList { get; set; }
        public SelectList CreatorList { get; set;  }

        public Enums.LACK1Type Lack1Type { get; set; }

    }
}