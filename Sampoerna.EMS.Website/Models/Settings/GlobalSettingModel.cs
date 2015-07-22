using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Settings
{
    public class GlobalSettingModel : BaseModel
    {
        [Required, Display(Name="Upload File Size")]
        public int UploadFileSize { get; set; }

        public FileSizeType FileSizeType { get; set; }

        public SelectList SizeType { get; set; }

        [Required, Display(Name="Use BackDate")]
        public bool UseBackDate { get; set; }
        

    }

    public enum FileSizeType { 
        MB,
        KB
    }
}