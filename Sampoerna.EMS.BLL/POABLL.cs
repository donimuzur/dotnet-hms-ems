using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class POABLL : IPOABLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<POA> _repository;
        private IGenericRepository<POA_MAP> _poaMapRepository;
        private string includeTables = "POA_MAP, USER, USER1, POA_SK";
        private IChangesHistoryBLL _changesHistoryBll;
        public POABLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<POA>();
            _poaMapRepository = _uow.GetGenericRepository<POA_MAP>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
        }


        public POA GetById(string id)
        {
            return _repository.Get(p => p.POA_ID == id, null, includeTables).FirstOrDefault();
        }

        public POADto GetDetailsById(string id)
        {
            return Mapper.Map<POADto>(_repository.Get(p => p.POA_ID == id, null, includeTables).FirstOrDefault());
        }

        public List<POA> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public void Save(POA poa)
        {



            try
            {
                //Insert
                _repository.InsertOrUpdate(poa);

                _uow.SaveChanges();

            }
            catch (Exception exception)
            {
                _uow.RevertChanges();
                _logger.Error(exception);

            }

        }
        
        public void Delete(string id)
        {
            var existingPoa = GetById(id);
            if (existingPoa.IS_ACTIVE == true)
            {
                existingPoa.IS_ACTIVE = false;
            }
            else
            {
                existingPoa.IS_ACTIVE = true;
            }
            _repository.Update(existingPoa);
            _uow.SaveChanges();
        }

        public void Update(POA poa)
        {
            try
            {
                
                _repository.Update(poa);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _uow.RevertChanges();
                throw;
            }

        }

        public Core.Enums.UserRole GetUserRole(string userId)
        {
            var poa = GetAll();

            if (poa.Any(zaidmExPoa => zaidmExPoa.MANAGER_ID == userId))
                return Core.Enums.UserRole.Manager;

            if (poa.Any(zaidmExPoa => zaidmExPoa.LOGIN_AS == userId))
                return Core.Enums.UserRole.POA;

            return Core.Enums.UserRole.User;
        }

        public string GetManagerIdByPoaId(string poaId)
        {
            var result = "";
            var dtData = _repository.Get(c => c.POA_ID == poaId).FirstOrDefault();
            if (dtData != null)
                result = dtData.MANAGER_ID;

            return result;
        }

        public List<POADto> GetPoaByNppbkcId(string nppbkcId)
        {
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.NPPBKC_ID == nppbkcId;
            var dbData = _poaMapRepository.Get(queryFilter, null, "POA");
            var poaList = dbData.ToList().Select(d => d.POA);
            return Mapper.Map<List<POADto>>(poaList.ToList());
        }
        //public POADto GetDetailsById(string id)
        //{
        //    var dtData =  _repository.Get(p => p.POA_ID == id, null, includeTables).FirstOrDefault();
        //    //if (dtData == null)
        //    //    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
        //    return AutoMapper.Mapper.Map<POADto>(dtData);
        //}

    }
}
