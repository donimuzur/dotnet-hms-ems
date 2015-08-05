using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PrintHistory;

namespace Sampoerna.EMS.Website.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            ChangesHistoryList = new List<ChangesHistoryItemModel>();
            PrintHistoryList = new List<PrintHistoryItemModel>();
            
        }
        public Enums.MenuList MainMenu { get; set; }
        public PAGE CurrentMenu { get; set; }
        //public string ErrorMessage { get; set; }

        public List<ChangesHistoryItemModel> ChangesHistoryList { get; set; }
        public List<PrintHistoryItemModel> PrintHistoryList { get; set; }
        
        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }

        public string MessageTitle { get; set; }
        public List<string> MessageBody { get; set; }

        public bool IsShowNewButton { get; set; }

    }
}