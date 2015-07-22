using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.GOODSTYPE
{
    public class GoodsTypeGroupEditViewModel : BaseModel
    {
        public GoodsTypeGroupEditViewModel()
        {
            GroupName = string.Empty;
            Details = new List<GoodsTypeDetails>();
        }
        public int Id { get; set; }
        public string GroupName { get; set; }

        public List<GoodsTypeDetails> Details { get; set; }
       
    }
}