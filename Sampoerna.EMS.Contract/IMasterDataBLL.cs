﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IMasterDataBLL
    {
        List<string> GetDataCompany();

        List<ZAIDM_EX_PCODE> GetDataPersonalization();
        ZAIDM_EX_PCODE GetDataPersonalizationById(string id);
        string GetPersonalizationDescById(string id);

        List<ZAIDM_EX_SERIES> GetAllDataSeries();
        ZAIDM_EX_SERIES GetDataSeriesById(string id);
        string GetDataSeriesDescById(string id);

        #region ZAIDM_EX_MARKET

        List<ZAIDM_EX_MARKET> GetAllDataMarket();
        ZAIDM_EX_MARKET GetDataMarketById(string id);
        string GetMarketDescById(string id);

        #endregion

        #region COUNTRY

        List<COUNTRY> GetAllDataCountry();
        #endregion

        #region CURRENCY

        List<CURRENCY> GetAllDataCurrency();
        #endregion

        string GetProductCodeTypeDescById(string id);
    }
}
