using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExPOABLL
    {
        ZAIDM_EX_POA GetById(int id);
        List<ZAIDM_EX_POA> GetAll();
        
        void Save(ZAIDM_EX_POA poa);

        void Update(ZAIDM_EX_POA poa);
        void Delete(int id);

        Enums.UserRole GetUserRole(int id);


    }
}