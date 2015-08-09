using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class ExGoodTyp
    {
        public ExGoodTyp()
        {
           ZAIDM_EX_GOODTYP = new List<ZAIDM_EX_GOODTYP>();
        }
        public int EX_GROUP_TYPE_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public List<ZAIDM_EX_GOODTYP> ZAIDM_EX_GOODTYP { get; set; }
    }
}
