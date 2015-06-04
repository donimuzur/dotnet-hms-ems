using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IUserBLL
    {
        List<USER> GetUsers(UserInput input);

        UserTree GetUserTreeByUserID(int userID);

        List<UserTree> GetUserTree();

    }
}
