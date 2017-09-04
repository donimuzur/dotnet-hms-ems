using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class CityModel
    {
        public long Id { set; get; }
        public string Name { set; get; }
        public long StateId { set; get; }
        public string StateName { set; get; }
    }
}