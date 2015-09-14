using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteBLL
    {
        List<WasteDto> GetAllByParam(WasteGetByParamInput input);

        List<WasteDto> GetAllWaste();

        void Save(WasteDto wasteDto);

        WasteDto GetById(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate);

        WASTE GetExistDto(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate);
    }
}
