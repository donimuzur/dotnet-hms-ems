namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class UserPlantMapDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string PlantId { get; set; }

        public string UserName { get; set; }

        public string PlantName { get; set; }
        public string NppbkcId { get; set; }

        public string IsActive { get; set; }
    }
}
