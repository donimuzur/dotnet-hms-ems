using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface ICompanyBLL
    {
       List<T1001> GetMasterData();

       void Save(T1001 saveCompany);

       void Update(T1001 updateToCompany);

       T1001 GetCompanyById(long id);
 
    }
}
