using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class CompanyModel
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string Alias { set; get; }
        public string Npwp { set; get; }
        public string City { set; get; }
        public string Address { set; get; }
    }
}