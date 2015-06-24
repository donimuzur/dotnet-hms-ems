using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataBLL : IMasterDataBLL
    {
        private IGenericRepository<T1001> _repositoryT1001;
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

        public string GetCeOfficeCodeByKppbcId(long kppBcId)
        {
            var dbZaid = _ZaidExKppbkcRepository.GetByID(kppBcId);
            if (dbZaid == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbZaid.KPPBC_NUMBER;

        }

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
    }
}
