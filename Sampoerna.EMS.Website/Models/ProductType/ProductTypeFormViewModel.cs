using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ProductType
{
    public class ProductTypeFormViewModel : BaseModel
    {
        public string ProdCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public string ModifiedBy { get; set; }
        public string IsDeleted { get; set; }
    }
}