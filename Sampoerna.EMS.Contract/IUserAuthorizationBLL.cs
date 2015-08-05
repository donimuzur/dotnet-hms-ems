using System;
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

    }
}
