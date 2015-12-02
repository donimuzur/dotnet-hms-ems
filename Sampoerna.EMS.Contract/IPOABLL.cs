using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IPOABLL
    {
        POA GetById(string id);
        List<POA> GetAll();

        void Save(POA poa);

        void Update(POA poa);
        void Delete(string id);
        Core.Enums.UserRole GetUserRole(string userId);

        string GetManagerIdByPoaId(string poaId);

        List<string> GetPOAIdByManagerId(string managerId);

        POADto GetDetailsById(string id);
        List<POADto> GetPoaByNppbkcIdAndMainPlant(string nppbkcId);

        POA GetActivePoaById(string id);

        List<POADto> GetPoaActiveByNppbkcId(string nppbkcId);

        List<POADto> GetPoaActiveByPlantId(string plantId);
    }
}