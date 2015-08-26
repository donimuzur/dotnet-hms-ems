using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK1BLL
    {
        List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input);
        List<Lack1Dto> GetCompletedDocumentByParam(Lack1GetByParamInput input);

        SaveLack1Output Save(Lack1SaveInput input);
        
        decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input);

        List<Lack1Dto> GetByPeriod(Lack1GetByPeriodParamInput input);

    }
}
