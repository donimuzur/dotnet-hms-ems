using System;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using System.Collections.Generic;
using System.Linq;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class EmailTemplateBLL : IEmailTemplateBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<EMAIL_TEMPLATE> _repository;
        private IGenericRepository<WORKFLOW_STATE> _workflowStateRepository;
        private string _includeTables = "";

        public EmailTemplateBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<EMAIL_TEMPLATE>();
            _workflowStateRepository = _uow.GetGenericRepository<WORKFLOW_STATE>();
        }

        public List<EMAIL_TEMPLATE> getAllEmailTemplates()
        {
            return _repository.Get(null, null, _includeTables).ToList();
        }

        public EMAIL_TEMPLATE getEmailTemplateById(long id)
        {
            return _repository.GetByID(id);
        }

        public void Save(EMAIL_TEMPLATE record)
        {
            if (record.EMAIL_TEMPLATE_ID != 0)
            {
                _repository.InsertOrUpdate(record);

            }
            else {
                _repository.Insert(record);
            }

            _uow.SaveChanges();
        }

        public EMAIL_TEMPLATEDto GetByDocumentAndActionType(EmailTemplateGetByDocumentAndActionTypeInput input)
        {
            Expression<Func<WORKFLOW_STATE, bool>> queryFilter =
                c => c.ACTION == input.ActionType && c.FORM_TYPE_ID.HasValue && c.FORM_TYPE_ID.Value == input.FormType;

            var dataEmailTemplate = _workflowStateRepository.Get(queryFilter, null, "EMAIL_TEMPLATE").FirstOrDefault();

            return dataEmailTemplate != null && dataEmailTemplate.EMAIL_TEMPLATE != null ? Mapper.Map<EMAIL_TEMPLATEDto>(dataEmailTemplate.EMAIL_TEMPLATE) : null;

        }
        
    }
}
