﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        ProductionDto GetById(ProductionGetByIdOutput output );


    }
}
