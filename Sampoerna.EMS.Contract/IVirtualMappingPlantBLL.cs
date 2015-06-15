using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IVirtualMappingPlantBLL
    {
        List<SaveVirtualMappingPlantOutput> GetAll();

        SaveVirtualMappingPlantOutput Save(VIRTUAL_PLANT_MAP virtualPlantMap);
       

    }
}
