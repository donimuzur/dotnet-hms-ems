﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class POABLL : IPOABLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<POA> _repository;
        private IGenericRepository<POA_MAP> _poaMapRepository;
        private IGenericRepository<BROLE_MAP> _broleMapRepository;
        private IMasterDataAprovalBLL _masterDataAprovalBLL;
        private string includeTables = "POA_MAP, USER, USER1, POA_SK";
        private IChangesHistoryBLL _changesHistoryBll;
        public POABLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<POA>();
            _poaMapRepository = _uow.GetGenericRepository<POA_MAP>();
            _broleMapRepository = _uow.GetGenericRepository<BROLE_MAP>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _masterDataAprovalBLL = new MasterDataApprovalBLL(_uow,_logger);
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

        public void Delete(string id,string userId)
        {
            var existingPoa = GetById(id);
            var tempExistingPoa = Mapper.Map<POADto>(existingPoa);
            bool isExist;
            if (existingPoa.IS_ACTIVE == true)
            {
                existingPoa.IS_ACTIVE = false;
            }
            else
            {
                existingPoa.IS_ACTIVE = true;
            }

            var oldPoa = Mapper.Map<POA>(tempExistingPoa);
            MASTER_DATA_APPROVAL approvalData;
            existingPoa = _masterDataAprovalBLL.MasterDataApprovalValidation((int) Sampoerna.EMS.Core.Enums.MenuList.POA, userId,
                oldPoa, existingPoa,out isExist,out approvalData);

            if (existingPoa.IS_ACTIVE != oldPoa.IS_ACTIVE)
            {
                _repository.Update(existingPoa);
                
            }

            _uow.SaveChanges();
            _masterDataAprovalBLL.SendEmailWorkflow(approvalData.APPROVAL_ID);
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
            var role = _broleMapRepository.Get(x => x.MSACCT.ToUpper() == userId).Select(x => x.ROLEID).FirstOrDefault();

            if (role.HasValue)
            {
                return role.Value;
            }
            else
            {
                var poa = GetAll();
                var manager = _broleMapRepository.Get(x => x.USER_BROLE.BROLE_DESC.Contains("POA_MANAGER")).Select(x => x.MSACCT.ToUpper()).ToList();
                if (manager.Contains(userId.ToUpper()))
                    return Core.Enums.UserRole.Controller;

                if (poa.Any(zaidmExPoa => zaidmExPoa.LOGIN_AS == userId))
                    return Core.Enums.UserRole.POA;


                return Core.Enums.UserRole.User;
            }
        }

        public string GetManagerIdByPoaId(string poaId)
        {
            var result = "";
            var dtData = _repository.Get(c => c.POA_ID == poaId).FirstOrDefault();
            if (dtData != null)
                result = dtData.MANAGER_ID;

            return result;
        }

        public List<string> GetPOAIdByManagerId(string managerId)
        {
            var dtData = _repository.Get(c => c.MANAGER_ID == managerId).Select(s => s.POA_ID).ToList();

            return dtData;
        }

        public List<POADto> GetPoaByNppbkcIdAndMainPlant(string nppbkcId)
        {
            //query by nppbkc, main plant and active poa
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.NPPBKC_ID == nppbkcId 
                && c.T001W.IS_MAIN_PLANT.HasValue && c.T001W.IS_MAIN_PLANT.Value 
                && c.POA.IS_ACTIVE.HasValue && c.POA.IS_ACTIVE.Value;
            var dbData = _poaMapRepository.Get(queryFilter, null, "POA");
            var poaList = dbData.ToList().Select(d => d.POA);
            return Mapper.Map<List<POADto>>(poaList.ToList());
        }



        public POA GetActivePoaById(string id)
        {
            return _repository.Get(p => p.POA_ID == id && p.IS_ACTIVE == true, null, includeTables).FirstOrDefault();
        }

        public List<POADto> GetPoaActiveByNppbkcId(string nppbkcId)
        {
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.NPPBKC_ID == nppbkcId && c.POA.IS_ACTIVE.Value;
            var dbData = _poaMapRepository.Get(queryFilter, null, "POA");
            var poaList = dbData.ToList().Select(d => d.POA);
            return Mapper.Map<List<POADto>>(poaList.ToList());
        }


        public List<POADto> GetPoaActiveByPlantId(string plantId)
        {
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.WERKS == plantId && c.POA.IS_ACTIVE.Value;
            var dbData = _poaMapRepository.Get(queryFilter, null, "POA");
            var poaList = dbData.ToList().Select(d => d.POA);
            return Mapper.Map<List<POADto>>(poaList.ToList());
        }

        public List<POADto> GetAllPoaActive()
        {
            var dbData = _repository.Get(c => c.IS_ACTIVE.Value).ToList();
            return Mapper.Map<List<POADto>>(dbData);
        }


        public List<string> GetPoaPlantByPoaId(string poaId)
        {
            var dbData = _poaMapRepository.Get(c => c.POA_ID == poaId && c.POA.IS_ACTIVE.Value).ToList();
            return dbData.Select(c => c.WERKS).ToList();
        }

        public List<POA> GetAllOnlyPoa()
        {
            return _repository.Get().ToList();
        }
    }
}
