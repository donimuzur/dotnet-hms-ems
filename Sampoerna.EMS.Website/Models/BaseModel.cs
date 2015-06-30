using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Models
{
    public class BaseModel
    {
        public Enums.MenuList MainMenu { get; set; }
        public PAGE CurrentMenu { get; set; }
        public string ErrorMessage { get; set; }

        public List<ChangesHistoryItemModel> ChangesHistoryList { get; set; }
       
    }
}