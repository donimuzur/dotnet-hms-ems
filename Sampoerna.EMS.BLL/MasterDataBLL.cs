﻿using System;
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
            _repositoryT1001 = _uow.GetGenericRepository<T001>();
            _repositoryT1001W = _uow.GetGenericRepository<T001W>();
            _repositoryPersonalization = _uow.GetGenericRepository<ZAIDM_EX_PCODE>();
            _repositorySeries = _uow.GetGenericRepository<ZAIDM_EX_SERIES>();
            _repositoryMarket = _uow.GetGenericRepository<ZAIDM_EX_MARKET>();
           _repositoryProduct = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
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

        public ZAIDM_EX_PCODE GetDataPersonalizationById(int id)
        {
            return _repositoryPersonalization.GetByID(id);
        }

        public string GetPersonalizationDescById(int id)
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

        public ZAIDM_EX_SERIES GetDataSeriesById(int id)
        {
            return _repositorySeries.GetByID(id);
        }

        public int? GetDataSeriesDescById(int? id)
        {
            var dbData = _repositorySeries.GetByID(id);
            return dbData == null ? 0 : dbData.SERIES_VALUE;
        }

        #endregion

        #region ZAIDM_EX_MARKET

        public List<ZAIDM_EX_MARKET> GetAllDataMarket()
        {
            return _repositoryMarket.Get().ToList();
        }

        public ZAIDM_EX_MARKET GetDataMarketById(int id)
        {
            return _repositoryMarket.GetByID(id);
        }

        public string GetMarketDescById(int? id)
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

        public string GetProductCodeTypeDescById(int id)
        {
            var dbData = _repositoryProduct.GetByID(id);
            return dbData == null ? string.Empty : dbData.PROD_CODE + " - " + dbData.PRODUCT_TYPE;
        }


        
    }
}
