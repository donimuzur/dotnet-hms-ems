﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IBrandRegistrationService
    {
        List<ZAIDM_EX_BRAND> GetByFaCodeList(List<string> input);

        ZAIDM_EX_BRAND GetByPlantIdAndFaCode(string plantId, string faCode);

        List<ZAIDM_EX_BRAND> GetBrandByPlant(string plant);

        ZAIDM_EX_GOODTYP GetGoodTypeByProdCodeInBrandRegistration(string prodCode);

        ZAIDM_EX_BRAND GetByPlantIdAndStickerCode(string plantId, string stickerCode);

        List<ZAIDM_EX_BRAND> GetBrandByPlantAndListProdCode(string plant, List<string> prodCode);

        List<ZAIDM_EX_BRAND> GetByPlantAndFaCode(List<string> plant, List<string> faCode);
    }
}