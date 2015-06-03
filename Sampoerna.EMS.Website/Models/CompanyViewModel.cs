using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models
{
    public class CompanyViewModel 
    {
        public long CompanyId { get; set; }
        public string DocumentBukrs { get; set; }
        public string DocumentBukrstxt { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public virtual ICollection<T1001K> T1001K { get; set; }
       
    }
}