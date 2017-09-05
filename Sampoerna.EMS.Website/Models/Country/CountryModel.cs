using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Country
{
    public class CountryModel : BaseModel
    {
        public long CountryID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }
}