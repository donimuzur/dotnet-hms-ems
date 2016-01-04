using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPoaDelegationBLL
    {
        List<POA_DELEGATIONDto> GetAllData();

        POA_DELEGATIONDto SavePoaDelegation(PoaDelegationSaveInput input);
    }
}
