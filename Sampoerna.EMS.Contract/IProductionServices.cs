using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IProductionServices
    {
        List<PRODUCTION> GetProductionByParam(GetProductionDailyProdByParamInput input);
    }
}
