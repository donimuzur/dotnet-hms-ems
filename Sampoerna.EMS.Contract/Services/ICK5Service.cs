using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ICK5Service
    {
        List<CK5> GetForLack1ByParam(Ck5GetForLack1ByParamInput input);
        List<CK5> GetForLack2ByParam(Ck5GetForLack2ByParamInput input);
    }
}