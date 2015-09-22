using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ILACK1Service
    {

        List<LACK1> GetAllByParam(Lack1GetByParamInput input);
        
        List<LACK1> GetByPeriod(Lack1GetByPeriodParamInput input);

        List<LACK1_PRODUCTION_DETAIL> GetProductionDetailByPeriode(Lack1GetByPeriodParamInput input);

        List<LACK1> GetCompletedDocumentByParam(Lack1GetByParamInput input);

        decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input);

        LACK1 GetLatestLack1ByParam(Lack1GetLatestLack1ByParamInput input);

        LACK1 GetById(int id);

        void Insert(LACK1 data);

        LACK1 GetBySelectionCriteria(Lack1GetBySelectionCriteriaParamInput input);

        LACK1 GetDetailsById(int id);

        List<LACK1> GetSummaryReportByParam(Lack1GetSummaryReportByParamInput input);

        List<int> GetYearList();

        List<LACK1> GetByCompanyCode(string companyCode);

        List<LACK1> GetDetailReportByParamInput(Lack1GetDetailReportByParamInput input);

    }
}