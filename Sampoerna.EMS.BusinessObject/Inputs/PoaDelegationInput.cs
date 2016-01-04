using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class PoaDelegationSaveInput
    {
        public POA_DELEGATIONDto PoaDelegationDto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }
}
