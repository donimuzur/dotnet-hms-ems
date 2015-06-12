using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IVirtualMappingPlanBLL
    {
        List<VIRTUAL_PLANT_MAP> GetAll();

        SaveVirtualMappingPlantOutput Save(VIRTUAL_PLANT_MAP virtualPlantMap);
       

    }
}
