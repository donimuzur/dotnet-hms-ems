using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class PoaDelegationBLL : IPoaDelegationBLL
    {
         private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<POA_DELEGATION> _repository;

        private IChangesHistoryBLL _changesHistoryBll;

        private string _includeTables = "POA, USER";

        public PoaDelegationBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<POA_DELEGATION>();

            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
        }


        public List<POA_DELEGATIONDto> GetAllData()
        {
            var listData = _repository.Get();

            return Mapper.Map<List<POA_DELEGATIONDto>>(listData);
        }

        public POA_DELEGATIONDto SavePoaDelegation(PoaDelegationSaveInput input)
        {
            //ValidateWasteStock(input);

            POA_DELEGATION dbData = null;
            if (input.PoaDelegationDto.POA_DELEGATION_ID > 0)
            {
                //update
                dbData = _repository.GetByID(input.PoaDelegationDto.POA_DELEGATION_ID);
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                var origin = Mapper.Map<POA_DELEGATIONDto>(dbData);

                SetChangesHistory(origin, input.PoaDelegationDto, input.UserId);

                Mapper.Map(input.PoaDelegationDto, dbData);

                dbData.MODIFIED_DATE = DateTime.Now;
                dbData.MODIFIED_BY = input.UserId;

            }
            else
            {
                input.PoaDelegationDto.CREATED_DATE = DateTime.Now;
                input.PoaDelegationDto.CREATED_BY = input.UserId;
                dbData = new POA_DELEGATION();
                Mapper.Map(input.PoaDelegationDto, dbData);
                _repository.Insert(dbData);
            }

            try
            {
                _uow.SaveChanges();
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


            return Mapper.Map<POA_DELEGATIONDto>(dbData);

        }

        private bool SetChangesHistory(POA_DELEGATIONDto origin, POA_DELEGATIONDto data, string userId)
        {
            bool isModified = false;

            var changesData = new Dictionary<string, bool>();
            changesData.Add("DELEGATION_FROM", origin.POA_FROM == data.POA_FROM);
            changesData.Add("DELEGATION_TO", origin.POA_TO == data.POA_TO);
            changesData.Add("FROM_DATE", origin.DATE_FROM == data.DATE_FROM);
            changesData.Add("TO_DATE", origin.DATE_TO == data.DATE_TO);
            changesData.Add("REASON", origin.REASON == data.REASON);

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.PoaDelegation;
                changes.FORM_ID = origin.POA_DELEGATION_ID.ToString();
                changes.FIELD_NAME = listChange.Key;
                changes.MODIFIED_BY = userId;
                changes.MODIFIED_DATE = DateTime.Now;
                switch (listChange.Key)
                {
                    case "DELEGATION_FROM":
                        changes.OLD_VALUE = origin.POA_FROM;
                        changes.NEW_VALUE = data.POA_FROM;
                        break;

                    case "DELEGATION_TO":
                        changes.OLD_VALUE = origin.POA_TO;
                        changes.NEW_VALUE = data.POA_TO;
                        break;

                    case "FROM_DATE":
                        changes.OLD_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(origin.DATE_FROM);
                        changes.NEW_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(data.DATE_FROM);
                        break;

                    case "TO_DATE":
                        changes.OLD_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(origin.DATE_TO);
                        changes.NEW_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(data.DATE_TO);
                        break;

                    case "REASON":
                        changes.OLD_VALUE = origin.REASON;
                        changes.NEW_VALUE = data.REASON;
                        break;

                }

                _changesHistoryBll.AddHistory(changes);
                isModified = true;
            }
            return isModified;
        }


        public POA_DELEGATIONDto GetById(int id)
        {
            var dtData = _repository.GetByID(id);
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<POA_DELEGATIONDto>(dtData);

        }

        public POA_DELEGATIONDto GetById(int id, bool isIncludeTable)
        {
            string include = "";
            if (isIncludeTable)
            {
                include = _includeTables;
                var dtData = _repository.Get(c => c.POA_DELEGATION_ID == id, null, include).FirstOrDefault();
                if (dtData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                return Mapper.Map<POA_DELEGATIONDto>(dtData);
            }

            return GetById(id);
        }

    }
}
