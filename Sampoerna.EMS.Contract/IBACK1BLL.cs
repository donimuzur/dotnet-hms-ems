using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IBACK1BLL
    {
        Back1Dto GetId(int id);

        List<Back1Dto> GetAll();

    }
}
