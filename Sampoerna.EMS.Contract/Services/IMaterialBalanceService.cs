using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IMaterialBalanceService
    {
        List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantAndMaterialList(string plantId, List<string> materialList);
    }
}
