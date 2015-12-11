using System.Collections.Generic;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Test
{
    [TestClass]
    public class EmailTemplateBLLTest
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<EMAIL_TEMPLATE> _repository;
        private IGenericRepository<WORKFLOW_STATE> _workflowStateRepository;
        private IEmailTemplateBLL _bll;

        [TestInitialize]
        public void SetUp()
        {
            BLLMapper.Initialize();
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = _uow.GetGenericRepository<EMAIL_TEMPLATE>();
            _workflowStateRepository = _uow.GetGenericRepository<WORKFLOW_STATE>();
            _bll = new EmailTemplateBLL(_uow, _logger);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _uow = null;
            _repository = null;
            _workflowStateRepository = null;
            _bll = null;
        }

        [TestMethod]
        public void GetByDocumentAndActionType_WhenNull_ReturnNull()
        {
            //arr
            var input = new EmailTemplateGetByDocumentAndActionTypeInput()
            {
                FormType = Core.Enums.FormType.PBCK1,
                ActionType = Core.Enums.ActionType.Submit
            };

            var workflowState = new WORKFLOW_STATE()
            {
                WORKFLOW_STATE_ID = 1,
                EMAIL_TEMPLATE_ID = 1,
                ACTION = Core.Enums.ActionType.Approve,
                FORM_TYPE_ID = Core.Enums.FormType.PBCK1,
                EMAIL_TEMPLATE = null
            };

            _workflowStateRepository.Get().ReturnsForAnyArgs(new List<WORKFLOW_STATE>() { workflowState });

            //act
            var result = _bll.GetByDocumentAndActionType(input);

            //assert
            Assert.AreEqual(null, result);

        }

        [TestMethod]
        public void GetByDocumentAndActionType_WhenDataExists_ReturnData()
        {
            //arr
            var input = new EmailTemplateGetByDocumentAndActionTypeInput()
            {
                FormType = Core.Enums.FormType.PBCK1,
                ActionType = Core.Enums.ActionType.Submit
            };

            var emailTemplate = new EMAIL_TEMPLATEDto()
            {
                EMAIL_TEMPLATE_ID = 1,
                TEMPLATE_NAME = "template name",
                SUBJECT = "subject",
                BODY = "body"
            };

            var workflowState = new WORKFLOW_STATE()
            {
                WORKFLOW_STATE_ID = 1,
                EMAIL_TEMPLATE_ID = 1,
                ACTION = Core.Enums.ActionType.Submit,
                FORM_TYPE_ID = Core.Enums.FormType.PBCK1,
                EMAIL_TEMPLATE = Mapper.Map<EMAIL_TEMPLATE>(emailTemplate)
            };

            _workflowStateRepository.Get().ReturnsForAnyArgs(new List<WORKFLOW_STATE>() { workflowState });

            //act
            var result = _bll.GetByDocumentAndActionType(input);

            //assert
            Assert.AreEqual(emailTemplate.TEMPLATE_NAME, result.TEMPLATE_NAME);

        }

    }
}
