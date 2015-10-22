namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class UserPlantMapGetAuthorizedNppbkc
    {
        public string UserId { get; set; }
        public string CompanyCode { get; set; }
    }

    public class UserPlantMapGetAuthorizedPlant : UserPlantMapGetAuthorizedNppbkc
    {
        public string NppbkcId { get; set; }
    }
}
