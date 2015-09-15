using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IBrandRegistrationService
    {
        List<ZAIDM_EX_BRAND> GetByFaCodeList(List<string> input);

        ZAIDM_EX_BRAND GetByPlantIdAndFaCode(string plantId, string faCode);
    }
}