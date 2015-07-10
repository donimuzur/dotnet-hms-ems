using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataBLL : IMasterDataBLL
    {
        private IGenericRepository<T001> _repositoryT1001;
        private IGenericRepository<ZAIDM_EX_PCODE> _repositoryPersonalization;
        private IGenericRepository<T001W> _repositoryT1001W;
        private IGenericRepository<ZAIDM_EX_SERIES> _repositorySeries;
        private IGenericRepository<ZAIDM_EX_MARKET> _repositoryMarket;
        private IGenericRepository<ZAIDM_EX_PRODTYP> _repositoryProduct;

        private IUnitOfWork _uow;

        public MasterDataBLL(IUnitOfWork uow)
        {
            _uow = uow;
            repositoryT1001 = uow.GetGenericRepository<T001>();
            repositoryT1001W = uow.GetGenericRepository<T001W>();
            repositoryPersonalization = uow.GetGenericRepository<ZAIDM_EX_PCODE>();
            repositorySeries = uow.GetGenericRepository<ZAIDM_EX_SERIES>();
            repositoryMarket = uow.GetGenericRepository<ZAIDM_EX_MARKET>();
            repositoryProduct = uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
        }

        public List<string> GetDataCompany()
        {
            return _repositoryT1001.Get().Select(p => p.BUTXT).Distinct().ToList();
        }

        #region ZAIDM_EX_PCODE

        public List<ZAIDM_EX_PCODE> GetDataPersonalization()
        {
            return _repositoryPersonalization.Get().ToList();
        }

        public ZAIDM_EX_PCODE GetDataPersonalizationById(string id)
        {
            return _repositoryPersonalization.GetByID(id);
        }

        public string GetPersonalizationDescById(string id)
        {
            var dbData = _repositoryPersonalization.GetByID(id);
            return dbData == null ? string.Empty : dbData.PER_DESC;
        }

        #endregion

        #region ZAIDM_EX_SERIES

        public List<ZAIDM_EX_SERIES> GetAllDataSeries()
        {
            return _repositorySeries.Get().ToList();
        }

        public ZAIDM_EX_SERIES GetDataSeriesById(string id)
        {
            return _repositorySeries.GetByID(id);
        }

        public string GetDataSeriesDescById(string id)
        {
            var dbData = _repositorySeries.GetByID(id);
            return dbData == null ? null : dbData.SERIES_VALUE;
        }

        #endregion

        #region ZAIDM_EX_MARKET

        public List<ZAIDM_EX_MARKET> GetAllDataMarket()
        {
            return _repositoryMarket.Get().ToList();
        }

        public ZAIDM_EX_MARKET GetDataMarketById(string id)
        {
            return _repositoryMarket.GetByID(id);
        }

        public string GetMarketDescById(string id)
        {
            var dbData = _repositoryMarket.GetByID(id);
            return dbData == null ? string.Empty : dbData.MARKET_DESC;
        }

        #endregion

        #region COUNTRY


        public List<string> GetAllDataCountry()
        {
            var list = new List<string>();
            list.Add("INA");
            list.Add("USA");
            list.Add("AUS");
            return list;

        }
        public List<string> GetAllDataCurrency()
        {
            var list = new List<string>();
            list.Add("IDR");
            list.Add("USD");
            return list;

        }

        #endregion

        #region CURRENCY



        #endregion

        public string GetProductCodeTypeDescById(string id)
        {
            var dbData = _repositoryProduct.GetByID(id);
            return dbData == null ? string.Empty : dbData.PROD_CODE + " - " + dbData.PRODUCT_TYPE;
        }



    }
}