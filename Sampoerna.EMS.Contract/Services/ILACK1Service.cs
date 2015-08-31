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

    }
}