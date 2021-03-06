﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class UserPlantMapService : IUserPlantMapService
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER_PLANT_MAP> _repository;
        private IGenericRepository<BROLE_MAP> _repositoryBRoleMap;

        private string _includeTables = "USER, T001W, T001W.T001K";

        public UserPlantMapService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<USER_PLANT_MAP>();
            _repositoryBRoleMap = _uow.GetGenericRepository<BROLE_MAP>();
        }
        public void Save(USER_PLANT_MAP userPlantMap)
        {
            _repository.InsertOrUpdate(userPlantMap);
        }

        public List<USER_PLANT_MAP> GetAll()
        {
            return _repository.Get(null, null, _includeTables).ToList();
        }

        public USER_PLANT_MAP GetById(int id)
        {
            var dbResult = _repository.GetByID(id);

            if (dbResult == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return dbResult;
        }

        public List<USER_PLANT_MAP> GetByUserId(string id)
        {
            return _repository.Get(p => p.USER_ID == id, null, _includeTables).ToList();
        }

        public USER_PLANT_MAP GetByUserIdAndPlant(string userid, string plantid)
        {
            return _repository.Get(p => p.USER_ID == userid && p.PLANT_ID == plantid, null, _includeTables).FirstOrDefault();
        }

        public USER_PLANT_MAP GetByUserPlantNppbkcId(UserPlantMapGetByUserPlantNppbkcIdParamInput input)
        {
            return
                _repository.Get(
                    c => c.PLANT_ID == input.PlantId && c.USER_ID == input.UserId && c.NPPBKC_ID == input.NppbkcId, null, _includeTables)
                    .FirstOrDefault();
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public List<ZAIDM_EX_NPPBKC> GetAuthorizedNppbkc(UserPlantMapGetAuthorizedNppbkc input)
        {
            Expression<Func<USER_PLANT_MAP, bool>> queryFilter = c => c.USER_ID == input.UserId && c.T001W.T001K.BUKRS == input.CompanyCode;
            var dataMap = _repository.Get(queryFilter, null, "T001W, ZAIDM_EX_NPPBKC, T001W.T001K").ToList();
            if (dataMap.Count == 0) return new List<ZAIDM_EX_NPPBKC>();
            var nppbkcList = dataMap.Where(c => c.ZAIDM_EX_NPPBKC != null && (!c.ZAIDM_EX_NPPBKC.IS_DELETED.HasValue 
                || !c.ZAIDM_EX_NPPBKC.IS_DELETED.Value)).Select(d => d.ZAIDM_EX_NPPBKC).Distinct().ToList();
            return nppbkcList;
        }

        public List<T001W> GetAuthorizdePlant(UserPlantMapGetAuthorizedPlant input)
        {
            Expression<Func<USER_PLANT_MAP, bool>> queryFilter = c => c.USER_ID == input.UserId && c.T001W.T001K.BUKRS == input.CompanyCode
                && (c.T001W.NPPBKC_ID == input.NppbkcId || c.T001W.NPPBKC_IMPORT_ID == input.NppbkcId);
            var dataMap = _repository.Get(queryFilter, null, "T001W, T001W.T001K").ToList();
            if (dataMap.Count == 0) return new List<T001W>();
            var plantList = dataMap.Where(c => c.T001W != null && (!c.T001W.IS_DELETED.HasValue || !c.T001W.IS_DELETED.Value)).Select(d => d.T001W).Distinct().ToList();
            return plantList;
        }


        public List<USER_PLANT_MAP> GetByPlantId(string plantId)
        {
            return _repository.Get(p => p.PLANT_ID == plantId, null, "USER").ToList();
        }

        public List<USER_PLANT_MAP> GetByNppbkcId(string nppbkcId)
        {
            return _repository.Get(p => p.NPPBKC_ID == nppbkcId).ToList();
        }

        public List<string> GetUserBRoleMapByPlantIdAndUserRole(string plantId, Enums.UserRole userRole)
        {
            var listUserMap = _repository.Get(p => p.PLANT_ID == plantId && p.USER.IS_ACTIVE == 1, null, "USER").ToList();
            var listUser = listUserMap.Select(x => x.USER_ID).ToList();
            var listBrole =
                _repositoryBRoleMap.Get(
                    c => listUser.Contains(c.MSACCT) && c.ROLEID == userRole);

            return listBrole.Select(x => x.MSACCT).ToList();
        }

    }
}
