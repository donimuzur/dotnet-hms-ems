using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP
{
    public class SKEPSupportingDocumentModel
    {
        public SKEPSupportingDocumentModel()
        {
            FileList = new List<FileUpload.FileUploadModel>();
        }
        public long Id { set; get; }
        public long FileUploadId { set; get; }
        public string Name { set; get; }
        public string FileName { set; get; }
        public string Path { set; get; }
        public bool IsEnable { set; get; }
        public CompanyModel Company { set; get; }
        public HttpPostedFileBase File { set; get; }
        public List<FileUpload.FileUploadModel> FileList { set; get; }
    }
}