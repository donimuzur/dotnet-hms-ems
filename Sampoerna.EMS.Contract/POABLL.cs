using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IPOABLL
    {
        POA GetById(int id);
        List<POA> GetAll();

        void Save(POA poa);

        void Update(POA poa);
        void Delete(int id);

        Enums.UserRole GetUserRole(string userId);

    }
}