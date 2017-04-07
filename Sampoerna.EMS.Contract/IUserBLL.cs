﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IUserBLL
    {
        List<USER> GetUsers(UserInput input);

        List<USER> GetUsers();

        List<USER> GetUsersByListId(List<string> useridlist);
        
        List<UserTree> GetUserTree();

        Login GetLogin(string userName);

        USER GetUserById(string id);

        List<UserDto> GetListUserRoleByUserId(string userId);

        void SaveUser(USER user);

    }
}
