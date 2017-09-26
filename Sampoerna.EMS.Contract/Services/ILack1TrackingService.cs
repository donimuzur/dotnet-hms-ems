using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack1TrackingService
    {
        void DeleteByLack1Id(int? lack1Id);

        void DeleteCalculationDetails(int? lack1Id);

        void DeletePeriodSummary(int? lack1Id);

        void DeleteDataList(IEnumerable<LACK1_TRACKING> listToDelete);

        List<long> GetMovement201FromTracking();
    }
}