using System.Collections.Generic;
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
        
        public IEnumerable<SelectListItem> PlantId { get; set; }
        public IEnumerable<SelectListItem> Nppbkc { get; set; }

        public  DetailPlantT1001W Detail { get; set; }
    }

    public class DetailPlantT1001W
    {
        public string PlantId { get; set; }
        public string Werks { get; set; }
        public string PlantDescription { get; set; }
        public bool IsMainPlant { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Skeptis { get; set; }
        public int? RecievedMaterialTypeId { get; set; }
        public string NPPBKC_NO { get; set; }
        public string KPPBC_NO { get; set; }

        public ZAIDM_EX_NPPBKC NPPBKC { get; set; }
    }

}