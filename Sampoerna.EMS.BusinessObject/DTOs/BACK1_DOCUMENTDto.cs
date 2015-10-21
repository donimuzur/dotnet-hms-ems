using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class BACK1_DOCUMENTDto
    {
        public long BACK1_DOCUMENT_ID { get; set; }
        public int BACK1 { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public bool IsDeleted { get; set; }
    }
}
