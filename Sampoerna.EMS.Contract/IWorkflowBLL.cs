using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowBLL
    {
        List<UserTree> GetUserTree();
        UserTree GetUserTreeByUserID(int userID);
       
    }
}
