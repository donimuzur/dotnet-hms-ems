using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ICK4CItemService
    {
        List<CK4C_ITEM> GetByParam(CK4CItemGetByParamInput input);
        List<string> GetFaCodeListByParam(CK4CItemGetByParamInput input);
    }
}
