using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack1PlantService
    {
        void DeleteByLack1Id(int lack1Id);
        void DeleteDataList(IEnumerable<LACK1_PLANT> listToDelete);
    }
}