using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class LACK2FilterViewModel
    {
        

        public string NppbkcId { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }

        public string PlantId { get; set; }
        public int? Year { get; set; }

        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public SelectList NppbkcIdList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList CreatorList { get; set; }
        public SelectList PlantList { get; set; }
        public SelectList YearList { get; set; }


    }
}