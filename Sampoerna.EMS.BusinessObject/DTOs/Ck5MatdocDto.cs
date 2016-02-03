using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Ck5MatdocDto
    {
        public string MatDoc { get; set; }
        public decimal Qty { get; set; }
        public string Bun { get; set; }
        public string PostingDate { get; set; }
    }
}
