using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ProductType
{
    public class ProductTypeIndexViewModel : BaseModel
    {
        public ProductTypeIndexViewModel()
        {
            ListProductTypes = new List<ProductTypeFormViewModel>();
        }
        public List<ProductTypeFormViewModel> ListProductTypes { get; set; }
    }
}