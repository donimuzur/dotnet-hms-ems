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
    public interface IProductionBLL
    {
        List<ProductionDto> GetAllByParam(ProductionGetByParamInput input);

        List<ProductionDto> GetAllProduction();

        SaveProductionOutput Save(ProductionDto productionDto, string userId);
        
        ProductionDto GetById(string companyCode, string plantWerk, string faCode, DateTime productionDate );

        List<ProductionDto> GetByCompPlant(string comp, string plant, string nppbkc, int period, int month, int year);

        PRODUCTION GetExistDto(string companyCode, string plantWerk, string faCode, DateTime productionDate);

        void SaveUpload(ProductionUploadItems uploadItems, string userId);

        void DeleteOldData(string companyCode, string plantWerk, string faCode, DateTime productionDate);

        List<ProductionDto> GetExactResult(List<ProductionDto> listItem);
        List<ProductionUploadItemsOutput> ValidationDailyUploadDocumentProcess(List<ProductionUploadItemsInput> inputs,string qtyPacked, string qty);
    }
}
