using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialItemModel : BaseModel
    {
        public long MATERIAL_ID { get; set; }
        [Required, Display(Name = "Sticker Id")]
        public string STICKER_ID { get; set; }

        [Required, Display(Name = "Sticker Code")]
        public string STICKER_CODE { get; set; }

        [Required, Display(Name = "Plant Id")]
        public long PLANT_ID { get; set; }

        [Required, Display(Name = "FA Code")]
        public string FA_CODE { get; set; }

        [Required, Display(Name = "Purchasing Group")]
        public string PURCHASING_GROUP { get; set; }

        [Required, Display(Name = "Issue Storage No.")]
        public string ISSUE_STORAGE { get; set; }

        [Required, Display(Name = "Conversion")]
        public Nullable<decimal> CONVERSION { get; set; }


        [Required, Display(Name = "Material Desc.")]
        public string MATERIAL_DESC { get; set; }


        [Required, Display(Name = "Base UOM")]
        public int UOM_ID { get; set; }

        [Required, Display(Name = "Excisable Goods Type")]
        public Nullable<int> GOODTYP_ID { get; set; }

        [Required, Display(Name = "Created On")]
        public Nullable<System.DateTime> CREATED_ON { get; set; }

        [Required, Display(Name = "Sticker Id")]
        public Nullable<System.DateTime> CHANGED_ON { get; set; }

        [Required, Display(Name = "Created By")]
        public string CREATED_DATE { get; set; }

        [Required, Display(Name = "Changed By")]
        public string CHANGED_BY { get; set; }


        // list for dropdown in the form
        public SelectList PlantList { get; set; }
        public SelectList GoodTypeList { get; set; }
        public SelectList BaseUOM { get; set; }
    }
}