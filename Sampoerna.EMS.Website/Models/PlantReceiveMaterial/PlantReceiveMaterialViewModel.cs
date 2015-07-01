namespace Sampoerna.EMS.Website.Models.PlantReceiveMaterial
{

    public class PlantReceiveMaterialItemModel
    {
        public long PLANT_MATERIAL_ID { get; set; }
        public long PLANT_ID { get; set; }
        public int GOODTYPE_ID { get; set; }
        public int? EXC_GOOD_TYP { get; set; }
        public string EXT_TYP_DESC { get; set; }
    }

}