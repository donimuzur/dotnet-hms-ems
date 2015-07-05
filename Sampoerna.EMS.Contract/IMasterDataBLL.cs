using System;
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
        ZAIDM_EX_PCODE GetDataPersonalizationById(int id);
        string GetPersonalizationDescById(int id);

        List<ZAIDM_EX_SERIES> GetAllDataSeries();
        ZAIDM_EX_SERIES GetDataSeriesById(int id);
        int? GetDataSeriesDescById(int? id);

        #region ZAIDM_EX_MARKET

        List<ZAIDM_EX_MARKET> GetAllDataMarket();
        ZAIDM_EX_MARKET GetDataMarketById(int id);
        string GetMarketDescById(int? id);

        #endregion

        #region COUNTRY

        List<string> GetAllDataCountry();
        #endregion

        #region CURRENCY

        List<string> GetAllDataCurrency();
        #endregion

        string GetProductCodeTypeDescById(int id);
    }
}
