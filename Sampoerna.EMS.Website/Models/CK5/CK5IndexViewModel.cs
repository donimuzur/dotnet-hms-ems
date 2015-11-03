using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

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

        public List<CK5Item> DetailList2 { get; set; }

        public List<CK5Item> DetailList3 { get; set; } 

        public Enums.CK5Type Ck5Type { get; set; }

        public bool IsCompletedType { get; set; }
    }

    public class CK5Item
    {
        public long Ck5Id { get; set; }

        public string DocumentNumber { get; set; }

        public string Qty { get; set; }

        public string QtyPackaging { get; set; }

        public string UOM { get; set; }

        public string POA { get; set; }

        public long? NPPBKC_ID { get; set; }

        public string SourcePlant { get; set; }

        public string DestinationPlant { get; set; }

        public string Status { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }

        public decimal GRAND_TOTAL_EX { get; set; }

        public string PACKAGE_UOM_ID { get; set; }
    }
}