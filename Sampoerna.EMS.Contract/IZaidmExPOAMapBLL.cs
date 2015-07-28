using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExPOAMapBLL
    {
        List<POA> GetPOAByNPPBKCID(string NPPBKCID);

    }
}