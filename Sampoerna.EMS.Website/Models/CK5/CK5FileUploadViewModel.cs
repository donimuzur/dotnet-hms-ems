using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5FileUploadViewModel
    {
        public long CK5_FILE_UPLOAD_ID { get; set; }
        public long CK5_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    }
}