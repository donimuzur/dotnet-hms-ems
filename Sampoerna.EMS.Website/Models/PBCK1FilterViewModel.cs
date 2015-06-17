using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models
{
    public class PBCK1FilterViewModel
    {
        public PBCK1FilterViewModel()
        {
            NPBCKID = string.Empty;
            POA = null;
            Pbck1Type = string.Empty;
            Creator = null;
            Year = null;
        }
        public string NPBCKID { get; set; }
        public int? POA { get; set; }
        public string Pbck1Type { get; set; }
        public int? Creator { get; set; }
        public int? Year { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public SelectList NPPBKCIDList { get; set; }
        public SelectList POAList { get; set; }
        public Enums.PBCK1Type PBCK1Types { get; set; }
        public SelectList CreatorList { get; set; }
        public SelectList YearList { get; set; }
    }
}