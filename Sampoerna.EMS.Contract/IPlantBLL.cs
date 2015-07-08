using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IPlantBLL
    {
        T001W GetT001W(string NppbkcId, bool IsPlant);
        Plant GetId(string id);
        List<Plant> GetAll();
        void save(Plant plantT1001W,string userid);

        string GetPlantWerksById(string id);

        List<PLANT_RECEIVE_MATERIAL> GetReceiveMaterials(string plantId);
    }
}