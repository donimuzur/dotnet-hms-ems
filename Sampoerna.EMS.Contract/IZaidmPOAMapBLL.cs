using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmPOAMapBLL
    {
        List<ZAIDM_EX_POA> GetPOAByNPPBKCID(string NPPBKCID);
    }
}