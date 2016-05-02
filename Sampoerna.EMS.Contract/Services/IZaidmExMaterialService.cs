using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IZaidmExMaterialService
    {
        ZAIDM_EX_MATERIAL GetByMaterialAndPlantId(string materialId, string plantId);
        List<ZAIDM_EX_MATERIAL> GetByPlantId(string plantId);
        List<ZAIDM_EX_MATERIAL> GetByMaterialListAndPlantId(List<string> materialList, string plantId);
        List<ZAIDM_EX_MATERIAL> GetByPlantIdAndExGoodType(List<string> plantId, string exGoodType);
        List<ZAIDM_EX_MATERIAL> GetAll();
    }
}