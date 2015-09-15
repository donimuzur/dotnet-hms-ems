using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class PBCK4_DOCUMENTDto
    {
        public long PBCK4_DOCUMENT_ID { get; set; }
        public int DOC_TYPE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public int PBCK4_ID { get; set; }
    }
}
