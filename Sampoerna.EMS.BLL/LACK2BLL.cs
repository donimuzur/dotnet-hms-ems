using System.Configuration;
using System.Linq;
using System.Text;
using AutoMapper;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.MessagingService;
using Sampoerna.EMS.Utils;
using System;
using System.Collections.Generic;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class LACK2BLL : ILACK2BLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IPOABLL _poaBll;
        private IUserBLL _userBll;
        private IDocumentSequenceNumberBLL _docSeqNumBll;

        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IMessageService _messageService;
        private IWorkflowBLL _workflowBll;
        private IMonthBLL _monthBll;

        private ILack2Service _lack2Service;
        private ILack2ItemService _lack2ItemService;
        private ILack2DocumentService _lack2DocumentService;
        private IZaidmExNppbkcService _nppbkcService;
        private IExGroupTypeService _exGroupTypeService;
        private ICK5Service _ck5Service;
        private IT001WService _t001WService;
        private IT001Service _t001Service;
        private IExcisableGoodsTypeService _excisableGoodsTypeService;
        
        public LACK2BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _lack2Service = new Lack2Service(_uow, _logger);
            _nppbkcService = new ZaidmExNppbkcService(_uow, _logger);
            _exGroupTypeService = new ExGroupTypeService(_uow, _logger);
            _ck5Service = new CK5Service(_uow, _logger);
            _t001WService = new T001WService(_uow, _logger);
            _lack2ItemService = new Lack2ItemService(_uow, _logger);
            _lack2DocumentService = new Lack2DocumentService(_uow, _logger);
            _t001Service = new T001Service(_uow, _logger);
            _excisableGoodsTypeService = new ExcisableGoodsTypeService(_uow, _logger);

            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _messageService = new MessageService(_logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow,_logger);
            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
        }

        public List<Lack2Dto> GetAll()
        {
            var data = _lack2Service.GetAll();
            return Mapper.Map<List<Lack2Dto>>(data);
        }

        public List<Lack2Dto> GetByParam(Lack2GetByParamInput input)
        {
            if (input.UserRole == Enums.UserRole.POA)
            {
                var nppbkc = _nppbkcService.GetNppbkcsByPoa(input.UserId);
                if (nppbkc != null && nppbkc.Count > 0)
                {
                    input.NppbkcList = nppbkc.Select(c => c.NPPBKC_ID).ToList();
                }
                else
                {
                    input.NppbkcList = new List<string>();
                }
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poaBll.GetPOAIdByManagerId(input.UserId);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);
                input.DocumentNumberList = document;
            }
            
            return Mapper.Map<List<Lack2Dto>>(_lack2Service.GetByParam(input));
        }
        
        public List<Lack2Dto> GetCompletedByParam(Lack2GetByParamInput input)
        {
            var dbData = _lack2Service.GetCompletedByParam(input);
            var mapResult = Mapper.Map<List<Lack2Dto>>(dbData.ToList());
            return mapResult;
        }

        public Lack2Dto GetById(int id)
        {
            return Mapper.Map<Lack2Dto>(_lack2Service.GetById(id));
        }

        public Lack2DetailsDto GetDetailsById(int id)
        {
            return Mapper.Map<Lack2DetailsDto>(_lack2Service.GetDetailsById(id));
        }

        public Lack2CreateOutput Create(Lack2CreateParamInput input)
        {
            input.IsCreateNew = true;
            var generatedData = GenerateLack2Data(input);
            if (!generatedData.Success)
            {
                return new Lack2CreateOutput()
                {
                    Success = generatedData.Success,
                    ErrorCode = generatedData.ErrorCode,
                    ErrorMessage = generatedData.ErrorMessage,
                    Id = null,
                    Lack2Number = string.Empty
                };
            }

            //check if exists
            var isExists = IsExistLack2Data(input);
            if (!isExists.Success) return new Lack2CreateOutput()
            {
                Success = isExists.Success,
                ErrorCode = isExists.ErrorCode,
                ErrorMessage = isExists.ErrorMessage,
                Id = null,
                Lack2Number = string.Empty
            };

            var rc = new Lack2CreateOutput()
            {
                Success = false,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            var data = Mapper.Map<LACK2>(input);

            //set default when create new LACK-1 Document
            data.APPROVED_BY = null;
            data.APPROVED_DATE = null;
            data.APPROVED_BY_MANAGER = null;
            data.APPROVED_BY_MANAGER_DATE = null;
            data.DECREE_DATE = null;
            data.GOV_STATUS = null;
            data.STATUS = Enums.DocumentStatus.Draft;
            data.CREATED_DATE = DateTime.Now;
            data.MODIFIED_BY = null;
            data.MODIFIED_DATE = null;
            data.REJECTED_BY = null;
            data.REJECTED_DATE = null;

            //Set Company Detail
            var companyData = _t001Service.GetById(input.CompanyCode);
            if (companyData != null)
            {
                data.BUKRS = companyData.BUKRS;
                data.BUTXT = companyData.BUTXT;
            }

            //set plant detail
            var plantData = _t001WService.GetById(input.SourcePlantId);
            if (plantData != null)
            {
                data.LEVEL_PLANT_CITY = plantData.ORT01;
                data.LEVEL_PLANT_NAME = plantData.NAME1;
            }

            //set excisable goods type
            var excisableGoodsTypeData = _excisableGoodsTypeService.GetById(input.ExcisableGoodsType);
            if (excisableGoodsTypeData != null)
            {
                data.EX_GOOD_TYP = excisableGoodsTypeData.EXC_GOOD_TYP;
                data.EX_TYP_DESC = excisableGoodsTypeData.EXT_TYP_DESC;
            }

            //set from input, exclude on mapper
            data.LACK2_ITEM = Mapper.Map<List<LACK2_ITEM>>(generatedData.Data.Ck5Items);
            data.LACK2_DOCUMENT = null;

            //generate new Document Number get from Sequence Number BLL
            var generateNumberInput = new GenerateDocNumberInput()
            {
                Month = Convert.ToInt32(input.PeriodMonth),
                Year = Convert.ToInt32(input.PeriodYear),
                NppbkcId = input.NppbkcId
            };

            data.LACK2_NUMBER = _docSeqNumBll.GenerateNumber(generateNumberInput);

            _lack2Service.Insert(data);

            //add workflow history for create document
            var getUserRole = _poaBll.GetUserRole(input.UserId);
            AddWorkflowHistory(new Lack2WorkflowDocumentInput()
            {
                DocumentId = null,
                DocumentNumber = data.LACK2_NUMBER,
                ActionType = Enums.ActionType.Created,
                UserId = input.UserId,
                UserRole = getUserRole
            });

            _uow.SaveChanges();

            rc.Success = true;
            rc.ErrorCode = string.Empty;
            rc.Id = data.LACK2_ID;
            rc.Lack2Number = data.LACK2_NUMBER;

            return rc;
        }

        public Lack2GeneratedOutput GenerateLack2DataByParam(Lack2GenerateDataParamInput input)
        {
            return GenerateLack2Data(input);
        }

        public Lack2SaveEditOutput SaveEdit(Lack2SaveEditInput input)
        {
            bool isModified = false;
            var rc = new Lack2SaveEditOutput()
            {
                Success = false
            };

            if (input == null)
            {
                throw new Exception("Invalid data entry");
            }

            //origin
            var dbData = _lack2Service.GetDetailsById(input.Lack2Id);

            //Set Company Detail
            var companyData = _t001Service.GetById(input.CompanyCode);
            if (companyData != null)
            {
                input.CompanyCode = companyData.BUKRS;
                input.CompanyName = companyData.BUTXT;
            }

            //set plant detail
            var plantData = _t001WService.GetById(input.SourcePlantId);
            if (plantData != null)
            {
                input.SourcePlantCity = plantData.ORT01;
                input.SourcePlantName = plantData.NAME1;
            }

            //set excisable goods type
            var excisableGoodsTypeData = _excisableGoodsTypeService.GetById(input.ExcisableGoodsType);
            if (excisableGoodsTypeData != null)
            {
                input.ExcisableGoodsType = excisableGoodsTypeData.EXC_GOOD_TYP;
                input.ExcisableGoodsTypeDesc = excisableGoodsTypeData.EXT_TYP_DESC;
            }

            //check if need to regenerate
            var isNeedToRegenerate = dbData.STATUS == Enums.DocumentStatus.Draft || dbData.STATUS == Enums.DocumentStatus.Rejected;
            var generateInput = Mapper.Map<Lack2GenerateDataParamInput>(input);

            if (isNeedToRegenerate)
            {
                //do regenerate data
                generateInput.IsCreateNew = false;
                generateInput.Lack2Id = input.Lack2Id;
                var generatedData = GenerateLack2Data(generateInput);
                if (!generatedData.Success)
                {
                    return new Lack2SaveEditOutput()
                    {
                        Success = false,
                        ErrorCode = generatedData.ErrorCode,
                        ErrorMessage = generatedData.ErrorMessage
                    };
                }

                var origin = Mapper.Map<Lack2Dto>(dbData);
                var destination = Mapper.Map<Lack2Dto>(input);
                destination.Lack2Id = dbData.LACK2_ID;
                destination.Lack2Number = dbData.LACK2_NUMBER;
                
                isModified = SetChangesHistory(origin, destination, input.UserId);

                //delete first
                _lack2ItemService.DeleteDataList(dbData.LACK2_ITEM);
                _lack2DocumentService.DeleteDataList(dbData.LACK2_DOCUMENT);
                
                //set to null
                dbData.LACK2_ITEM = null;

                //set from input
                dbData.LACK2_ITEM = Mapper.Map<List<LACK2_ITEM>>(generatedData.Data.Ck5Items);
                
            }
            else
            {
                var origin = Mapper.Map<Lack2Dto>(dbData);
                var destination = Mapper.Map<Lack2Dto>(input);
                isModified = SetChangesHistory(origin, destination, input.UserId);
            }
            
            dbData.SUBMISSION_DATE = input.SubmissionDate;
            dbData.LEVEL_PLANT_ID = input.SourcePlantId;
            dbData.NPPBKC_ID = input.NppbkcId;
            dbData.PERIOD_MONTH = input.PeriodMonth;
            dbData.PERIOD_YEAR = input.PeriodYear;
            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;
            dbData.BUKRS = input.CompanyCode;
            dbData.BUTXT = input.CompanyName;
            dbData.LEVEL_PLANT_CITY = input.SourcePlantCity;
            dbData.LEVEL_PLANT_NAME = input.SourcePlantName;
            dbData.EX_GOOD_TYP = input.ExcisableGoodsType;
            dbData.EX_TYP_DESC = input.ExcisableGoodsTypeDesc;

            if (dbData.STATUS == Enums.DocumentStatus.Rejected)
            {
                //add history for changes status from rejected to draft
                WorkflowStatusAddChanges(new Lack2WorkflowDocumentInput(){ DocumentId = dbData.LACK2_ID, UserId = input.UserId }, dbData.STATUS, Enums.DocumentStatus.Draft);
                dbData.STATUS = Enums.DocumentStatus.Draft;
            }
            
            _uow.SaveChanges();

            rc.Success = true;
            rc.Id = dbData.LACK2_ID;
            rc.Lack2Number = dbData.LACK2_NUMBER;
            rc.IsModifiedHistory = isModified;
            //set workflow history
            var getUserRole = _poaBll.GetUserRole(input.UserId);

            var inputAddWorkflowHistory = new Lack2WorkflowDocumentInput()
            {
                DocumentId = rc.Id,
                DocumentNumber = rc.Lack2Number,
                ActionType = input.WorkflowActionType,
                UserId = input.UserId,
                UserRole = getUserRole
            };

            AddWorkflowHistory(inputAddWorkflowHistory);

            _uow.SaveChanges();

            return rc;
        }
        
        #region workflow

        //private void Lack2WorkflowAfterCompleted(Lack2WorkflowDocumentInput input)
        //{
             
        //}

        public void Lack2Workflow(Lack2WorkflowDocumentInput input)
        {
            var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    SubmitDocument(input);
                    break;
                case Enums.ActionType.Approve:
                    ApproveDocument(input);
                    break;
                case Enums.ActionType.Reject:
                    RejectDocument(input);
                    break;
                case Enums.ActionType.GovApprove:
                    GovApproveDocument(input);
                    //isNeedSendNotif = false;
                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocument(input);
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.GovPartialApprove:
                    GovPartialApproveDocument(input);
                    //isNeedSendNotif = false;
                    break;
            }

            //todo sent mail
            if (isNeedSendNotif)
                SendEmailWorkflow(input);
            _uow.SaveChanges();
        }

        private void AddWorkflowHistory(Lack2WorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.FORM_TYPE_ID = Enums.FormType.LACK2;

            if (!input.IsModified && input.ActionType == Enums.ActionType.Submit)
                _workflowHistoryBll.UpdateHistoryModifiedForSubmit(dbData);
            else
                _workflowHistoryBll.Save(dbData);

        }

        private void SubmitDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                //var dbData = _lack1Service.GetById(input.DocumentId.Value);
                var dbData = _lack2Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.Draft)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                if (dbData.CREATED_BY != input.UserId)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingForApproval);

                switch (input.UserRole)
                {
                    case Enums.UserRole.User:
                        dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;
                        break;
                    case Enums.UserRole.POA:
                        dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                        break;
                    default:
                        throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
                }

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void ApproveDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                //var dbData = _lack1Service.GetById(input.DocumentId.Value);

                var dbData = _lack2Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
                {
                    CreatedUser = dbData.CREATED_BY,
                    CurrentUser = input.UserId,
                    DocumentStatus = dbData.STATUS,
                    UserRole = input.UserRole,
                    NppbkcId = dbData.NPPBKC_ID,
                    DocumentNumber = dbData.LACK2_NUMBER
                });

                if (!isOperationAllow)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //todo: gk boleh loncat approval nya, creator->poa->manager atau poa(creator)->manager
                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;
                //Add Changes
                if (input.UserRole == Enums.UserRole.POA)
                {
                    WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingForApprovalManager);
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                    //dbData.APPROVED_BY_POA = input.UserId;
                    //dbData.APPROVED_DATE_POA = DateTime.Now;
                    dbData.APPROVED_BY = input.UserId;
                    dbData.APPROVED_DATE = DateTime.Now;
                }
                else
                {
                    WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);
                    dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                    dbData.APPROVED_BY_MANAGER = input.UserId;
                    dbData.APPROVED_BY_MANAGER_DATE = DateTime.Now;
                }

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void RejectDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack2Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                    dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                    dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);

                //todo ask
                if (dbData.STATUS == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    //manager reject
                    dbData.APPROVED_BY_MANAGER = null;
                    dbData.APPROVED_BY_MANAGER_DATE = null;
                }
                else
                {
                    //poa reject
                    dbData.APPROVED_BY = null;
                    dbData.APPROVED_DATE = null;
                }

                dbData.STATUS = Enums.DocumentStatus.Rejected;

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack2Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.FullApproved);
                WorkflowDecreeDateAddChanges(input.DocumentId, input.UserId, dbData.DECREE_DATE,
                    input.AdditionalDocumentData.DecreeDate);

                dbData.LACK2_DOCUMENT = null;
                dbData.STATUS = Enums.DocumentStatus.Completed;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
                dbData.LACK2_DOCUMENT = Mapper.Map<List<LACK2_DOCUMENT>>(input.AdditionalDocumentData.Lack2DecreeDoc);
                dbData.GOV_STATUS = Enums.DocumentStatusGov.FullApproved;

                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;

                //dbData.APPROVED_BY = input.UserId;
                //dbData.APPROVED_DATE = DateTime.Now;

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void GovPartialApproveDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack2Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.PartialApproved);
                WorkflowDecreeDateAddChanges(input.DocumentId, input.UserId, dbData.DECREE_DATE,
                    input.AdditionalDocumentData.DecreeDate);

                input.DocumentNumber = dbData.LACK2_NUMBER;

                dbData.LACK2_DOCUMENT = null;
                dbData.STATUS = Enums.DocumentStatus.Completed;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
                dbData.LACK2_DOCUMENT = Mapper.Map<List<LACK2_DOCUMENT>>(input.AdditionalDocumentData.Lack2DecreeDoc);
                dbData.GOV_STATUS = Enums.DocumentStatusGov.PartialApproved;

                //dbData.APPROVED_BY = input.UserId;
                //dbData.APPROVED_DATE = DateTime.Now;

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);
        }

        private void GovRejectedDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack2Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.Rejected);
                WorkflowDecreeDateAddChanges(input.DocumentId, input.UserId, dbData.DECREE_DATE,
                    input.AdditionalDocumentData.DecreeDate);
                
                dbData.STATUS = Enums.DocumentStatus.Rejected;
                dbData.GOV_STATUS = Enums.DocumentStatusGov.Rejected;
                dbData.LACK2_DOCUMENT = Mapper.Map<List<LACK2_DOCUMENT>>(input.AdditionalDocumentData.Lack2DecreeDoc);
                
                //set to null
                dbData.APPROVED_BY = null;
                dbData.APPROVED_BY_MANAGER = null;
                dbData.APPROVED_BY_MANAGER_DATE = null;
                dbData.APPROVED_DATE = null;

                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void WorkflowStatusAddChanges(Lack2WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK2,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = EnumHelper.GetDescription(oldStatus),
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };
            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowStatusGovAddChanges(Lack2WorkflowDocumentInput input, Enums.DocumentStatusGov? oldStatus, Enums.DocumentStatusGov newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK2,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "GOV_STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowDecreeDateAddChanges(int? documentId, string userId, DateTime? origin, DateTime? updated)
        {
            if (origin == updated) return;
            //set changes log
            if (documentId == null) return;
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK2,
                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                FORM_ID = documentId.Value.ToString(),
                FIELD_NAME = "Decree Date",
                NEW_VALUE = origin.HasValue ? origin.Value.ToString("dd-MM-yyyy") : "NULL",
                OLD_VALUE = updated.HasValue ? updated.Value.ToString("dd-MM-yyyy") : "NULL",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void SendEmailWorkflow(Lack2WorkflowDocumentInput input)
        {
            //todo: body message from email template
            //todo: to = ?
            //todo: subject = from email template
            //var to = "irmansulaeman41@gmail.com";
            //var subject = "this is subject for " + input.DocumentNumber;
            //var body = "this is body message for " + input.DocumentNumber;
            //var from = "a@gmail.com";

            if (input.DocumentId == null) return;

            var lack2Data = Mapper.Map<Lack2DetailsDto>(_lack2Service.GetDetailsById(input.DocumentId.Value));

            if ((input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
                && lack2Data.Status != Enums.DocumentStatus.Completed)
                return;

            var mailProcess = ProsesMailNotificationBody(lack2Data, input);

            List<string> listTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(listTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(listTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private string GetManagerEmail(string poaId)
        {
            var managerId = _poaBll.GetManagerIdByPoaId(poaId);
            var managerDetail = _userBll.GetUserById(managerId);
            return managerDetail.EMAIL;
        }

        private MailNotification ProsesMailNotificationBody(Lack2Dto lackData, Lack2WorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();

            var rejected = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(new GetByFormTypeAndFormIdInput()
            {
                FormId = lackData.Lack2Id,
                FormType = Enums.FormType.LACK2
            });

            var poaList = _poaBll.GetPoaByNppbkcId(lackData.NppbkcId);

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            rc.Subject = "LACK-2 " + lackData.Lack2Number + " is " + EnumHelper.GetDescription(lackData.Status);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + lackData.Burks + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + lackData.NppbkcId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + lackData.Lack2Number + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : LACK-2</td></tr>");
            bodyMail.AppendLine();
            if (input.ActionType == Enums.ActionType.Reject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/Lack2/Detail/" + lackData.Lack2Id + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (lackData.Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (rejected != null)
                        {
                            rc.To.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(lackData.CreatedBy).EMAIL);
                    }
                    else if (lackData.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var poaData = _poaBll.GetById(lackData.CreatedBy);
                        rc.To.Add(GetManagerEmail(lackData.CreatedBy));
                        rc.CC.Add(poaData.POA_EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            if (poaData.POA_ID != poaDto.POA_ID)
                                rc.To.Add(poaDto.POA_EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    if (lackData.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(lackData.ApprovedBy));

                        if (rejected != null)
                        {
                            rc.CC.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.CC.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(lackData.CreatedBy).EMAIL);

                    }
                    else if (lackData.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(lackData.CreatedBy);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(lackData.CreatedBy);
                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(_poaBll.GetById(lackData.ApprovedBy).POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(lackData.CreatedBy);
                    var poaData2 = _poaBll.GetById(lackData.CreatedBy);

                    if (lackData.ApprovedBy != null || poaData2 != null)
                    {
                        if (poaData2 == null)
                        {
                            var poa = _poaBll.GetById(lackData.ApprovedBy);
                            rc.To.Add(userDetail.EMAIL);
                            rc.CC.Add(poa.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                        }
                        else
                        {
                            rc.To.Add(poaData2.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                        }
                    }
                    else
                    {
                        rc.To.Add(userDetail.EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            rc.CC.Add(poaDto.POA_EMAIL);
                        }
                    }

                    rc.IsCCExist = true;
                    break;

                case Enums.ActionType.GovApprove:
                    var poaData3 = _poaBll.GetById(lackData.CreatedBy);
                    if (poaData3 != null)
                    {
                        //creator is poa user
                        rc.To.Add(poaData3.POA_EMAIL);
                        rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(lackData.CreatedBy);
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_poaBll.GetById(lackData.ApprovedBy).POA_EMAIL);
                        rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovPartialApprove:
                    var poaData4 = _poaBll.GetById(lackData.CreatedBy);
                    if (poaData4 != null)
                    {
                        //creator is poa user
                        rc.To.Add(poaData4.POA_EMAIL);
                        rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                    }
                    else
                    {
                        //creator is excise executive
                        //var userData = _userBll.GetUserById(lackData.CreatedBy);
                        //rc.To.Add(_poaBll.GetById(lackData.ApprovedBy).POA_EMAIL);
                        //rc.To.Add(GetManagerEmail(lackData.ApprovedBy));
                        //rc.CC.Add(userData.EMAIL);
                        var userData = _userBll.GetUserById(lackData.CreatedBy);
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_poaBll.GetById(lackData.ApprovedBy).POA_EMAIL);
                        rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovReject:
                    var poaData5 = _poaBll.GetById(lackData.CreatedBy);
                    if (poaData5 != null)
                    {
                        //creator is poa user
                        rc.To.Add(GetManagerEmail(lackData.CreatedBy));
                        rc.CC.Add(poaData5.POA_EMAIL);
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(lackData.CreatedBy);
                        rc.To.Add(_poaBll.GetById(lackData.ApprovedBy).POA_EMAIL);
                        rc.To.Add(GetManagerEmail(lackData.ApprovedBy));
                        rc.CC.Add(userData.EMAIL);
                    }
                    rc.IsCCExist = true;
                    break;

            }
            rc.Body = bodyMail.ToString();
            return rc;
        }
        
        #endregion

        #region private method

        private Lack2GeneratedOutput GenerateLack2Data(Lack2GenerateDataParamInput input)
        {
            var validationResult = ValidateOnGenerateLack2Data(ref input);
            if (!validationResult.Success) return validationResult;

            var rc = new Lack2GeneratedOutput
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Data = new Lack2GeneratedDto()
            };
            
            //get ck5 data
            var ck5Selected = _ck5Service.GetForLack2ByParam(new Ck5GetForLack2ByParamInput()
            {
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                SourcePlantId = input.SourcePlantId,
                ExGroupTypeId = input.ExGroupTypeId,
                CompanyCode = input.CompanyCode,
                NppbkcId = input.NppbkcId
            });

            if (ck5Selected.Count == 0)
            {
                return new Lack2GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.MissingCk5DataSelected.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingCk5DataSelected),
                    Data = null
                };
            }

            rc.Data.Ck5Items = Mapper.Map<List<Lack2GeneratedItemDto>>(ck5Selected);

            return rc;
        }

        private Lack2GeneratedOutput ValidateOnGenerateLack2Data(ref Lack2GenerateDataParamInput input)
        {
            var rc = new Lack2GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            #region validation

            //Check Excisable Group Type if exists
            var checkExcisableGroupType = _exGroupTypeService.GetGroupTypeDetailByGoodsType(input.ExcisableGoodsType);
            if (checkExcisableGroupType == null)
            {
                return new Lack2GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound),
                    Data = null
                };
            }

            if (checkExcisableGroupType.EX_GROUP_TYPE_ID.HasValue)
                input.ExGroupTypeId = checkExcisableGroupType.EX_GROUP_TYPE_ID.Value;
            
            #endregion

            return rc;

        }

        private Lack2GeneratedOutput IsExistLack2Data(Lack2GenerateDataParamInput input)
        {
            //check if already exists with same selection criteria
            var lackCheck = _lack2Service.GetBySelectionCriteria(new Lack2GetBySelectionCriteriaParamInput()
            {
                CompanyCode = input.CompanyCode,
                NppbkcId = input.NppbkcId,
                SourcePlantId = input.SourcePlantId,
                ExGoodTypeId = input.ExcisableGoodsType,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear
            });

            if (input.IsCreateNew)
            {
                if (lackCheck != null)
                {
                    return new Lack2GeneratedOutput()
                    {
                        Success = false,
                        ErrorCode = ExceptionCodes.BLLExceptions.Lack2DuplicateSelectionCriteria.ToString(),
                        ErrorMessage =
                            EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack2DuplicateSelectionCriteria),
                        Data = null
                    };
                }

            }
            else
            {
                if (lackCheck != null && lackCheck.LACK2_ID != input.Lack2Id)
                {
                    return new Lack2GeneratedOutput()
                    {
                        Success = false,
                        ErrorCode = ExceptionCodes.BLLExceptions.Lack2DuplicateSelectionCriteria.ToString(),
                        ErrorMessage =
                            EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack2DuplicateSelectionCriteria),
                        Data = null
                    };
                }
            }

            return new Lack2GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };
        }

        private bool SetChangesHistory(Lack2Dto origin, Lack2Dto data, string userId)
        {
            var rc = false;
            var changesData = new Dictionary<string, bool>
            {
                { "Company Code", origin.Burks == data.Burks },
                { "Company Name", origin.Butxt == data.Butxt },
                { "Period Month", origin.PeriodMonth == data.PeriodMonth },
                { "Period Year", origin.PeriodMonth == data.PeriodMonth },
                { "Submission Date", origin.SubmissionDate == data.SubmissionDate },
                { "NPPBKC ID", origin.NppbkcId == data.NppbkcId },
                { "Plant ID", origin.LevelPlantId == data.LevelPlantId },
                { "Plant Name", origin.LevelPlantName == data.LevelPlantName },
                { "Plant City", origin.LevelPlantCity == data.LevelPlantCity },
                { "Excisable Goods Type ID", origin.ExGoodTyp == data.ExGoodTyp },
                { "Excisable Goods Type Desc", origin.ExTypDesc == data.ExTypDesc }
            };

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Enums.MenuList.LACK2,
                        FORM_ID = data.Lack2Id.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "Company Code":
                            changes.OLD_VALUE = origin.Burks;
                            changes.NEW_VALUE = data.Burks;
                            break;
                        case "Company Name":
                            changes.OLD_VALUE = origin.Butxt;
                            changes.NEW_VALUE = data.Butxt;
                            break;
                        case "Period Month":
                            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                            changes.OLD_VALUE = origin.PeriodMonth.ToString();
                            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                            changes.NEW_VALUE = data.PeriodMonth.ToString();
                            break;
                        case "Period Year":
                            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                            changes.OLD_VALUE = origin.PeriodYear.ToString();
                            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                            changes.NEW_VALUE = data.PeriodYear.ToString();
                            break;
                        case "Submission Date":
                            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                            changes.OLD_VALUE = origin.SubmissionDate.ToString();
                            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                            changes.NEW_VALUE = data.SubmissionDate.ToString();
                            break;
                        case "NPPBKC ID":
                            changes.OLD_VALUE = origin.NppbkcId;
                            changes.NEW_VALUE = data.NppbkcId;
                            break;
                        case "Plant ID":
                            changes.OLD_VALUE = origin.LevelPlantId;
                            changes.NEW_VALUE = data.LevelPlantId;
                            break;
                        case "Plant Name":
                            changes.OLD_VALUE = origin.LevelPlantName;
                            changes.NEW_VALUE = data.LevelPlantName;
                            break;
                        case "Plant City":
                            changes.OLD_VALUE = origin.LevelPlantCity;
                            changes.NEW_VALUE = data.LevelPlantCity;
                            break;
                        case "Excisable Goods Type ID":
                            changes.OLD_VALUE = origin.ExGoodTyp;
                            changes.NEW_VALUE = data.ExGoodTyp;
                            break;
                        case "Excisable Goods Type Desc":
                            changes.OLD_VALUE = origin.ExTypDesc;
                            changes.NEW_VALUE = data.ExTypDesc;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                    rc = true;
                }
            }
            return rc;
        }

        #endregion

        #region Summary Report 

        public List<Lack2SummaryReportDto> GetSummaryReportsByParam(Lack2GetSummaryReportByParamInput input)
        {
            var rc = _lack2Service.GetSummaryReportsByParam(input);
            return SetDataSummaryReport(rc);
        }

        private List<Lack2SummaryReportDto> SetDataSummaryReport(IEnumerable<LACK2> listLack2)
        {
            var result = new List<Lack2SummaryReportDto>();

            foreach (var dtData in listLack2)
            {

                var summaryDto = new Lack2SummaryReportDto
                {
                    Lack2Number = dtData.LACK2_NUMBER,
                    DocumentType = EnumHelper.GetDescription(Enums.FormType.LACK2),
                    CompanyCode = dtData.BUKRS,
                    CompanyName = dtData.BUTXT,
                    NppbkcId = dtData.NPPBKC_ID,
                    Ck5SendingPlant = dtData.LEVEL_PLANT_ID
                };

                //var dbPlant = _plantBll.GetT001WById(dtData.LEVEL_PLANT_ID);
                var dbPlant = _t001WService.GetById(dtData.LEVEL_PLANT_ID);
                if (dbPlant != null)
                {
                    summaryDto.SendingPlantAddress = dbPlant.ADDRESS;
                }

                var monthData = _monthBll.GetMonth(dtData.PERIOD_MONTH);
                if (monthData != null)
                {
                    summaryDto.Lack2Period = monthData.MONTH_NAME_IND + " " + dtData.PERIOD_YEAR;
                }

                summaryDto.Lack2Date = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.SUBMISSION_DATE);

                summaryDto.TypeExcisableGoods = dtData.EX_GOOD_TYP;
                summaryDto.TypeExcisableGoodsDesc = dtData.EX_TYP_DESC;

                decimal total = dtData.LACK2_ITEM.Where(lack2Item => lack2Item.CK5 != null).Sum(lack2Item => lack2Item.CK5.GRAND_TOTAL_EX.HasValue ? lack2Item.CK5.GRAND_TOTAL_EX.Value : 0);
                summaryDto.TotalDeliveryExcisable = ConvertHelper.ConvertDecimalToStringMoneyFormat(total);

                foreach (var lack2Item in dtData.LACK2_ITEM.Where(lack2Item => lack2Item.CK5 != null))
                {
                    summaryDto.Uom = lack2Item.CK5.PACKAGE_UOM_ID;
                    break;
                }
                summaryDto.LegalizeData = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.DECREE_DATE);
                
                summaryDto.Poa = dtData.APPROVED_BY;
                summaryDto.PoaManager = dtData.APPROVED_BY_MANAGER;


                summaryDto.CreatedDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.CREATED_DATE);
                summaryDto.CreatedTime = ConvertHelper.ConvertDateToStringHHmm(dtData.CREATED_DATE);
                summaryDto.CreatedBy = dtData.CREATED_BY;

                summaryDto.ApprovedDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.APPROVED_DATE);
                summaryDto.ApprovedTime = ConvertHelper.ConvertDateToStringHHmm(dtData.APPROVED_DATE);
                summaryDto.ApprovedBy = dtData.APPROVED_BY;

                summaryDto.LastChangedDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.MODIFIED_DATE);
                summaryDto.LastChangedTime = ConvertHelper.ConvertDateToStringHHmm(dtData.MODIFIED_DATE);

                summaryDto.Status = EnumHelper.GetDescription(dtData.STATUS);

                //search
                summaryDto.PeriodYear = dtData.PERIOD_YEAR.ToString();
                result.Add(summaryDto);

            }

            return result;
        }

        #endregion

        #region Detail Report Summary

        public List<Lack2DetailReportDto> GetDetailReportsByParam(Lack2GetDetailReportByParamInput input)
        {

            var rc = _lack2Service.GetDetailReportsByParam(input);

            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var result = SetDataDetailReport(rc);

            if (input.DateFrom.HasValue)
            {
                input.DateFrom = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                result = result.Where(c => c.GiDate >= input.DateFrom).ToList();
            }

            if (!input.DateTo.HasValue) return result;

            input.DateFrom = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
            result = result.Where(c => c.GiDate <= input.DateTo).ToList();

            return result;

        }

        private List<Lack2DetailReportDto> SetDataDetailReport(IEnumerable<LACK2> listLack2)
        {
            var result = new List<Lack2DetailReportDto>();

            foreach (var dtData in listLack2)
            {
                foreach (var lack2Item in dtData.LACK2_ITEM)
                {
                    var summaryDto = new Lack2DetailReportDto();

                    summaryDto.Lack2Number = dtData.LACK2_NUMBER;

                    if (lack2Item.CK5 != null)
                    {
                        summaryDto.GiDate = lack2Item.CK5.GI_DATE;
                        summaryDto.Ck5GiDate = ConvertHelper.ConvertDateToStringddMMMyyyy(lack2Item.CK5.GI_DATE);
                        summaryDto.Ck5RegistrationNumber = lack2Item.CK5.REGISTRATION_NUMBER;
                        summaryDto.Ck5RegistrationDate = ConvertHelper.ConvertDateToStringddMMMyyyy(lack2Item.CK5.REGISTRATION_DATE);
                        summaryDto.Ck5Total = ConvertHelper.ConvertDecimalToStringMoneyFormat(lack2Item.CK5.GRAND_TOTAL_EX);

                        summaryDto.ReceivingCompanyCode = lack2Item.CK5.DEST_PLANT_COMPANY_CODE;
                        summaryDto.ReceivingCompanyName = lack2Item.CK5.DEST_PLANT_COMPANY_NAME;
                        summaryDto.ReceivingNppbkc = lack2Item.CK5.DEST_PLANT_NPPBKC_ID;
                        summaryDto.ReceivingAddress = lack2Item.CK5.DEST_PLANT_ADDRESS;
                    }

                    summaryDto.Ck5SendingPlant = dtData.LEVEL_PLANT_ID;
                    var dbPlant = _t001WService.GetById(dtData.LEVEL_PLANT_ID);
                    if (dbPlant != null)
                    {
                        summaryDto.SendingPlantAddress = dbPlant.ADDRESS;
                    }
                    summaryDto.CompanyCode = dtData.BUKRS;
                    summaryDto.CompanyName = dtData.BUTXT;
                    summaryDto.NppbkcId = dtData.NPPBKC_ID;
                    summaryDto.TypeExcisableGoods = dtData.EX_GOOD_TYP;
                    summaryDto.TypeExcisableGoodsDesc = dtData.EX_TYP_DESC;

                    result.Add(summaryDto);

                }
            }

            return result;
        }

        #endregion

    }
}
