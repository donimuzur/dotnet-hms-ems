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
        private IGenericRepository<EX_SETTLEMENT> _ExSettlementRepository;
        private IGenericRepository<EX_STATUS> _ExStatusRepository;
        private IGenericRepository<REQUEST_TYPE> _RequestTypeRepository;

        private IUnitOfWork _uow;

        public MasterDataBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _repositoryT1001 = _uow.GetGenericRepository<T1001>();
            _ExSettlementRepository = _uow.GetGenericRepository<EX_SETTLEMENT>();
            _ExStatusRepository = _uow.GetGenericRepository<EX_STATUS>();
            _RequestTypeRepository = _uow.GetGenericRepository<REQUEST_TYPE>();
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

    }
}
