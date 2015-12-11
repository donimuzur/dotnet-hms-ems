using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Sampoerna.EMS.Website.Models.NPPBKC;

namespace Sampoerna.EMS.Website.Models.GOODSTYPE
{
    public class GoodsTypeGroupViewModel : BaseModel
    {
        public GoodsTypeGroupViewModel()
        {
            Details = new List<DetailsGoodsTypGroup>();
        }
        public List<DetailsGoodsTypGroup> Details;
      

    }
    public class DetailsGoodsTypGroup
    {
        public int GoodsTypeId { get; set; }

        public string GroupName { get; set; }

        public string GroupTypeName { get; set; }

        public string Inactive { get; set; }

    }
}