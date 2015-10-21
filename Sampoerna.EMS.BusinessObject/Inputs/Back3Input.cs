using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class SaveBack3ByPbck3IdInput
    {
        public int Pbck3Id { get; set; }

        public string Back3Number { get; set; }
        public DateTime? Back3Date { get; set; }

        public List<BACK3_DOCUMENTDto> Back3Documents { get; set; }
    }
}
