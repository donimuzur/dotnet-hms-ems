using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface INlogBLL
    {
        NlogDto GetById(long id);

        List<NlogDto> GetNlogByParam(NlogGetByParamInput input);

        List<NlogDto> GetAllData();

        void DeleteDataByParam(NlogGetByParamInput input);

        void BackupXmlLog(BackupXmlLogInput input);
    }
}
