using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class PrintoutLayout
    {
        public Int64 Id { set; get; }
        public Int32 LayoutId { set; get; }
        public String Name { set; get; }
        public String Layout { set; get; }
        public String DefaultLayout { set; get; }
        public String CompleteLayout { set; get; }
        public String User { set; get; }
    }
}