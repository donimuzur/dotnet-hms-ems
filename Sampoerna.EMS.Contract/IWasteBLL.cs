using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteBLL
    {
        List<WasteDto> GetAllByParam(WasteGetByParamInput input);

        List<WasteDto> GetAllWaste();

        bool Save(WasteDto wasteDto, string userId);

        WasteDto GetById(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate);

        WASTE GetExistDto(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate);

        void SaveUpload(WasteUploadItems wasteUpload);

        void DeleteOldData(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate);

        List<WasteUploadItemsOuput> ValidationWasteUploadDocumentProcess(List<WasteUploadItemsInput> inputs);
    }
}
