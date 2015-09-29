using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models
{
    public class UserViewModel : BaseModel
    {
        public List<UserItem> Details { get; set; }
    }

    public class UserItem
    {
        public UserItem()
        {
            this.Employees = new List<USER>();
            this.Manager = new USER();
           
        }
        public string USER_ID { get; set; }
        public string MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string USER_GROUP_ID { get; set; }
        public List<USER> Employees { get; set; }
        public USER Manager { get; set; }
        
        public string EMAIL { get; set; }

        public string PHONE { get; set; }

        
    }

    public class UserItemViewModel : BaseModel
    {
        public UserItem Detail { get; set; }
    }
}