using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models
{
    public class BaseModel
    {
        public Enums.MenuList MainMenu { get; set; }
        public PAGE CurrentMenu { get; set; }
        public string ErrorMessage { get; set; }

       
    }
}