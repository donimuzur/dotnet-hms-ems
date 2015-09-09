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

        void Save(ProductionDto productionDto);

        ProductionDto GetById(string companyCode, string plantWerk, string faCode, DateTime productionDate );

        List<ProductionDto> GetByCompPlant(string comp, string plant);

        PRODUCTION GetExistDto(string companyCode, string plantWerk, string faCode, DateTime productionDate);
    }
}
