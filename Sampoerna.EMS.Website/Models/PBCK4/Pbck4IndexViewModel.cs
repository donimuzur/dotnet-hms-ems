using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.PBCK4
{
    public class Pbck4IndexViewModel : BaseModel
    {

        public Pbck4IndexViewModel()
        {
            SearchView = new Pbck4SearchViewModel();
            DetailsList = new List<Pbck4Item>();

        }

        public Pbck4SearchViewModel SearchView { get; set; }

        public List<Pbck4Item> DetailsList { get; set; }
        
        public bool IsCompletedType { get; set; }
    }

    public class Pbck4Item
    {
        public int PbckId { get; set; }

        public string PlantId { get; set; }

        public string PlantDescription { get; set; }

        public string NppbkcId { get; set; }

        public string ReportedOn { get; set; }

        public string Poa { get; set; }
        
        public string Status { get; set; }
        
    }
}