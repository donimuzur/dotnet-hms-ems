using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.PBCK4
{
    public class Pbck4FileUploadViewModel
    {
        public long PBCK4_DOCUMENT_ID { get; set; }
        public int DOC_TYPE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public int PBCK4_ID { get; set; }
    }
}