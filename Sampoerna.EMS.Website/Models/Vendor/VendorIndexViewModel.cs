using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.Vendor
{
    public class VendorIndexViewModel : BaseModel
    {
        public List<LFA1Dto> Details { get; set; }
    }
}