using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExPOABLL
    {
        ZAIDM_EX_POA GetById(int id);
        List<ZAIDM_EX_POA> GetAll();

        void save(ZAIDM_EX_POA poa);


    }
}