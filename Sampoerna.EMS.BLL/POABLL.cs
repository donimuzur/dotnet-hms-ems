using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.CustomService.Data;
using System.Reflection;
using Sampoerna.EMS.CustomService.Services;

namespace Sampoerna.EMS.BLL
{
    public class POABLL : IPOABLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<BusinessObject.POA> _repository;
        private IGenericRepository<BusinessObject.POA_MAP> _poaMapRepository;
        private IGenericRepository<BusinessObject.BROLE_MAP> _broleMapRepository;
        private IMasterDataAprovalBLL _masterDataAprovalBLL;
        private string includeTables = "POA_MAP, USER, USER1, POA_SK";
        private IChangesHistoryBLL _changesHistoryBll;
        private PoaExciserService _poaExciserService;
        private SystemReferenceService refService;
        public POABLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<BusinessObject.POA>();
            _poaMapRepository = _uow.GetGenericRepository<BusinessObject.POA_MAP>();
            _broleMapRepository = _uow.GetGenericRepository<BusinessObject.BROLE_MAP>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _masterDataAprovalBLL = new MasterDataApprovalBLL(_uow,_logger);
            this.refService = new SystemReferenceService();
        }


        public BusinessObject.POA GetById(string id)
        {
            return _repository.Get(p => p.POA_ID == id, null, includeTables).FirstOrDefault();
        }

        public POADto GetDetailsById(string id)
        {
            return Mapper.Map<POADto>(_repository.Get(p => p.POA_ID == id, null, includeTables).FirstOrDefault());
        }

        public List<BusinessObject.POA> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public void Save(BusinessObject.POA poa)
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
            var oldPoa = Mapper.Map<BusinessObject.POA>(tempExistingPoa);
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

        public void Update(BusinessObject.POA poa)
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
            Expression<Func<BusinessObject.POA_MAP, bool>> queryFilter = c => c.NPPBKC_ID == nppbkcId 
                && c.T001W.IS_MAIN_PLANT.HasValue && c.T001W.IS_MAIN_PLANT.Value 
                && c.POA.IS_ACTIVE.HasValue && c.POA.IS_ACTIVE.Value;
            var dbData = _poaMapRepository.Get(queryFilter, null, "POA");
            var poaList = dbData.ToList().Select(d => d.POA);
            return Mapper.Map<List<POADto>>(poaList.ToList());
        }



        public BusinessObject.POA GetActivePoaById(string id)
        {
            return _repository.Get(p => p.POA_ID == id && p.IS_ACTIVE == true, null, includeTables).FirstOrDefault();
        }

        public List<POADto> GetPoaActiveByNppbkcId(string nppbkcId)
        {
            Expression<Func<BusinessObject.POA_MAP, bool>> queryFilter = c => c.NPPBKC_ID == nppbkcId && c.POA.IS_ACTIVE.Value;
            var dbData = _poaMapRepository.Get(queryFilter, null, "POA");
            var poaList = dbData.ToList().Select(d => d.POA);
            return Mapper.Map<List<POADto>>(poaList.ToList());
        }


        public List<POADto> GetPoaActiveByPlantId(string plantId)
        {
            Expression<Func<BusinessObject.POA_MAP, bool>> queryFilter = c => c.WERKS == plantId && c.POA.IS_ACTIVE.Value;
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

        public List<BusinessObject.POA> GetAllOnlyPoa()
        {
            return _repository.Get().ToList();
        }


        #region Insert to Changes Log
        public BusinessObject.POA InsertChangesLog(BusinessObject.POA data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return data;
        }
        #endregion

        #region Update to Changes Log
        public bool EditChangesLog(BusinessObject.POA data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = GetById(data.POA_ID);

                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Changes Log

        /// <summary> Part of Changes Log Step which Mark All Available Changes </summary>        
        /// <param name="old"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        private Dictionary<string, string[]> GetAllChanges(BusinessObject.POA old, BusinessObject.POA updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                        "ID_CARD",
                        "POA_PHONE",
                        "POA_ADDRESS",
                        "POA_EMAIL",
                        "PRINTED_NAME",
                        "TITLE",
                        "LOGIN_AS",
                        "MANAGER_ID",
                        "IS_ACTIVE"
                     };
                var oldProps = new Dictionary<string, object>();
                var props = new Dictionary<string, object>();

                foreach (var prop in updated.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {

                    props.Add(prop.Name, prop.GetValue(updated, null));
                    if (old != null)
                        oldProps.Add(prop.Name, prop.GetValue(old, null));
                    else
                        oldProps.Add(prop.Name, null);
                }
                foreach (var item in props)
                {
                    var oldValue = (oldProps[item.Key] != null) ? oldProps[item.Key].ToString() : "N/A";
                    var newValue = (props[item.Key] != null) ? props[item.Key].ToString() : "N/A"; // updated value
                    //  var newValue = (item.Value != null) ? item.ToString() : "N/A"; // updated field and value 

                    if (!columns.Contains(item.Key))
                        continue;

                    if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                        changes.Add(item.Key, new string[] { oldValue, newValue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary> Part of Changes Log Step which Set All Changed Field into Table Changes Log </summary>        
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="changes"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="actor"></param>
        /// <param name="comment"></param>
        private void LogsActivity(EMSDataModel context, BusinessObject.POA data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context, formType, data.POA_ID.ToString(), map.Key, map.Value[0], map.Value[1], actor, DateTime.Now);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
