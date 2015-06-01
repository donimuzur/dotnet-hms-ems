using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject
{
    public class UserTree
    {
       public UserTree()
       {
            Employees = new List<USER>();
            
       }
        public int Id { get; set; }
        public  USER Manager { get; set; }
        public  List<USER> Employees { get; set; } 
    }


    
}
