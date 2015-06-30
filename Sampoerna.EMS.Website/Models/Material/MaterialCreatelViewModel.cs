using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialCreateViewModel : BaseModel
    {
        public long MaterialId { get; set; }
        [Required, Display(Name = "Sticker Id")]
        public string StickerId { get; set; }

        [Required, Display(Name = "Sticker Code")]
        public string StickerCode { get; set; }

        [Required, Display(Name = "Plant Id")]
        public long PlantId { get; set; }

        [Required, Display(Name = "FA Code")]
        public string FaCode { get; set; }

        [Required, Display(Name = "Purchasing Group")]
        public string PurchasingGroup { get; set; }

        [Required, Display(Name = "Issue Storage No.")]
        public string IssueStorage { get; set; }

        [Required, Display(Name = "Conversion")]
        public Nullable<decimal> Conversion { get; set; }


        [Required, Display(Name = "Material Desc.")]
        public string MaterialDesc { get; set; }


        [Required, Display(Name = "Base UOM")]
        public int UomId { get; set; }

        [Required, Display(Name = "Excisable Goods Type")]
        public Nullable<int> GoodtypId { get; set; }

        [Required, Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedOn { get; set; }

        [Required, Display(Name = "Sticker Id")]
        public Nullable<System.DateTime> ChangedOn { get; set; }

        [Required, Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Required, Display(Name = "Changed By")]
        public string ChangedBy { get; set; }


        // list for dropdown in the form
        public SelectList PlantList { get; set; }
        public SelectList GoodTypeList { get; set; }
        public SelectList BaseUOM { get; set; }
    }
}