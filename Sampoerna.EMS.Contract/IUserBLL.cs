using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IUserBLL
    {
        List<USER> GetUsers(UserInput input);

        List<USER> GetUsers();

        
        List<UserTree> GetUserTree();

        Login GetLogin(string userName);

        USER GetUserById(string id);
        
    }
}
