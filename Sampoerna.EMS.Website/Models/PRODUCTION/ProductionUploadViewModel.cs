using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.PRODUCTION
{
    public class ProductionUploadViewModel : BaseModel
    {
        public List<ProductionUploadItems> UploadItems { get; set; }
    }


}