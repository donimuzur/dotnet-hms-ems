﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IUserAuthorizationBLL
    {
        List<UserAuthorizationDto> GetAll();

        UserAuthorizationDto GetById(string id);

        List<BRoleDto> GetAllBRole();

        List<int?> GetAuthPages(string userid);

        List<string> GetPlants(string userid);

        List<NppbkcPlantDto> GetNppbckPlants(string userid);

        List<string> GetListPlantByUserId(string userId);

        List<string> GetListNppbkcByUserId(string userId);

    }
}
