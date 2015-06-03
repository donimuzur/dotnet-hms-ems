using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class UserTree
    {
        public UserTree()
        {
            Employees = new List<USER>();
            ZAIDM_EX_POA = new List<ZAIDM_EX_POA>();
        }

        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public Nullable<int> MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }

        public USER Manager { get; set; }
        public List<USER> Employees { get; set; }
        public List<ZAIDM_EX_POA> ZAIDM_EX_POA { get; set; }
    }



}
