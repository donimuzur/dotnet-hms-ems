using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class WorkflowHistoryBLL : IWorkflowHistoryBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<WORKFLOW_HISTORY> _repository;

        private string includeTables = "USER";

        public WorkflowHistoryBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<WORKFLOW_HISTORY>();
        }

        public WorkflowHistoryDto GetById(long id)
        {
            return Mapper.Map<WorkflowHistoryDto>(_repository.Get(c => c.WORKFLOW_HISTORY_ID == id, null, includeTables).FirstOrDefault());
        }

        public List<WorkflowHistoryDto> GetByFormTypeAndFormId(GetByFormTypeAndFormIdInput input)
        {
            var dbData =
                _repository.Get(c => c.FORM_TYPE_ID == input.FormType && c.FORM_ID == input.FormId, null, includeTables)
                    .ToList();
            return Mapper.Map<List<WorkflowHistoryDto>>(dbData);
        }

        public WorkflowHistoryDto GetByActionAndFormNumber(GetByActionAndFormNumberInput input)
        {
            var dbData =
                _repository.Get(c => c.ACTION == input.ActionType && c.FORM_NUMBER == input.FormNumber, null,
                    includeTables).FirstOrDefault();
            return Mapper.Map<WorkflowHistoryDto>(dbData);
        }


        public List<WorkflowHistoryDto> GetByFormNumber(string formNumber)
        {
            var dbData =
                _repository.Get(c => c.FORM_NUMBER == formNumber, null, includeTables)
                    .ToList();
            return Mapper.Map<List<WorkflowHistoryDto>>(dbData);
        }


        public void AddHistory(WorkflowHistoryDto history)
        {
            var dbData = Mapper.Map<WORKFLOW_HISTORY>(history);
            _repository.Insert(dbData);
        }

        public void Save(WorkflowHistoryDto history)
        {
            WORKFLOW_HISTORY dbData = null;
            if (history.WORKFLOW_HISTORY_ID > 0)
            {
                dbData = _repository.GetByID(history.WORKFLOW_HISTORY_ID);
                Mapper.Map<WorkflowHistoryDto, WORKFLOW_HISTORY>(history, dbData);
            }
            else
            {
                dbData = Mapper.Map<WORKFLOW_HISTORY>(history);
                _repository.Insert(dbData);
            }
        }
        
    }
}
