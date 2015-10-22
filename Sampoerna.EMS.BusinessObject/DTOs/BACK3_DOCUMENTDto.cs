using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class BACK3_DOCUMENTDto
    {
        public long BACK3_DOC_ID { get; set; }
        public string FILE_PATH { get; set; }
        public string FILE_NAME { get; set; }
        public int BACK3_ID { get; set; }
        public bool IsDeleted { get; set; }
    }
}
