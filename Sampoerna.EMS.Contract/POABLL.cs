using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPOABLL
    {
        POA GetById(int id);
        List<POA> GetAll();

        void Save(POA poa);

        void Update(POA poa);
        void Delete(int id);



    }
}