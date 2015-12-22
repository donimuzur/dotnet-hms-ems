using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IPbck1ProdConverterService
    {
        List<PBCK1_PROD_CONVERTER> GetProductionLack1TisToTis(Pbck1GetProductionLack1TisToTisParamInput input);
    }
}