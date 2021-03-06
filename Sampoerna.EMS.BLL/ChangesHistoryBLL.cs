﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class ChangesHistoryBLL : IChangesHistoryBLL
    {

        private IGenericRepository<CHANGES_HISTORY> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "USER";

        public ChangesHistoryBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CHANGES_HISTORY>();
        }

        public CHANGES_HISTORY GetById(long id)
        {
            return _repository.Get(c => c.CHANGES_HISTORY_ID == id, null, includeTables).FirstOrDefault();
        }

        public List<CHANGES_HISTORY> GetByFormTypeId(Enums.MenuList formTypeId)
        {
            return _repository.Get(c => c.FORM_TYPE_ID == formTypeId, null, includeTables).ToList();
        }

        public List<CHANGES_HISTORY> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public void AddHistory(CHANGES_HISTORY history)
        {
            history.MODIFIED_DATE = DateTime.Now;
            _repository.Insert(history);
        }

       
        public List<CHANGES_HISTORY> GetByFormTypeAndFormId(Enums.MenuList formTypeId, string id)
        {
            return _repository.Get(c => c.FORM_TYPE_ID == formTypeId && c.FORM_ID == id, null, includeTables).OrderByDescending(c => c.MODIFIED_DATE).ToList();
        }

        public void DeleteByFormIdAndNewValue(string formId , string newValue)
        {
            var dbData =
                _repository.Get(c => c.FORM_ID == formId && c.NEW_VALUE == newValue, null,
                    includeTables).FirstOrDefault();

            if (dbData != null)
                _repository.Delete(dbData);

        }

        public void MoveHistoryToNewData(CHANGES_HISTORY history, string formId)
        {
            history.FORM_ID = formId;
            _repository.Update(history);
        }

        public void UpdateHistoryFormIdByFormType(string oldFormId, string newFormId, Enums.MenuList formType)
        {
            var dtData = _repository.Get(c => c.FORM_TYPE_ID == formType && c.FORM_ID == oldFormId).ToList();
            foreach (var changesHistory in dtData)
            {
                var changeHistory = _repository.GetByID(changesHistory.CHANGES_HISTORY_ID);
                changesHistory.FORM_ID = newFormId;
                _repository.Update(changesHistory);
            }
        }

        public List<CHANGES_HISTORY> GetByFormTypeAndListFormId(Enums.MenuList formTypeId, List<string> listId)
        {
            return _repository.Get(c => c.FORM_TYPE_ID == formTypeId && listId.Contains(c.FORM_ID) , null, includeTables).OrderByDescending(c => c.MODIFIED_DATE).ToList();
        }
    }
}
