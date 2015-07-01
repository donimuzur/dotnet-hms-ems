﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Website.Models.PlantReceiveMaterial;


namespace Sampoerna.EMS.Website.Models.PLANT
{
    public class PlantViewModel : BaseModel
    {
        public PlantViewModel()
        {
            Details = new List<T1001W>();
        }

        public List<T1001W> Details { get; set; }
        public ZAIDM_EX_NPPBKC Nppbkc { get; set; }
        public long PlantId { get; set; }
        public string Werks { get; set; }
        public string PlantDescription { get; set; }
        public bool IsMainPlant { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Skeptis { get; set; }
        public int? RecievedMaterialTypeId { get; set; }
       

    }
    public class PlantFormModel : BaseModel
    {

        public IEnumerable<SelectListItem> PlantIdListItems { get; set; }
        public IEnumerable<SelectListItem> Nppbkc { get; set; }
        public DetailPlantT1001W Detail { get; set; }
    }

    public class DetailPlantT1001W
    {

        public DetailPlantT1001W()
        {
            ReceiveMaterials = new List<PlantReceiveMaterialItemModel>();
        }

        public long PlantId { get; set; }

        public T1001W WerksT1001W { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Werks")]
        public string Werks { get; set; }
        public string PlantDescription { get; set; }

        public bool IsMainPlant { get; set; }
        public bool IsYes { get; set; }
        public bool IsNo { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Plant City")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string City { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Skeptis")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string Skeptis { get; set; }

        public string NPPBKC_NO { get; set; }
        public string KPPBC_NO { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        public int NppbkcNo { get; set; }
        
        public List<PlantReceiveMaterialItemModel> ReceiveMaterials { get; set; }

        public string Phone { get; set; }
    }

}