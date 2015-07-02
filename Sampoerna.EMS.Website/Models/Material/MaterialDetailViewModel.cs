using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialDetailViewModel : BaseModel
    {
        public long MaterialId { get; set; }

        [Display(Name = "Material Number")]
        public string MaterialNumber { get; set; }

        [Display(Name = "Material Description")]
        public string MaterialDesc { get; set; }

        [Display(Name = "Material Group")]
        public string MaterialGroup { get; set; }

        [Display(Name = "Purchasing Group")]
        public string PurchasingGroup { get; set; }

        

        [Display(Name = "Plant")]
        public string PlantName { get; set; }

        [Display(Name = "Excisable Good Type")]        
        public string GoodTypeName { get; set; }

        [Display(Name = "Issue Storace Loc")]        
        public string IssueStorageLoc { get; set; }

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

        public Nullable<bool> isPlantDeleteTemp;
        [Required, Display(Name = "Plant Deletion")]
        public string IsPlantDelete
        {
            get
            {
                if (this.isPlantDeleteTemp.HasValue)
                {
                    return this.isPlantDeleteTemp.Value ? "Yes" : "No";
                }
                else
                {
                    return "No";
                }
            }
            
        }


        public Nullable<bool> isClientDelete;

        [Required, Display(Name = "Client Deletion")]
        public string IsClientDelete
        {
            get
            {
                if (this.isClientDelete.HasValue)
                {
                    return this.isClientDelete.Value ? "Yes" : "No";
                }
                else
                {
                    return "No";
                }
            }
            
        }
        
    }
}