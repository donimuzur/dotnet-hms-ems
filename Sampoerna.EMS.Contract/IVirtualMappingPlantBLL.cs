using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IVirtualMappingPlantBLL
    {
        VIRTUAL_PLANT_MAP GetByCompany(string companyid);
        VIRTUAL_PLANT_MAP GetById(long id);
        VIRTUAL_PLANT_MAP GetByIdIncludeChild(int id);
        List<VIRTUAL_PLANT_MAP> GetAll();
        bool Save(VIRTUAL_PLANT_MAP virtualPlant);

        bool Delete(int id, string userId);

    }
}
