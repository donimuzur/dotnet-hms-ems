using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ICK5Service
    {
        List<CK5> GetForLack1ByParam(Ck5GetForLack1ByParamInput input);
        List<CK5> GetForLack2ByParam(Ck5GetForLack2ByParamInput input);

        WasteStockQuotaOutput GetWasteStockQuota(decimal wasteStock, string plantId, string materialNumber);
    }
}