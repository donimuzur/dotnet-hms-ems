using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public class DetailPlantT1001W
        {
            public long PlantId { get; set; }
            public string Werks { get; set; }
            public bool IsMainPlant { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
        }
    
    }