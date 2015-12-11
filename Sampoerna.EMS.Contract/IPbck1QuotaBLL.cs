using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPbck1QuotaBLL
    {
        List<Pbck1QuotaDto> GetAll();
        List<Pbck1QuotaDto> GetByParam(Pbck1QuotaGetByParamInput input);
    }
}