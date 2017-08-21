using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class EmailVariable
    {
        public long Id { set; get; }
        public string Name { set; get; }
        public string Notes { set; get; }
        public long ? ContentID { set; get; }
    }
}