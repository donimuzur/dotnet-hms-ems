using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface ILFA1BLL
    {
        List<LFA1Dto> GetAll();

        LFA1Dto GetById(string id);
    }
}