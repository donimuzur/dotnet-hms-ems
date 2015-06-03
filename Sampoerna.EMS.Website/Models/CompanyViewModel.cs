using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models
{
    public class CompanyViewModel
    {
        public long COMPANY_ID { get; set; }
        public string BUKRS { get; set; }
        public string BUKRSTXT { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }

        public virtual ICollection<T1001K> T1001K { get; set; }
    }
}