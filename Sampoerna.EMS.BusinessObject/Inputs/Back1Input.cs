using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class SaveBack1ByCk5IdInput
    {
        public long Ck5Id { get; set; }
        public string Back1Number { get; set; }
        public DateTime Back1Date { get; set; }

        public List<BACK1_DOCUMENT> Back1Documents { get; set; }
    }

    public class SaveBack1ByPbck7IdInput
    {
        public int Pbck7Id { get; set; }
        public string Back1Number { get; set; }
        public DateTime Back1Date { get; set; }

        public List<BACK1_DOCUMENT> Back1Documents { get; set; }
    }
}
