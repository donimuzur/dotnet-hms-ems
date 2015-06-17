using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5IndexViewModel : BaseModel
    {

        public CK5IndexViewModel()
        {
            SearchView = new CK5SearchViewModel();
            DetailsList = new List<CK5Item>();

        }
        public CK5SearchViewModel SearchView { get; set; }

        public List<CK5Item> DetailsList { get; set; }
        
    }

    public class CK5Item
    {
        public string DocumentNumber { get; set; }

        public int Qty { get; set; }

        public string UOM { get; set; }

        public string POA { get; set; }

        public long? NPPBKC_ID { get; set; }

        public string SourcePlant { get; set; }

        public string DestinationPlant { get; set; }

        public string Status { get; set; }


    }
}