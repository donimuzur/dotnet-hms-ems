using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.GOODSTYPE
{
    public class GoodsTypeGroupCreateViewModel : BaseModel
    {
        public GoodsTypeGroupCreateViewModel()
        {
            GroupName = string.Empty;
            Details = new List<GoodsTypeDetails>();

        }

        [Required (ErrorMessage = "Group Name invalid")]
        [StringLength(50)]
        public string GroupName { get; set; }
       
        public List<GoodsTypeDetails> Details { get; set; }
        
    }


    public class GoodsTypeDetails
    {
        public GoodsTypeDetails()
        {
            GoodTypeName = string.Empty;
        }
        public string GoodTypeId { get; set; }
        public string GoodTypeName { get; set; }
        public bool IsChecked { get; set; }
        
    }

   
}