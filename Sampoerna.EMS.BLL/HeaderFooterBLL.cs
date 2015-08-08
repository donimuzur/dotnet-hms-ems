using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class HeaderFooterBLL : IHeaderFooterBLL
    {

        private IGenericRepository<HEADER_FOOTER> _repository;
        private IGenericRepository<HEADER_FOOTER_FORM_MAP> _mapRepository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "T001, HEADER_FOOTER_FORM_MAP";
        private IChangesHistoryBLL _changesHistoryBll;

        public HeaderFooterBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<HEADER_FOOTER>();
            _mapRepository = _uow.GetGenericRepository<HEADER_FOOTER_FORM_MAP>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
        }

        public HeaderFooterDetails GetDetailsById(int id)
        {
            return Mapper.Map<HeaderFooterDetails>(_repository.Get(c => c.HEADER_FOOTER_ID == id, null, includeTables).FirstOrDefault());
        }

        public HeaderFooter GetById(int id)
        {
           return Mapper.Map<HeaderFooter>(_repository.GetByID(id));
        }
        public List<HeaderFooter> GetAll()
        {
            return Mapper.Map<List<HeaderFooter>>(_repository.Get(null, null, includeTables).ToList());
        }

        public SaveHeaderFooterOutput GetCompanyId(string companyId)
        {
            var output = new SaveHeaderFooterOutput();
            var dtCompany = Mapper.Map<HEADER_FOOTER>(_repository.Get(c => c.BUKRS == companyId).FirstOrDefault());
            if (dtCompany != null)
            {
                output.MessageExist = "1";
                return output;
            }
            return output;
        }
        
        public SaveHeaderFooterOutput Save(HeaderFooterDetails headerFooterData, string userId)
        {
             HEADER_FOOTER dbData = null;
             

            //var dtcompany = _repository.Get(c => c.BUKRS == headerFooterData.COMPANY_ID).FirstOrDefault();
            // if (dtcompany != null)
            // {
            //     output.MessageExist = "1";
            //     return output;
            // }

            if (headerFooterData.HEADER_FOOTER_ID > 0)
            {

                //update
                dbData =
                    _repository.Get(c => c.HEADER_FOOTER_ID == headerFooterData.HEADER_FOOTER_ID, null, includeTables)
                        .FirstOrDefault();

                var headerFooterUpdated = Mapper.Map<HEADER_FOOTER>(headerFooterData);
               

                SetChanges(dbData, headerFooterUpdated, userId);

               
                //hapus dulu aja ya ? //todo ask the cleanist way
                var dataToDelete =
                    _mapRepository.Get(c => c.HEADER_FOOTER_ID == headerFooterData.HEADER_FOOTER_ID)
                        .ToList();
                foreach (var item in dataToDelete)
                {
                    _mapRepository.Delete(item);
                }

               

                Mapper.Map<HeaderFooterDetails, HEADER_FOOTER>(headerFooterData, dbData);
               

                dbData.HEADER_FOOTER_FORM_MAP = null;
                dbData.HEADER_FOOTER_FORM_MAP = Mapper.Map<List<HEADER_FOOTER_FORM_MAP>>(headerFooterData.HeaderFooterMapList);
               


            }
            else
            {
                //Insert
                dbData = Mapper.Map<HEADER_FOOTER>(headerFooterData);
                dbData.CREATED_DATE = DateTime.Now;
                dbData.CREATED_BY = userId;
                _repository.Insert(dbData);
            }


            var output = new SaveHeaderFooterOutput();
            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.HeaderFooterId = dbData.HEADER_FOOTER_ID;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;
        }

        public void Delete(int id, string userId)
        {
            var existingData = _repository.GetByID(id);

            _repository.Update(existingData);

            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                FORM_ID = existingData.HEADER_FOOTER_ID.ToString(),
                FIELD_NAME = "IS_DELETED",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now,
                OLD_VALUE = existingData.IS_DELETED.HasValue ? existingData.IS_DELETED.Value.ToString() : "NULL",
                NEW_VALUE = true.ToString()
            };
            existingData.IS_DELETED = true;
            _changesHistoryBll.AddHistory(changes);
            _uow.SaveChanges();
        }

        private void SetChanges(HEADER_FOOTER origin, HEADER_FOOTER data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("COMPANY_ID", origin.BUKRS == data.BUKRS);
            changesData.Add("HEADER_IMAGE_PATH", origin.HEADER_IMAGE_PATH == data.HEADER_IMAGE_PATH);
            changesData.Add("FOOTER_CONTENT", origin.FOOTER_CONTENT == data.FOOTER_CONTENT);
            changesData.Add("IS_ACTIVE", origin.IS_ACTIVE == data.IS_ACTIVE);
            changesData.Add("IS_DELETED", origin.IS_DELETED == data.IS_DELETED);
            var originHeaderFooterCheck = string.Empty;
            var originIndex = 0;
            var originMap = origin.HEADER_FOOTER_FORM_MAP;
            if (originMap != null)
            {
                foreach (var orMap in originMap)
                {

                    originIndex++;
                    originHeaderFooterCheck += orMap.FORM_TYPE_ID.ToString() + ": Footer Set : " + (orMap.IS_FOOTER_SET == true ? "Yes" : "No") + " Header Set : " + (orMap.IS_HEADER_SET == true ? "Yes" : "No");

                    if (originIndex < originMap.Count)
                    {
                        originHeaderFooterCheck += ", ";
                    }

                }
            }
            var dataHeaderFooterCheck = string.Empty;
            var dataIndex = 0;
            var dataMap = data.HEADER_FOOTER_FORM_MAP;
            if (dataMap != null)
            {
                foreach (var dtMap in dataMap)
                {

                    dataIndex++;
                    dataHeaderFooterCheck += dtMap.FORM_TYPE_ID.ToString() + ": Footer Set : " + (dtMap.IS_FOOTER_SET == true ? "Yes" : "No") + " Header Set : " + (dtMap.IS_HEADER_SET == true ? "Yes" : "No");

                    if (dataIndex < dataMap.Count)
                    {
                        dataHeaderFooterCheck += ", ";
                    }

                }
            }
            changesData.Add("HEADER_FOOTER_FORM_MAP", originHeaderFooterCheck == dataHeaderFooterCheck);


            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                        FORM_ID = data.HEADER_FOOTER_ID.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "COMPANY_ID":
                            changes.OLD_VALUE = origin.BUKRS;
                            changes.NEW_VALUE = data.BUKRS;
                            break;
                        case "HEADER_IMAGE_PATH":
                            changes.OLD_VALUE = origin.HEADER_IMAGE_PATH;
                            changes.NEW_VALUE = data.HEADER_IMAGE_PATH;
                            break;
                        case "FOOTER_CONTENT":
                            changes.OLD_VALUE = origin.FOOTER_CONTENT;
                            changes.NEW_VALUE = data.FOOTER_CONTENT;
                            break;
                        case "IS_ACTIVE":
                            changes.OLD_VALUE = origin.IS_ACTIVE.HasValue ? origin.IS_ACTIVE.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.IS_ACTIVE.HasValue ? data.IS_ACTIVE.Value.ToString() : "NULL";
                            break;
                        case "IS_DELETED":
                            changes.OLD_VALUE = origin.IS_DELETED.HasValue ? origin.IS_DELETED.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.IS_DELETED.HasValue ? data.IS_DELETED.Value.ToString() : "NULL";
                            break;
                        case "HEADER_FOOTER_FORM_MAP":
                            changes.OLD_VALUE = originHeaderFooterCheck;
                            changes.NEW_VALUE = dataHeaderFooterCheck;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        private void SetChanges(HEADER_FOOTER_FORM_MAP origin, HEADER_FOOTER_FORM_MAP data, string userId)
        {
            var changeData = new Dictionary<string, bool>();
            changeData.Add("FORM_TYPE_ID", origin.FORM_TYPE_ID.Equals(data.FORM_TYPE_ID));
            changeData.Add("IS_HEADER_SET", origin.IS_HEADER_SET.Equals(data.IS_HEADER_SET));
            changeData.Add("IS_FOOTER_SET", origin.IS_FOOTER_SET.Equals(data.IS_FOOTER_SET));

            foreach (var listchange in changeData)
            {
                var change = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Enums.MenuList.HeaderFooter,
                    FORM_ID = data.HEADER_FOOTER_FORM_MAP_ID.ToString(),
                    FIELD_NAME = listchange.Key,
                    MODIFIED_BY = userId,
                    MODIFIED_DATE = DateTime.Now

                };
                switch (listchange.Key)
                {
                    case "IS_HEADER_SET":
                        change.OLD_VALUE = origin.IS_HEADER_SET.ToString();
                        change.NEW_VALUE = data.IS_HEADER_SET.ToString();
                        break;
                    case "IS_FOOTER_SET":
                        change.OLD_VALUE = origin.IS_FOOTER_SET.ToString();
                        change.NEW_VALUE = origin.IS_FOOTER_SET.ToString();
                        break;
                    case "FORM_TYPE_ID":
                        change.OLD_VALUE = origin.FORM_TYPE_ID.ToString();
                        change.NEW_VALUE = origin.IS_FOOTER_SET.ToString();
                        break;
                }
                _changesHistoryBll.AddHistory(change);

            }
        }



       
    }
}
