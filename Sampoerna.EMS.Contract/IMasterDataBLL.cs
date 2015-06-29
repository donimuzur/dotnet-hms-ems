using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IMasterDataBLL
    {
        List<string> GetDataCompany();

        List<ZAIDM_EX_PCODE> GetDataPersonalization();
        ZAIDM_EX_PCODE GetDataPersonalizationById(long id);

        List<ZAIDM_EX_SERIES> GetAllDataSeries();
        ZAIDM_EX_SERIES GetDataSeriesById(long id);

        List<ZAIDM_EX_MARKET> GetAllDataMarket();
        ZAIDM_EX_MARKET GetDataMarketById(long id);

        List<COUNTRY> GetAllDataCountry();

        List<CURRENCY> GetAllDataCurrency();
    }
}
