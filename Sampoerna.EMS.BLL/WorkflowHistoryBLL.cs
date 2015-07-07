using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class WorkflowHistoryBLL : IWorkflowHistoryBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<WORKFLOW_HISTORY> _repository;

        public WorkflowHistoryBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<WORKFLOW_HISTORY>();
        }

        public WORKFLOW_HISTORY GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public WORKFLOW_HISTORY GetByActionAndFormId(Enums.ActionType actionType, long formId)
        {
            return _repository.Get(w=> w.ACTION == actionType && w.FORM_ID == formId).FirstOrDefault();
        }

        public WORKFLOW_HISTORY GetSpecificWorkflowHistory(Enums.FormType formType, long formId,
            Enums.ActionType actionType)
        {
            return
                _repository.Get(c => c.FORM_TYPE_ID == formType && c.FORM_ID == formId && c.ACTION == actionType)
                    .FirstOrDefault();
        }

        public void AddHistory(WORKFLOW_HISTORY history)
        {
            _repository.Insert(history);
        }

        public void Save(WORKFLOW_HISTORY history)
        {
            _repository.InsertOrUpdate(history);
        }

        public List<WORKFLOW_HISTORY> GetByFormTypeAndFormId(Enums.FormType formTypeId, long id)
        {
            return _repository.Get(c => c.FORM_TYPE_ID == formTypeId && c.FORM_ID == id, null, "USER").ToList();
        }

    }
}
