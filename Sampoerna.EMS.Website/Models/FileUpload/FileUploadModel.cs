using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.FileUpload
{
    public class FileUploadModel : BaseModel
    {
        public long FileID { get; set; }
        public int FormTypeID { get; set; }
        public string FileName { get; set; }
        public string FormID { get; set; }
        public string PathURL { get; set; }
        public DateTime UploadDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long DocumentID { get; set; }
        public bool IsGovernmentDoc { get; set; }
        public bool IsActive { get; set; }
    }
}