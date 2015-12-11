using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.GOODSTYPE
{
    public class GoodsTypeGroupDetailsViewModel : BaseModel
    {
        public GoodsTypeGroupDetailsViewModel()
        {
            GroupName = string.Empty;
            Details = new List<GoodsTypeDetails>();
        }
       
        public string GroupName { get; set; }

        public List<GoodsTypeDetails> Details { get; set; }
    }
}