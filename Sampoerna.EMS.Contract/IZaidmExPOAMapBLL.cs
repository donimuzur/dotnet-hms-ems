using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExPOAMapBLL
    {
        List<POADto> GetPOAByNPPBKCID(string NPPBKCID);
        POA_MAPDto GetByUserLogin(string userLogin);
    }
}