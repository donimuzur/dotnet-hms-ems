using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ProductDevUpload
{
    public class ProductDevelopmentUploadModel : BaseModel
    {
        public long File_ID { get; set; }
        public long PD_Detail_ID { get; set; }
        public int? Item_ID { get; set; }
        public string Path_Url { get; set; }
        public DateTime? Upload_Date { get; set; }
        public long Document_ID { get; set; }
        public bool Is_Active { get; set; }
        public string File_Name { get; set; }
    }
}