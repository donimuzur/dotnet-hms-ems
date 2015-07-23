using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5ListViewModel : BaseModel   
    {
        public CK5ListViewModel()
        {
            SearchView = new CK5SearchViewModel();
            DetailList = new List<CK5ItemModel>();
            DetailList2 = new List<CK5ItemModel>();        
        }

        public CK5SearchViewModel SearchView { get; set; }

        public List<CK5ItemModel> DetailList { get; set; }

        public List<CK5ItemModel> DetailList2 { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }
    }
}