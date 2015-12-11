﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteStockBLL
    {
        WasteStockDto GetById(int id);

        WasteStockDto GetById(int id, bool isIncludeTable);

        List<WasteStockDto> GetAllDataOrderByUserAndGroupRole();

        List<GetListMaterialByPlantOutput> GetListMaterialByPlant(string plantId);

        GetListMaterialUomByMaterialAndPlantOutput GetListMaterialUomByMaterialAndPlant(string materialNumber,
            string plantId);

        WasteStockDto SaveWasteStock(WasteStockSaveInput input);
    }
}