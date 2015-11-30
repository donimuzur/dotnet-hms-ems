using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class UserTree
    {
        public UserTree()
        {
            this.Employees = new List<USER>();
            this.Manager = new USER();
            
        }

        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public int? MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public int? USER_GROUP_ID { get; set; }
        public List<USER> Employees { get; set; }
        public USER Manager { get; set; }
     
        public string EMAIL { get; set; }
        
    }

}
