using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IXmlFileLogBLL
    {
        List<XML_LOGSDto> GetXmlLogByParam(GetXmlLogByParamInput input);

        XML_LOGSDto GetByIdIncludeTables(long id);
    }
}
