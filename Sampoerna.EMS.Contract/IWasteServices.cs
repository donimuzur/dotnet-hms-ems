
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteServices
    {
        List<WASTE> GetWasteDailyProdByParam(GetWasteDailyProdByParamInput input);
    }
}
