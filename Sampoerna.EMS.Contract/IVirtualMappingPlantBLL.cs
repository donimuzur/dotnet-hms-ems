using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IVirtualMappingPlantBLL
    {
        VIRTUAL_PLANT_MAP GetById(long id);
        VIRTUAL_PLANT_MAP GetByIdIncludeChild(int id);
        List<VIRTUAL_PLANT_MAP> GetAll();
        bool Save(VIRTUAL_PLANT_MAP virtualPlant);
        // SaveVirtualMappingPlantOutput Save(VIRTUAL_PLANT_MAP virtualPlantMap);



        void Delete(int id, string p);
    }
}
