using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface ICompanyBLL
    {
        List<T001> GetMasterData();

        List<T001> GetAllData();

        T001 GetById(string id);

    }
}
