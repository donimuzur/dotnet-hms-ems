using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.Contract
{
    public interface IPlantBLL
    {
        Plant GetId(long id);
        List<Plant> GetAll();
        void save(Plant plantT1001W, int userId);

    }
}