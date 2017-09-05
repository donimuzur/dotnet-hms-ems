using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditNppbkc
    {
        public ExciseCreditNppbkc()
        {
            this.Company = new CompanyModel();
        }
        public string Id { set; get; }
        public string KppbcId { set; get; }
        public string City { set; get; }
        public string CityAlias { set; get; }
        public string Region { set; get; }
        public string Address { set; get; }
        public CompanyModel Company { set; get; }
    }
}