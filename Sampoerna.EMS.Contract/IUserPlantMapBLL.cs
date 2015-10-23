using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IUserPlantMapBLL
    {
        void Save(USER_PLANT_MAP userPlantMap);

        List<USER_PLANT_MAP> GetAll();

        USER_PLANT_MAP GetById(int id);

        List<USER_PLANT_MAP> GetByUserId(string id);
        USER_PLANT_MAP GetByUserIdAndPlant(string userid, string plantid);

        void Delete(int id);

        List<T001WCompositeDto> GetAuthorizdePlant(UserPlantMapGetAuthorizedPlant input);
        List<ZAIDM_EX_NPPBKCCompositeDto> GetAuthorizedNppbkc(UserPlantMapGetAuthorizedNppbkc input);

        List<string> GetPlantByUserId(string id);
    }
}