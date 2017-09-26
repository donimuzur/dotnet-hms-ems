using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditSupportingDocument
    {
        public ExciseCreditSupportingDocument()
        {
            FileList = new List<FileUpload.FileUploadModel>();
        }
        public long Id { set; get; }
        public string Name { set; get; }
        public CompanyModel Company { set; get; }
        string CompanyId { set; get; }
        public HttpPostedFileBase File { set; get; }
        public List<FileUpload.FileUploadModel> FileList { set; get; }
    }
}