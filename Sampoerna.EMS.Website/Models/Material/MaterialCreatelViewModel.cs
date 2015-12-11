using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialCreateViewModel : BaseModel
    {
        
        [Required, Display(Name = "Material Number")]
        public string MaterialNumber { get; set; }

        [Required, Display(Name = "Material Description")]
        public string MaterialDesc { get; set; }

        [Required, Display(Name = "Material Group")]
        public string MaterialGroup { get; set; }

        [Required, Display(Name = "Purchasing Group")]
        public string PurchasingGroup { get; set; }



        [Required, Display(Name = "Plant")]
        public string[] PlantId { get; set; }
        public string PlantName { get; set; }

        [Required, Display(Name = "Excisable Good Type")]
        public string GoodTypeId { get; set; }
        public string GoodTypeName { get; set; }

        [Required, Display(Name = "Issue Storace Loc")]
        public string IssueStorageLoc { get; set; }

        [Required, Display(Name = "Base UOM")]
        public string UomId { get; set; }
        public string UomName { get; set; }

        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedById { get; set; }

        [Display(Name = "Created By"), Editable(false)]
        public string CreatedBy { get; set; }
        public Nullable<bool> IsFromSap { get; set; }

        [Display(Name = "Changed On"), Editable(false)]
        public Nullable<System.DateTime> ChangedDate { get; set; }
       

        public Nullable<int> ChangedById { get; set; }

        [Display(Name = "Changed By"), Editable(false)]
        public string ChangedBy { get; set; }


        [Display(Name = "Plant Deletion")]
        public bool IsPlantDelete
        {
            get; set;
        }


     
        [ Display(Name = "Client Deletion")]
        public bool IsClientDelete
        {
            get; set;
        }

        public decimal? Conversion
        {
            get { return Convert.ToDecimal(ConversionValueStr); }
            set { value = Conversion; }
        }
        
        public string ConversionValueStr
        {
            get;
            set;
        }

        [Display(Name = "HJE")]
        public string HjeStr { get; set; }

        public decimal? Hje
        {
            get; set;
        }

        [Display(Name = "Tariff")]
        public string TariffStr { get; set; }

        public decimal? Tariff
        {
            get; set;
        }

        [Display(Name = "Tariff Currency")]
        public string Tariff_Curr { get; set; }

        [Display(Name = "Hje Currency")]
        public string Hje_Curr { get; set; }

       
        // list for dropdown in the form
        public MultiSelectList PlantList { get; set; }
        public SelectList GoodTypeList { get; set; }
        public SelectList BaseUOM { get; set; }

        public string ConversionUom { get; set; }
        
        public SelectList ConversionUomList { get; set; }

        public SelectList CurrencyList { get; set; }

        public List<MaterialUomDetails> MaterialUom { get; set; }       
    }
}