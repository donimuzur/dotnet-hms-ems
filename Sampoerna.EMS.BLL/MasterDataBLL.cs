using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core.Exceptions;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataBLL : IMasterDataBLL
    {
        private IGenericRepository<T1001> _repositoryT1001;
        private IGenericRepository<ZAIDM_EX_PCODE> _repositoryPersonalization;
        private IGenericRepository<T1001W> _repositoryT1001W;
        private IGenericRepository<ZAIDM_EX_SERIES> _repositorySeries;
        private IGenericRepository<ZAIDM_EX_MARKET> _repositoryMarket;
        private IGenericRepository<COUNTRY> _repositoryCountry;
        private IGenericRepository<CURRENCY> _repositoryCurrency;
        private IGenericRepository<ZAIDM_EX_PRODTYP> _repositoryProduct;
        
        private IGenericRepository<EX_SETTLEMENT> _ExSettlementRepository;
        private IGenericRepository<EX_STATUS> _ExStatusRepository;
        private IGenericRepository<REQUEST_TYPE> _RequestTypeRepository;
        private IGenericRepository<ZAIDM_EX_KPPBC> _ZaidExKppbkcRepository;
        private IGenericRepository<T1001W> _T1001WRepository;
        private IGenericRepository<CARRIAGE_METHOD> _CarriageMethodRepository;

        
        private IUnitOfWork _uow;

        public MasterDataBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _repositoryT1001 = _uow.GetGenericRepository<T1001>();
            _repositoryT1001W = _uow.GetGenericRepository<T1001W>();
            _repositoryPersonalization = _uow.GetGenericRepository<ZAIDM_EX_PCODE>();
            _repositorySeries = _uow.GetGenericRepository<ZAIDM_EX_SERIES>();
            _repositoryMarket = _uow.GetGenericRepository<ZAIDM_EX_MARKET>();
            _repositoryCountry = _uow.GetGenericRepository<COUNTRY>();
            _repositoryCurrency = _uow.GetGenericRepository<CURRENCY>();
            _repositoryProduct = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
            
            _ExSettlementRepository = _uow.GetGenericRepository<EX_SETTLEMENT>();
            _ExStatusRepository = _uow.GetGenericRepository<EX_STATUS>();
            _RequestTypeRepository = _uow.GetGenericRepository<REQUEST_TYPE>();
            _ZaidExKppbkcRepository = _uow.GetGenericRepository<ZAIDM_EX_KPPBC>();
            _T1001WRepository = _uow.GetGenericRepository<T1001W>();
            _CarriageMethodRepository = _uow.GetGenericRepository<CARRIAGE_METHOD>();

        }

        public List<string> GetDataCompany()
        {
            return _repositoryT1001.Get().Select(p => p.BUKRSTXT).Distinct().ToList();
        }

        
        public List<EX_SETTLEMENT> GetAllExciseExSettlements()
        {
            return _ExSettlementRepository.Get().ToList();
        }

        public List<EX_STATUS> GetAllExciseStatus()
        {
            return _ExStatusRepository.Get().ToList();
        }

        public List<REQUEST_TYPE> GetAllRequestTypes()
        {
            return _RequestTypeRepository.Get().ToList();
        }

        //use nppnkc get from ZAIDM_EX_NPPBKC
        //public string GetCeOfficeCodeByKppbcId(long kppBcId)
        //{
        //    var dbZaid = _ZaidExKppbkcRepository.GetByID(kppBcId);
        //    if (dbZaid == null)
        //        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

        //    return dbZaid.KPPBC_NUMBER;

        //}

      

        public List<T1001W> GetAllSourcePlants()
        {
            return _T1001WRepository.Get().ToList();
        }

        public T1001W GetPlantById(long plantId)
        {
            var includeTables = "ZAIDM_EX_NPPBKC.T1001";

            //var dbT100W = _T1001WRepository.GetByID(plantId);
            var dbT100W = _T1001WRepository.Get(p=>p.PLANT_ID == plantId, null, includeTables).FirstOrDefault();
           if (dbT100W == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbT100W;
        }

        public List<CARRIAGE_METHOD> GetAllCarriageMethods()
        {
            return _CarriageMethodRepository.Get().ToList();
        }

        
        #region ZAIDM_EX_PCODE

        public List<ZAIDM_EX_PCODE> GetDataPersonalization()
        {
            return _repositoryPersonalization.Get().ToList();
        }

        public ZAIDM_EX_PCODE GetDataPersonalizationById(long id)
        {
            return _repositoryPersonalization.GetByID(id);
        }

        public string GetPersonalizationDescById(long id)
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

        public ZAIDM_EX_SERIES GetDataSeriesById(long id)
        {
            return _repositorySeries.GetByID(id);
        }

        public string GetDataSeriesDescById(long id)
        {
            var dbData = _repositorySeries.GetByID(id);
            return dbData == null ? string.Empty : dbData.SERIES_VALUE;
        }

        #endregion

        #region ZAIDM_EX_MARKET

        public List<ZAIDM_EX_MARKET> GetAllDataMarket()
        {
            return _repositoryMarket.Get().ToList();
        }

        public ZAIDM_EX_MARKET GetDataMarketById(long id)
        {
            return _repositoryMarket.GetByID(id);
        }

        public string GetMarketDescById(long id)
        {
            var dbData = _repositoryMarket.GetByID(id);
            return dbData == null ? string.Empty : dbData.MARKET_DESC;
        }

        #endregion

        #region COUNTRY

        public List<COUNTRY> GetAllDataCountry()
        {
            return _repositoryCountry.Get().ToList();
        }

        public string GetCountryCodeById(int? id)
        {
            var dbData = _repositoryCountry.GetByID(id);
            return dbData == null ? string.Empty : dbData.COUNTRY_CODE;
        }

        #endregion

        #region CURRENCY

        public List<CURRENCY> GetAllDataCurrency()
        {
            return _repositoryCurrency.Get().ToList();
        }

        public string GetCurrencyCodeById(int? id)
        {
            var dbData = _repositoryCurrency.GetByID(id);
            return dbData == null ? string.Empty : dbData.CURRENCY_CODE;
        }

        #endregion

        public string GetProductCodeTypeDescById(int? id)
        {
            var dbData = _repositoryProduct.GetByID(id);
            return dbData == null ? string.Empty : dbData.PRODUCT_CODE + " - " + dbData.PRODUCT_TYPE;
        }
    }
}
