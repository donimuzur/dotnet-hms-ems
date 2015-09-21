using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.Waste
{
    public class WasteUploadViewModel : BaseModel
    {
        public List<WasteUploadItems> UploadItems { get; set; }
    }


}