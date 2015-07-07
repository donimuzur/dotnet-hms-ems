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
        ZAIDM_EX_PCODE GetDataPersonalizationById(long id);
        string GetPersonalizationDescById(long id);

        List<EX_SETTLEMENT> GetAllExciseExSettlements();
        string GetExSettlementsNameById(int? id);

        List<EX_STATUS> GetAllExciseStatus();
        string GetExStatusNameById(int? id);

        List<REQUEST_TYPE> GetAllRequestTypes();
        string GetRequestTypeNameById(int? id);

        //string GetCeOfficeCodeByKppbcId(long kppBcId);

        List<T1001W> GetAllSourcePlants();

        T1001W GetPlantById(long plantId);

        List<CARRIAGE_METHOD> GetAllCarriageMethods();
        string GetCarriageMethodeNameById(int? id);

        List<ZAIDM_EX_SERIES> GetAllDataSeries();
        ZAIDM_EX_SERIES GetDataSeriesById(long id);
        string GetDataSeriesDescById(long id);

        #region ZAIDM_EX_MARKET

        List<ZAIDM_EX_MARKET> GetAllDataMarket();
        ZAIDM_EX_MARKET GetDataMarketById(long id);
        string GetMarketDescById(long id);

        #endregion

        #region COUNTRY

        List<COUNTRY> GetAllDataCountry();
        string GetCountryCodeById(int? id);
        
        #endregion

        #region CURRENCY

        List<CURRENCY> GetAllDataCurrency();
        string GetCurrencyCodeById(int? id);

        #endregion

        string GetProductCodeTypeDescById(int? id);
    }
}
