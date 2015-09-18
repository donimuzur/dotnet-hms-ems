using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck4XmlDto
    {
        public string PbckNo { get; set; }
        public string NppbckId { get; set; }

        public string CompNo { get; set; }

        public string CompType { get; set; }

        public DateTime? CompnDate { get; set; }

        public string CompnValue { get; set; }

        public string DeleteFlag { get; set; }

        public string GeneratedXmlPath { get; set; }


    }
}
