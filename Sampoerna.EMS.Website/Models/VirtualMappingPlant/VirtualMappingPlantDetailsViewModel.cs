namespace Sampoerna.EMS.Website.Models.VirtualMappingPlant
{
    public class VirtualMappingPlantDetailsViewModel : BaseModel
    {
        public long VirtualMapId { get; set; }
        public string CompanyName { get; set; }
        public string ImportPlanName { get; set; }
        public string ExportPlanName { get; set; }
        public string ImportPlantDesc { get; set; }
        public string ExportPlantDesc { get; set; }

        public bool? IsDeleted { get; set; }
    }
}