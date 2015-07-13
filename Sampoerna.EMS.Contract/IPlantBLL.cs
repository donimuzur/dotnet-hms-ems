using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IPlantBLL
    {
        Plant GetId(long id);
        List<Plant> GetAll();
        void save(Plant plantT1001W,int userid);

        string GetPlantWerksById(long id);

        string GetPlantNameById(long id);
    }
}