using System;

namespace Sampoerna.EMS.Website.Models
{
    public class UserViewModel
    {
        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public Nullable<int> MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
    }
}