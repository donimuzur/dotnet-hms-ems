using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseApprovedProduct
    {
        public long ExciseId { set; get; }
        public string ProductCode { set; get; }
        public string ProductAlias { set; get; }
        public decimal Amount { set; get; }
    }
}