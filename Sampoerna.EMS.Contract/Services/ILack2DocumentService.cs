using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack2DocumentService
    {
        void DeleteDataList(IEnumerable<LACK2_DOCUMENT> listToDelete);
        void DeleteByLack2Id(int lack2Id);
    }
}