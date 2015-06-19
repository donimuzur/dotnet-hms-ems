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
        public int? ExcisableGoodsType { get; set; }
        public string ExtTypDescending { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}