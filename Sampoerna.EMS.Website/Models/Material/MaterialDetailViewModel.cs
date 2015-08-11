using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialDetailViewModel : BaseModel
    {
     
        [Display(Name = "Material Number")]
        public string MaterialNumber { get; set; }

        [Display(Name = "Material Description")]
        public string MaterialDesc { get; set; }

        [Display(Name = "Material Group")]
        public string MaterialGroup { get; set; }

        [Display(Name = "Purchasing Group")]
        public string PurchasingGroup { get; set; }


        public string PlantId { get; set; }
        [Display(Name = "Plant")]
        public string PlantName { get; set; }

        public int GoodTypeId { get; set; }
        [Display(Name = "Excisable Good Type")]        
        public string GoodTypeName { get; set; }

        [Display(Name = "Issue Storace Loc")]        
        public string IssueStorageLoc { get; set; }

        public int UomId { get; set; }
        [Display(Name = "Base UOM")]
        public string UomName { get; set; }

        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedById { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public Nullable<bool> IsFromSap { get; set; }

        [Display(Name = "Changed On")]
        public Nullable<System.DateTime> ChangedDate { get; set; }

        
        public Nullable<int> ChangedById { get; set; }

        [Display(Name = "Changed By")]
        public string ChangedBy { get; set; }

        [Display(Name = "Convertion")]
        public decimal? Convertion { get; set; }

        [Required, Display(Name = "Plant Deletion")]
        public bool IsPlantDelete
        {
            get; set;

        }


        [Required, Display(Name = "Client Deletion")]
        public bool IsClientDelete
        {
            get; set;

        }

        public Nullable<bool> IsDeleted;

        public bool IsAllowDelete { get; set; }

        public decimal? Conversion
        {
            get;
            set;
        }
        [Required]
        public string ConversionValueStr
        {
            get;
            set;
        }
       

        public List<MaterialUomDetails> MaterialUom { get; set; } 
        
    }
}