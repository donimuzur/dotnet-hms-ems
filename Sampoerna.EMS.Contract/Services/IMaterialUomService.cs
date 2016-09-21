using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IMaterialUomService
    {
        List<MATERIAL_UOM> GetByMaterialListAndPlantId(List<string> materialList, string plantId);

        List<MATERIAL_UOM> GetByMaterialListAndPlantIdListSpecificBkcUom(List<string> materialList, List<string> plantId, string bkcUomId);

        List<MATERIAL_UOM> GetByMeinh(string meinh);
    }
}