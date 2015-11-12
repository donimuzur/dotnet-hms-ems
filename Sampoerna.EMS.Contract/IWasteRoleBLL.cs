using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteRoleBLL
    {
        List<WasteRoleDto> GetAllData(bool isIncludeTables);
    }
}
