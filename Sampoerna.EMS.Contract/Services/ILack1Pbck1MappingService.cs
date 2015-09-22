using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack1Pbck1MappingService
    {
        void DeleteByLack1Id(int lack1Id);
        void DeleteDataList(IEnumerable<LACK1_PBCK1_MAPPING> listToDelete);
    }
}