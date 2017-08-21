using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment
{
    public class ProductDevSupportingDocumentModel
    {
        public ProductDevSupportingDocumentModel()
        {
            FileList = new List<FileUpload.FileUploadModel>();
            FileListUpload = new List<ProductDevUpload.ProductDevelopmentUploadModel>();
        }
        public long Id { set; get; }
        public string Name { set; get; }
        public CompanyModel Company { set; get; }
        public HttpPostedFileBase File { set; get; }
        public List<FileUpload.FileUploadModel> FileList { set; get; }
        public List<ProductDevUpload.ProductDevelopmentUploadModel> FileListUpload { get; set; }
    }
}