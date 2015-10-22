using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class CK2_DOCUMENTDto
    {
        public long CK2_DOC_ID { get; set; }
        public string FILE_PATH { get; set; }
        public string FILE_NAME { get; set; }
        public int CK2_ID { get; set; }
        public bool IsDeleted { get; set; }
    }
}
