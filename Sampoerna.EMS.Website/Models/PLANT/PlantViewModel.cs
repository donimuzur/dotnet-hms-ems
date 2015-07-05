using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.PlantReceiveMaterial;


namespace Sampoerna.EMS.Website.Models.PLANT
{
    public class PlantViewModel : BaseModel
    {
        public PlantViewModel()
        {
            Details = new List<DetailPlantT1001W>();
        }
        public List<DetailPlantT1001W> Details { get; set; }
    }
    public class PlantFormModel : BaseModel
    {
        public IEnumerable<SelectListItem> Nppbkc { get; set; }
        public DetailPlantT1001W Detail { get; set; }
    }

    public class DetailPlantT1001W
    {

        public DetailPlantT1001W()
        {
            ReceiveMaterials = new List<PlantReceiveMaterialItemModel>();
        }

       
                [Display(Name = "Werks")]
        public string Werks { get; set; }
        public string Name1 { get; set; }
        public string PlantDescription { get; set; }
        public string Ort01 { get; set; }

        public bool IsMainPlant { get; set; }
        public bool IsYes { get; set; }
        public bool IsNo { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Plant City")]
        public string City { get; set; }

        [Display(Name = "Skeptis")]
        public string Skeptis { get; set; }

        public string KPPBC_NO { get; set; }

        [Required]
        public string NPPBKC_ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        
        public List<PlantReceiveMaterialItemModel> ReceiveMaterials { get; set; }

        public string Phone { get; set; }
    }

}