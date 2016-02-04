using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IMaterialBalanceService
    {
        List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantAndMaterialList(List<string> plantId, List<string> materialList, int month, int year,string uomId);

        List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantListAndMaterialList(List<string> plantId, List<string> materialList);
    }
}
