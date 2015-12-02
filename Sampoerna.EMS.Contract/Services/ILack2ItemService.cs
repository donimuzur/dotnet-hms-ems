using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack2ItemService
    {
        void DeleteDataList(IEnumerable<LACK2_ITEM> listToDelete);
        void DeleteByLack2Id(int lack2Id);
        List<LACK2_ITEM> GetLack2ItemByCk5Id(long ck5Id);
    }
}