using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack1ProductionDetailService
    {
        void DeleteByLack1Id(int lack1Id);
        void DeleteDataList(IEnumerable<LACK1_PRODUCTION_DETAIL> listToDelete);
    }
}