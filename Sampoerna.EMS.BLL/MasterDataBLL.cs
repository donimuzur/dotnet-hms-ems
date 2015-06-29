using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataBLL : IMasterDataBLL
    {
        private IGenericRepository<T1001> _repositoryT1001;
        private IGenericRepository<ZAIDM_EX_PCODE> _repositoryPersonalization;
        private IGenericRepository<ZAIDM_EX_SERIES> _repositorySeries;
        private IGenericRepository<ZAIDM_EX_MARKET> _repositoryMarket;
        private IGenericRepository<COUNTRY> _repositoryCountry;
        private IGenericRepository<CURRENCY> _repositoryCurrency;

        private IUnitOfWork _uow;

        public MasterDataBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _repositoryT1001 = _uow.GetGenericRepository<T1001>();
            _repositoryPersonalization = _uow.GetGenericRepository<ZAIDM_EX_PCODE>();
            _repositorySeries = _uow.GetGenericRepository<ZAIDM_EX_SERIES>();
            _repositoryMarket = _uow.GetGenericRepository<ZAIDM_EX_MARKET>();
            _repositoryCountry = _uow.GetGenericRepository<COUNTRY>();
            _repositoryCurrency = _uow.GetGenericRepository<CURRENCY>();
        }

        public List<string> GetDataCompany()
        {
            return _repositoryT1001.Get().Select(p => p.BUKRSTXT).Distinct().ToList();
        }

        public List<ZAIDM_EX_PCODE> GetDataPersonalization()
        {
            return _repositoryPersonalization.Get().ToList();
        }

        public ZAIDM_EX_PCODE GetDataPersonalizationById(long id)
        {
            return _repositoryPersonalization.GetByID(id);
        }


        public List<ZAIDM_EX_SERIES> GetAllDataSeries()
        {
            return _repositorySeries.Get().ToList();
        }

        public ZAIDM_EX_SERIES GetDataSeriesById(long id)
        {
            return _repositorySeries.GetByID(id);
        }

        public List<ZAIDM_EX_MARKET> GetAllDataMarket()
        {
            return _repositoryMarket.Get().ToList();
        }

        public ZAIDM_EX_MARKET GetDataMarketById(long id)
        {
            return _repositoryMarket.GetByID(id);
        }

        public List<COUNTRY> GetAllDataCountry()
        {
            return _repositoryCountry.Get().ToList();
        }

        public List<CURRENCY> GetAllDataCurrency()
        {
            return _repositoryCurrency.Get().ToList();
        }
    }
}
