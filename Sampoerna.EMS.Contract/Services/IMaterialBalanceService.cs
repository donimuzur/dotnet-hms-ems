using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IMaterialBalanceService
    {
        List<MaterialBalanceDto> GetByPlantAndMaterialList(List<string> plantId, List<string> materialList, int month,
            int year, string uomId);

        List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantListAndMaterialList(List<string> plantId, List<string> materialList);

        MaterialBalanceTotalDto GetByMaterialListAndPlant(string plantId, List<string> materialList, int month,
            int year);
    }
}
