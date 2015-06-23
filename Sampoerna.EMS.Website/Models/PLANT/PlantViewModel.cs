using System.Collections.Generic;

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
    public class DetailPlantT1001W
    {
        public long PlantId { get; set; }
        public string PlantDescription { get; set; }
        public bool IsMainPlant { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public string NPPBKC_NO { get; set; }
        public string KPPBC_NO { get; set; }

    }

}