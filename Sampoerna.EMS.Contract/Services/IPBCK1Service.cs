using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IPBCK1Service
    {
        List<PBCK1> GetForLack1ByParam(Pbck1GetDataForLack1ParamInput input);
    }
}