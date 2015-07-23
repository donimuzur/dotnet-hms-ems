using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            ChangesHistoryList = new List<ChangesHistoryItemModel>();
        }
        public Enums.MenuList MainMenu { get; set; }
        public PAGE CurrentMenu { get; set; }
        //public string ErrorMessage { get; set; }

        public List<ChangesHistoryItemModel> ChangesHistoryList { get; set; }


        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }

        public string MessageTitle { get; set; }
        public List<string> MessageBody { get; set; }

       
    }
}