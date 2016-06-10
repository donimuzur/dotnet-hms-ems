
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class ProductTypeSaveInput
    {
        public ZAIDM_EX_PRODTYP ProductType { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }
}
