using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IUserPlantMapService
    {
        void Save(USER_PLANT_MAP userPlantMap);
        List<USER_PLANT_MAP> GetAll();
        USER_PLANT_MAP GetById(int id);
        List<USER_PLANT_MAP> GetByUserId(string id);
        USER_PLANT_MAP GetByUserIdAndPlant(string userid, string plantid);
        void Delete(int id);
        List<T001W> GetAuthorizdePlant(UserPlantMapGetAuthorizedPlant input);
        List<ZAIDM_EX_NPPBKC> GetAuthorizedNppbkc(UserPlantMapGetAuthorizedNppbkc input);
        USER_PLANT_MAP GetByUserPlantNppbkcId(UserPlantMapGetByUserPlantNppbkcIdParamInput input);
    }
}