using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILack2Service
    {
        List<LACK2> GetAll();
        List<LACK2> GetByParam(Lack2GetByParamInput input);
        List<LACK2> GetCompletedByParam(Lack2GetByParamInput input);
        LACK2 GetById(int id);
        LACK2 GetDetailsById(int id);
        void Insert(LACK2 data);
        LACK2 GetBySelectionCriteria(Lack2GetBySelectionCriteriaParamInput input);
        List<LACK2> GetSummaryReportsByParam(Lack2GetSummaryReportByParamInput input);
        List<LACK2> GetDetailReportsByParam(Lack2GetDetailReportByParamInput input);
        List<LACK2> GetDashboardDataByParam(Lack2GetDashboardDataByParamInput input);

    }
}