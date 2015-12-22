using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteRoleBLL
    {
        List<WasteRoleDto> GetAllData(bool isIncludeTables = true);

        List<WasteRoleDto> GetAllDataOrderByUserAndGroupRole();

        List<WasteRoleDto> GetAllDataGroupByRoleOrderByUserAndGroupRole();

        WasteRoleDto SaveWasteRole(WasteRoleSaveInput input);

        WasteRoleDto GetById(int id);

        WasteRoleDto GetById(int id, bool isIncludeTable);

        WasteRoleDto GetDetailsById(int id);
    }
}
