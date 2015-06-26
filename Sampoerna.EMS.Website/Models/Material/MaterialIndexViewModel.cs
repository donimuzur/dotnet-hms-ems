using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialIndexViewModel : BaseModel
    {
        public MaterialIndexViewModel()
        {
            Details = new List<MaterialDetail>();
        }

        public List<MaterialDetail> Details { get; set; }
    }

    public class MaterialDetail
    {
        public string PlantName { get; set; }

        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }

        public string GoodTypeName { get; set; }
        public string UomName { get; set; }

        public bool PlantDeletion { get; set; }
        public bool ClientDeletion { get; set; }

    }
}