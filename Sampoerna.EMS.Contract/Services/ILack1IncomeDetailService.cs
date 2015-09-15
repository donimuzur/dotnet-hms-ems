using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack1IncomeDetailService
    {
        void DeleteByLack1Id(int lack1Id);
        void DeleteDataList(IEnumerable<LACK1_INCOME_DETAIL> listToDelete);
    }
}