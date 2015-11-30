using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Back1Dto
    {
        public int Back1Id { get; set; }
        public string Back1Number { get; set; }
        public DateTime? Back1Date { get; set; }

        public List<BACK1_DOCUMENTDto> Documents { get; set; } 
        public int Pbck7Id { get; set; }
    }
}
