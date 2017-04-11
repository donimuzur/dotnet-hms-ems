using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class TableDetail
    {
        public string PropertyName { get; set; }
        public string TypeUsageName { get; set; }
        public Documentation Documentation { get; set; }
    }
}
