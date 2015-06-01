using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowBLL
    {
        List<UserTree> GetUserTree();
        UserTree GetUserTreeByUserID(int userID);
       
    }
}
