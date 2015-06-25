using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;


namespace Sampoerna.EMS.Website.Models.PLANT
{
    public class PlantViewModel : BaseModel
    {
        public PlantViewModel()
        {
            Details = new List<DetailPlantT1001W>();
        }

        public List<DetailPlantT1001W> Details;

    }
    public class PlantFormModel : BaseModel
    {

        public IEnumerable<SelectListItem> PlantIdListItems { get; set; }
        public IEnumerable<SelectListItem> Nppbkc { get; set; }
        public IEnumerable<SelectListItem> RecieveMaterialListItems { get; set; }
        public DetailPlantT1001W Detail { get; set; }
    }

    public class DetailPlantT1001W
    {

        public long PlantId { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Werks")]
        public string Werks { get; set; }
        public string PlantDescription { get; set; }

        public bool IsMainPlant { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "City")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string City { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Skeptis")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string Skeptis { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Skeptis")]
        public int? RecievedMaterialTypeId { get; set; }
        public string NPPBKC_NO { get; set; }
        public string KPPBC_NO { get; set; }
        public ZAIDM_EX_NPPBKC NPPBKC { get; set; }

        public ZAIDM_EX_GOODTYP GOODTYP { get; set; }
    }

}