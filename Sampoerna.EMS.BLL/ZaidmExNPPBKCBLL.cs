using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExNPPBKCBLL : IZaidmExNPPBKCBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _repository;
        private string includeTables = "ZAIDM_EX_KPPBC,T001W, LFA1";
        private IChangesHistoryBLL _changesHistoryBll;
        private IGenericRepository<T001K> _repositoryT001k;
        private IGenericRepository<T001W> _repositoryT001w;
        private IGenericRepository<POA_MAP> _poaMapRepository;
        public ZaidmExNPPBKCBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
            _poaMapRepository = _uow.GetGenericRepository<POA_MAP>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow,_logger);
            
        }

        public ZAIDM_EX_NPPBKC GetById(string id)
        {
            //return _repository.Get(id);
            return _repository.Get(c => c.NPPBKC_ID == id, null, includeTables).FirstOrDefault();
        }

        public ZAIDM_EX_NPPBKCDto GetDetailsById(string id)
        {
            return AutoMapper.Mapper.Map<ZAIDM_EX_NPPBKCDto>(_repository.Get(c => c.NPPBKC_ID == id, null, ", T001W, T001, ZAIDM_EX_KPPBC").FirstOrDefault());
        }

        public List<ZAIDM_EX_NPPBKC> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public void Save(ZAIDM_EX_NPPBKC nppbkc)
        {
            //if (!string.IsNullOrEmpty(nppbkc.NPPBKC_ID))
            //{
            //    //update
            //    _repository.Update(nppbkc);
            //}
            //else
            //{
            //    //Insert
            //    _repository.Insert(nppbkc);
            //}

            try
            {
                _repository.InsertOrUpdate(nppbkc);
                _uow.SaveChanges();

            }
            catch (Exception exception)
            {
                _logger.Error(exception);

            }

        }


        public void Delete(string id)
        {
            var existingNppbkc = GetById(id);
            existingNppbkc.IS_DELETED = true;
            _repository.Update(existingNppbkc);
            _uow.SaveChanges();
        }
        
        public void Update(ZAIDM_EX_NPPBKC nppbkc)
        {
            try
            {
                _repository.Update(nppbkc);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _uow.RevertChanges();
                throw;
            }
        }

        public string GetCityByNppbkcId(string nppBkcId)
        {
            var dbData = _repository.GetByID(nppBkcId);
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData.CITY;

        }

        public string GetCeOfficeCodeByNppbcId(string nppBkcId)
        {

            var dbData = _repository.Get(n => n.NPPBKC_ID == nppBkcId, null, "ZAIDM_EX_KPPBC").FirstOrDefault();
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData.ZAIDM_EX_KPPBC.KPPBC_ID;

        }

        public List<ZAIDM_EX_NPPBKCDto> GetByFlagDeletion(bool isDeleted)
        {
            var queryFilter = PredicateHelper.True<ZAIDM_EX_NPPBKC>();
            if (!isDeleted)
            {
                queryFilter =
                    queryFilter.And(
                        c => !c.IS_DELETED.HasValue || (c.IS_DELETED.HasValue && c.IS_DELETED.Value == false));
            }
            else
            {
                queryFilter = queryFilter.And(c => c.IS_DELETED.HasValue && c.IS_DELETED.Value == true);
            }

            var dbData = _repository.Get(queryFilter, null, includeTables);
            return Mapper.Map<List<ZAIDM_EX_NPPBKCDto>>(dbData.ToList());
        }

        public ZAIDM_EX_NPPBKCDto GetDetailsByCityName(string cityName)
        {
            return AutoMapper.Mapper.Map<ZAIDM_EX_NPPBKCDto>(_repository.Get(c => c.CITY == cityName, null, ", T001W, T0011, ZAIDM_EX_KPPBC").FirstOrDefault());
        }

        public List<ZAIDM_EX_NPPBKC> GetNppbkcsByCompany(string companyId)
        {
            _repositoryT001k = _uow.GetGenericRepository<T001K>();
            _repositoryT001w = _uow.GetGenericRepository<T001W>();
            var data = (from a in _repository.GetQuery()
                join b
                    in _repositoryT001w.GetQuery() on a.NPPBKC_ID
                    equals b.NPPBKC_ID
                join c
                    in _repositoryT001k.GetQuery()
                    on b.WERKS equals c.BWKEY
                where
                    c.BUKRS == companyId
                select a
                ).Distinct();
                
            return data.ToList();
        }
        public List<ZAIDM_EX_NPPBKCDto> GetNppbkcsByPOA(string poaId)
        {
            //query by nppbkc, main plant and active poa
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.POA_ID == poaId
                && c.POA.IS_ACTIVE.HasValue && c.POA.IS_ACTIVE.Value;

            var dbData = _poaMapRepository.Get(queryFilter, null, "ZAIDM_EX_NPPBKC");
            var nppbkcList = dbData.ToList().Select(d => d.ZAIDM_EX_NPPBKC).Distinct();
            return Mapper.Map<List<ZAIDM_EX_NPPBKCDto>>(nppbkcList.ToList());
        }

        public List<ZAIDM_EX_NPPBKCDto> GetNppbkcMainPlantOnlyByPoa(string poaId)
        {
            //query by nppbkc, main plant and active poa
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.POA_ID == poaId
                && c.T001W.IS_MAIN_PLANT.HasValue && c.T001W.IS_MAIN_PLANT.Value
                && c.POA.IS_ACTIVE.HasValue && c.POA.IS_ACTIVE.Value;

            var dbData = _poaMapRepository.Get(queryFilter, null, "ZAIDM_EX_NPPBKC");
            var nppbkcList = dbData.ToList().Select(d => d.ZAIDM_EX_NPPBKC).Distinct();
            return Mapper.Map<List<ZAIDM_EX_NPPBKCDto>>(nppbkcList.ToList());
        }

        private bool IsNppbkcImport(string id)
        {
            
            bool isImport = _repositoryT001w.Get(x => x.NPPBKC_IMPORT_ID == id).Any();

            return isImport;
        }

       

    }
}
