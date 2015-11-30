using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ICK5MaterialService
    {
        List<CK5_MATERIAL> GetForLack1ByParam(Ck5MaterialGetForLackByParamInput input);
    }
}