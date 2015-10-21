using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.LinqExtensions;
using Sampoerna.EMS.MessagingService;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using AutoMapper;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class LACK1BLL : ILACK1BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private IPOABLL _poaBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IHeaderFooterBLL _headerFooterBll;
        private IWorkflowBLL _workflowBll;
        private IUserBLL _userBll;

        //services
        private ICK4CItemService _ck4cItemService;
        private ICK5Service _ck5Service;
        private IPBCK1Service _pbck1Service;
        private IT001KService _t001KService;
        private ILACK1Service _lack1Service;
        private IT001WService _t001WServices;
        private IExGroupTypeService _exGroupTypeService;
        private IInventoryMovementService _inventoryMovementService;
        private IMessageService _messageService;
        private ILack1IncomeDetailService _lack1IncomeDetailService;
        private ILack1Pbck1MappingService _lack1Pbck1MappingService;
        private ILack1PlantService _lack1PlantService;
        private ILack1ProductionDetailService _lack1ProductionDetailService;
        private ILack1TrackingService _lack1TrackingService;
        private IZaidmExProdTypeService _prodTypeService;
        private IZaidmExNppbkcService _nppbkcService;

        public LACK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
            _headerFooterBll = new HeaderFooterBLL(_uow, _logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);

            _ck4cItemService = new CK4CItemService(_uow, _logger);
            _ck5Service = new CK5Service(_uow, _logger);
            _pbck1Service = new PBCK1Service(_uow, _logger);
            _t001KService = new T001KService(_uow, _logger);
            _lack1Service = new LACK1Service(_uow, _logger);
            _t001WServices = new T001WService(_uow, _logger);
            _exGroupTypeService = new ExGroupTypeService(_uow, _logger);
            _inventoryMovementService = new InventoryMovementService(_uow, _logger);
            _messageService = new MessageService(_logger);
            _lack1IncomeDetailService = new Lack1IncomeDetailService(_uow, _logger);
            _lack1Pbck1MappingService = new Lack1Pbck1MappingService(_uow, _logger);
            _lack1PlantService = new Lack1PlantService(_uow, _logger);
            _lack1ProductionDetailService = new Lack1ProductionDetailService(_uow, _logger);
            _lack1TrackingService = new Lack1TrackingService(_uow, _logger);
            _prodTypeService = new ZaidmExProdTypeService(_uow, _logger);
            _nppbkcService = new ZaidmExNppbkcService(_uow, _logger);
        }

        public List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input)
        {
            if (input.UserRole == Enums.UserRole.POA)
            {
                var nppbkc = _nppbkcService.GetNppbkcsByPoa(input.UserId);
                if (nppbkc != null && nppbkc.Count > 0)
                {
                    input.NppbkcList = nppbkc.Select(c => c.NPPBKC_ID).ToList();
                }
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poaBll.GetPOAIdByManagerId(input.UserId);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);
                input.DocumentNumberList = document;
            }
            return Mapper.Map<List<Lack1Dto>>(_lack1Service.GetAllByParam(input));
        }

        public List<Lack1Dto> GetCompletedDocumentByParam(Lack1GetByParamInput input)
        {
            var dbData = _lack1Service.GetCompletedDocumentByParam(input);
            var mapResult = Mapper.Map<List<Lack1Dto>>(dbData.ToList());
            return mapResult;
        }

        public Lack1CreateOutput Create(Lack1CreateParamInput input)
        {
            input.IsCreateNew = true;
            var generatedData = GenerateLack1Data(input);
            if (!generatedData.Success)
            {
                return new Lack1CreateOutput()
                {
                    Success = generatedData.Success,
                    ErrorCode = generatedData.ErrorCode,
                    ErrorMessage = generatedData.ErrorMessage,
                    Id = null,
                    Lack1Number = string.Empty
                };
            }
            
            //check if exists
            var isExists = IsExistLack1Data(input);
            if (!isExists.Success) return new Lack1CreateOutput()
            {
                Success = isExists.Success,
                ErrorCode = isExists.ErrorCode,
                ErrorMessage = isExists.ErrorMessage,
                Id = null,
                Lack1Number = string.Empty
            };

            var rc = new Lack1CreateOutput()
            {
                Success = false,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            var data = Mapper.Map<LACK1>(generatedData.Data);

            //set default when create new LACK-1 Document
            data.APPROVED_BY_POA = null;
            data.APPROVED_DATE_POA = null;
            data.APPROVED_BY_MANAGER = null;
            data.APPROVED_DATE_MANAGER = null;
            data.DECREE_DATE = null;
            data.GOV_STATUS = null;
            data.STATUS = Enums.DocumentStatus.Draft;
            data.CREATED_DATE = DateTime.Now;

            //set from input, exclude on mapper
            data.CREATED_BY = input.UserId;
            data.LACK1_LEVEL = input.Lack1Level;
            data.SUBMISSION_DATE = input.SubmissionDate;
            data.WASTE_QTY = input.WasteAmount;
            data.WASTE_UOM = input.WasteAmountUom;
            data.RETURN_QTY = input.ReturnAmount;
            data.RETURN_UOM = input.ReturnAmountUom;

            //set LACK1_TRACKING
            var allTrackingList = generatedData.Data.InvMovementAllList;
            allTrackingList.AddRange(generatedData.Data.InvMovementReceivingList);
            data.LACK1_TRACKING = Mapper.Map<List<LACK1_TRACKING>>(allTrackingList);

            //generate new Document Number get from Sequence Number BLL
            var generateNumberInput = new GenerateDocNumberInput()
            {
                Month = Convert.ToInt32(input.PeriodMonth),
                Year = Convert.ToInt32(input.PeriodYear),
                NppbkcId = input.NppbkcId
            };
            data.LACK1_NUMBER = _docSeqNumBll.GenerateNumber(generateNumberInput);

            data.LACK1_PLANT = null;

            //set LACK1_PLANT table
            if (input.Lack1Level == Enums.Lack1Level.Nppbkc)
            {
                var plantListFromMaster = _t001WServices.GetByNppbkcId(input.NppbkcId);
                data.LACK1_PLANT = Mapper.Map<List<LACK1_PLANT>>(plantListFromMaster);
            }
            else
            {
                var plantFromMaster = _t001WServices.GetById(input.ReceivedPlantId);
                data.LACK1_PLANT = new List<LACK1_PLANT>() { Mapper.Map<LACK1_PLANT>(plantFromMaster) };
            }

            _lack1Service.Insert(data);

            //add workflow history for create document
            var getUserRole = _poaBll.GetUserRole(input.UserId);
            AddWorkflowHistory(new Lack1WorkflowDocumentInput()
            {
                DocumentId = null,
                DocumentNumber = data.LACK1_NUMBER,
                ActionType = Enums.ActionType.Created,
                UserId = input.UserId,
                UserRole = getUserRole
            });

            _uow.SaveChanges();

            rc.Success = true;
            rc.ErrorCode = string.Empty;
            rc.Id = data.LACK1_ID;
            rc.Lack1Number = data.LACK1_NUMBER;

            return rc;
        }

        public SaveLack1Output SaveEdit(Lack1SaveEditInput input)
        {
            bool isModified = false;
            var rc = new SaveLack1Output()
            {
                Success = false
            };

            if (input == null)
            {
                throw new Exception("Invalid data entry");
            }

            //origin
            var dbData = _lack1Service.GetDetailsById(input.Detail.Lack1Id);

            //check if need to regenerate
            var isNeedToRegenerate = dbData.STATUS == Enums.DocumentStatus.Draft || dbData.STATUS == Enums.DocumentStatus.Rejected;
            var generateInput = Mapper.Map<Lack1GenerateDataParamInput>(input);
            //if (!isNeedToRegenerate)
            //{
            //    isNeedToRegenerate = IsNeedToRegenerate(generateInput, dbData);
            //}
            
            if (isNeedToRegenerate)
            {
                //do regenerate data
                generateInput.IsCreateNew = false;
                var generatedData = GenerateLack1Data(generateInput);
                if (!generatedData.Success)
                {
                    return new SaveLack1Output()
                    {
                        Success = false,
                        ErrorCode = generatedData.ErrorCode,
                        ErrorMessage = generatedData.ErrorMessage
                    };
                }

                var origin = Mapper.Map<Lack1DetailsDto>(dbData);
                var destination = Mapper.Map<Lack1DetailsDto>(generatedData.Data);

                destination.Lack1Id = dbData.LACK1_ID;
                destination.Lack1Number = dbData.LACK1_NUMBER;
                destination.Lack1Level = dbData.LACK1_LEVEL;
                destination.SubmissionDate = input.Detail.SubmissionDate;
                destination.SupplierCompanyName = origin.SupplierCompanyName;
                destination.SupplierCompanyCode = origin.SupplierCompanyCode;
                destination.WasteQty = input.Detail.WasteQty;
                destination.WasteUom = input.Detail.WasteUom;
                destination.WasteUomDesc = input.Detail.WasteUomDesc;
                destination.ReturnQty = input.Detail.ReturnQty;
                destination.ReturnUom = input.Detail.ReturnUom;
                destination.ReturnUomDesc = input.Detail.ReturnUomDesc;
                destination.Status = input.Detail.Status;
                destination.GovStatus = input.Detail.GovStatus;
                destination.DecreeDate = input.Detail.DecreeDate;

                isModified = SetChangesHistory(origin, destination, input.UserId);

                //delete first
                _lack1TrackingService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1IncomeDetailService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1Pbck1MappingService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1PlantService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1ProductionDetailService.DeleteByLack1Id(dbData.LACK1_ID);

                //regenerate
                Mapper.Map<Lack1GeneratedDto, LACK1>(generatedData.Data, dbData);

                //set to null
                dbData.LACK1_INCOME_DETAIL = null;
                dbData.LACK1_PBCK1_MAPPING = null;
                dbData.LACK1_PLANT = null;
                dbData.LACK1_PRODUCTION_DETAIL = null;
                dbData.LACK1_TRACKING = null;

                //set from input
                dbData.LACK1_INCOME_DETAIL = Mapper.Map<List<LACK1_INCOME_DETAIL>>(generatedData.Data.IncomeList);
                dbData.LACK1_PBCK1_MAPPING = Mapper.Map<List<LACK1_PBCK1_MAPPING>>(generatedData.Data.Pbck1List);
                dbData.LACK1_PRODUCTION_DETAIL =
                    Mapper.Map<List<LACK1_PRODUCTION_DETAIL>>(generatedData.Data.ProductionList);

                //set LACK1_TRACKING
                var allTrackingList = generatedData.Data.InvMovementAllList;
                allTrackingList.AddRange(generatedData.Data.InvMovementReceivingList);
                dbData.LACK1_TRACKING = Mapper.Map<List<LACK1_TRACKING>>(allTrackingList);

                //set LACK1_PLANT table
                if (input.Detail.Lack1Level == Enums.Lack1Level.Nppbkc)
                {
                    var plantListFromMaster = _t001WServices.GetByNppbkcId(input.Detail.NppbkcId);
                    dbData.LACK1_PLANT = Mapper.Map<List<LACK1_PLANT>>(plantListFromMaster);
                }
                else
                {
                    var plantFromMaster = _t001WServices.GetById(input.Detail.LevelPlantId);
                    dbData.LACK1_PLANT = new List<LACK1_PLANT>() {Mapper.Map<LACK1_PLANT>(plantFromMaster)};
                }
            }
            else
            {
                var origin = Mapper.Map<Lack1DetailsDto>(dbData);

                isModified = SetChangesHistory(origin, input.Detail, input.UserId);
            }
            
            dbData.SUBMISSION_DATE = input.Detail.SubmissionDate;
            dbData.LACK1_LEVEL = input.Detail.Lack1Level;
            dbData.WASTE_QTY = input.Detail.WasteQty;
            dbData.WASTE_UOM = input.Detail.WasteUom;
            dbData.RETURN_QTY = input.Detail.ReturnQty;
            dbData.RETURN_UOM = input.Detail.ReturnUom;
            dbData.NOTED = input.Detail.Noted;
            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData.STATUS == Enums.DocumentStatus.Rejected)
            {
                //add history for changes status from rejected to draft
                WorkflowStatusAddChanges(new Lack1WorkflowDocumentInput() { DocumentId = dbData.LACK1_ID, UserId = input.UserId }, dbData.STATUS, Enums.DocumentStatus.Draft);
                dbData.STATUS = Enums.DocumentStatus.Draft;
            }

            _uow.SaveChanges();

            rc.Success = true;
            rc.Id = dbData.LACK1_ID;
            rc.Lack1Number = dbData.LACK1_NUMBER;
            rc.IsModifiedHistory = isModified;

            //set workflow history
            var getUserRole = _poaBll.GetUserRole(input.UserId);

            var inputAddWorkflowHistory = new Lack1WorkflowDocumentInput()
            {
                DocumentId = rc.Id,
                DocumentNumber = rc.Lack1Number,
                ActionType = input.WorkflowActionType,
                UserId = input.UserId,
                UserRole = getUserRole
            };

            AddWorkflowHistory(inputAddWorkflowHistory);

            _uow.SaveChanges();

            return rc;

        }
        
        public Lack1DetailsDto GetDetailsById(int id)
        {
            var dbData = _lack1Service.GetDetailsById(id);
            return Mapper.Map<Lack1DetailsDto>(dbData);
        }

        public decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input)
        {
            return _lack1Service.GetLatestSaldoPerPeriod(input);
        }

        #region workflow

        public void Lack1Workflow(Lack1WorkflowDocumentInput input)
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

        private void AddWorkflowHistory(Lack1WorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.FORM_TYPE_ID = Enums.FormType.LACK1;

            if (!input.IsModified && input.ActionType == Enums.ActionType.Submit)
                _workflowHistoryBll.UpdateHistoryModifiedForSubmit(dbData);
            else
                _workflowHistoryBll.Save(dbData);

        }

        private void SubmitDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

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

                input.DocumentNumber = dbData.LACK1_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void ApproveDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //todo: gk boleh loncat approval nya, creator->poa->manager atau poa(creator)->manager
                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;
                
                if (input.UserRole == Enums.UserRole.POA)
                {
                    if (dbData.STATUS == Enums.DocumentStatus.WaitingForApproval)
                    {
                        //Add Changes
                        WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingForApprovalManager);
                        dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                        dbData.APPROVED_BY_POA = input.UserId;
                        dbData.APPROVED_DATE_POA = DateTime.Now;
                    }
                    else
                    {
                        throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
                    }
                }
                else
                {
                    //manager
                    if (dbData.STATUS == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        //Add Changes
                        WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);
                        dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                        dbData.APPROVED_BY_MANAGER = input.UserId;
                        dbData.APPROVED_DATE_MANAGER = DateTime.Now;
                    }
                    else
                    {
                        throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
                    }
                }

                input.DocumentNumber = dbData.LACK1_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void RejectDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                    dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                    dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);

                //change back to draft
                dbData.STATUS = Enums.DocumentStatus.Rejected;

                //todo ask
                dbData.APPROVED_BY_POA = null;
                dbData.APPROVED_DATE_POA = null;

                input.DocumentNumber = dbData.LACK1_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.FullApproved);

                dbData.LACK1_DOCUMENT = null;
                dbData.STATUS = Enums.DocumentStatus.Completed;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
                dbData.LACK1_DOCUMENT = Mapper.Map<List<LACK1_DOCUMENT>>(input.AdditionalDocumentData.Lack1Document);
                dbData.GOV_STATUS = Enums.DocumentStatusGov.FullApproved;

                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;

                input.DocumentNumber = dbData.LACK1_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void GovPartialApproveDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.PartialApproved);

                input.DocumentNumber = dbData.LACK1_NUMBER;

                dbData.LACK1_DOCUMENT = null;
                dbData.STATUS = Enums.DocumentStatus.Completed;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
                dbData.LACK1_DOCUMENT = Mapper.Map<List<LACK1_DOCUMENT>>(input.AdditionalDocumentData.Lack1Document);
                dbData.GOV_STATUS = Enums.DocumentStatusGov.PartialApproved;

                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;

                input.DocumentNumber = dbData.LACK1_NUMBER;
            }

            AddWorkflowHistory(input);
        }

        private void GovRejectedDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.GovRejected);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.Rejected);

                dbData.STATUS = Enums.DocumentStatus.GovRejected;
                dbData.GOV_STATUS = Enums.DocumentStatusGov.Rejected;
                dbData.LACK1_DOCUMENT = Mapper.Map<List<LACK1_DOCUMENT>>(input.AdditionalDocumentData.Lack1Document);
                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;

                input.DocumentNumber = dbData.LACK1_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void WorkflowStatusAddChanges(Lack1WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK1,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = EnumHelper.GetDescription(oldStatus),
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };
            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowStatusGovAddChanges(Lack1WorkflowDocumentInput input, Enums.DocumentStatusGov? oldStatus, Enums.DocumentStatusGov newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK1,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "GOV_STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void SendEmailWorkflow(Lack1WorkflowDocumentInput input)
        {
            //todo: body message from email template
            //todo: to = ?
            //todo: subject = from email template
            //var to = "irmansulaeman41@gmail.com";
            //var subject = "this is subject for " + input.DocumentNumber;
            //var body = "this is body message for " + input.DocumentNumber;
            //var from = "a@gmail.com";

            if (input.DocumentId == null) return;

            var lack1Data = Mapper.Map<Lack1DetailsDto>(_lack1Service.GetDetailsById(input.DocumentId.Value));

            if ((input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
                && lack1Data.Status != Enums.DocumentStatus.Completed)
                return;

            var mailProcess = ProsesMailNotificationBody(lack1Data, input);

            _messageService.SendEmailToList(mailProcess.To, mailProcess.Subject, mailProcess.Body, true);
        }

        private MailNotification ProsesMailNotificationBody(Lack1DetailsDto lack1Data, Lack1WorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();

            var rejected = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(new GetByFormTypeAndFormIdInput()
            {
                FormId = lack1Data.Lack1Id,
                FormType = Enums.FormType.LACK1
            });

            var poaList = _poaBll.GetPoaByNppbkcId(lack1Data.NppbkcId);

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            rc.Subject = "LACK-1 " + lack1Data.Lack1Number + " is " + EnumHelper.GetDescription(lack1Data.Status);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + lack1Data.Bukrs + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + lack1Data.NppbkcId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + lack1Data.Lack1Number + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : LACK-1</td></tr>");
            bodyMail.AppendLine();
            if (input.ActionType == Enums.ActionType.Reject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/Lack1/Details/" + lack1Data.Lack1Id + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (lack1Data.Status == Enums.DocumentStatus.WaitingForApproval)
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

                        rc.CC.Add(_userBll.GetUserById(lack1Data.CreateBy).EMAIL);
                    }
                    else if (lack1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var poaData = _poaBll.GetById(lack1Data.CreateBy);
                        rc.To.Add(GetManagerEmail(lack1Data.CreateBy));
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
                    if (lack1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(lack1Data.ApprovedByPoa));

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

                        rc.CC.Add(_userBll.GetUserById(lack1Data.CreateBy).EMAIL);

                    }
                    else if (lack1Data.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(lack1Data.CreateBy);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lack1Data.CreateBy));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(lack1Data.CreateBy);
                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lack1Data.ApprovedByPoa));
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    ////send notification to creator
                    //var userDetail = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                    //rc.To.Add(userDetail.EMAIL);

                    ////rejected by poa or manager
                    //rc.CC.Add(_userBll.GetUserById(input.UserId).EMAIL);

                    //if (input.UserRole == Enums.UserRole.Manager) //rejected by manager
                    //{
                    //    //add cc poa
                    //    if (pbck4Dto.APPROVED_BY_POA != null)
                    //    {
                    //        rc.CC.Add(_userBll.GetUserById(pbck4Dto.APPROVED_BY_POA).EMAIL);
                    //    }
                    //}

                    //rc.IsCCExist = true;
                    //break;
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(lack1Data.CreateBy);
                    var poaData2 = _poaBll.GetById(lack1Data.CreateBy);

                    if (lack1Data.ApprovedByPoa != null || poaData2 != null)
                    {
                        if (poaData2 == null)
                        {
                            var poa = _poaBll.GetById(lack1Data.ApprovedByPoa);
                            rc.To.Add(userDetail.EMAIL);
                            rc.CC.Add(poa.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lack1Data.ApprovedByPoa));
                        }
                        else
                        {
                            rc.To.Add(poaData2.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(lack1Data.CreateBy));
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
                    var poaData3 = _poaBll.GetById(lack1Data.CreateBy);
                    if (poaData3 != null)
                    {
                        //creator is poa user
                        rc.To.Add(GetManagerEmail(lack1Data.CreateBy));
                        rc.CC.Add(poaData3.POA_EMAIL);
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(lack1Data.CreateBy);
                        rc.To.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                        rc.To.Add(GetManagerEmail(lack1Data.ApprovedByPoa));
                        rc.CC.Add(userData.EMAIL);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovPartialApprove:
                    var poaData4 = _poaBll.GetById(lack1Data.CreateBy);
                    if (poaData4 != null)
                    {
                        //creator is poa user
                        rc.To.Add(GetManagerEmail(lack1Data.CreateBy));
                        rc.CC.Add(poaData4.POA_EMAIL);
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(lack1Data.CreateBy);
                        rc.To.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                        rc.To.Add(GetManagerEmail(lack1Data.ApprovedByPoa));
                        rc.CC.Add(userData.EMAIL);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovReject:
                    var poaData5 = _poaBll.GetById(lack1Data.CreateBy);
                    if (poaData5 != null)
                    {
                        //creator is poa user
                        rc.To.Add(GetManagerEmail(lack1Data.CreateBy));
                        rc.CC.Add(poaData5.POA_EMAIL);
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(lack1Data.CreateBy);
                        rc.To.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                        rc.To.Add(GetManagerEmail(lack1Data.ApprovedByPoa));
                        rc.CC.Add(userData.EMAIL);
                    }
                    rc.IsCCExist = true;
                    break;

            }
            rc.Body = bodyMail.ToString();
            return rc;
        }

        private string GetManagerEmail(string poaId)
        {
            var managerId = _poaBll.GetManagerIdByPoaId(poaId);
            var managerDetail = _userBll.GetUserById(managerId);
            return managerDetail.EMAIL;
        }

        #endregion

        public List<Lack1Dto> GetByPeriod(Lack1GetByPeriodParamInput input)
        {
            var getData = _lack1Service.GetByPeriod(input);

            if (getData.Count == 0) return new List<Lack1Dto>();

            var mappedData = Mapper.Map<List<Lack1Dto>>(getData);

            var selected =
                mappedData.Where(c => c.Periode <= input.PeriodTo.AddDays(1) && c.Periode >= input.PeriodFrom)
                    .OrderByDescending(o => o.Periode)
                    .ToList();

            return selected.Count == 0 ? new List<Lack1Dto>() : selected;
        }
        
        public Lack1GeneratedOutput GenerateLack1DataByParam(Lack1GenerateDataParamInput input)
        {
            return GenerateLack1Data(input);
        }

        public Lack1PrintOutDto GetPrintOutData(int id)
        {
            var dbData = _lack1Service.GetDetailsById(id);
            var dtToReturn = Mapper.Map<Lack1PrintOutDto>(dbData);

            if (dtToReturn.Lack1Pbck1Mapping.Count > 0)
            {
                var monthList = _monthBll.GetAll();
                for (int i = 0; i < dtToReturn.Lack1Pbck1Mapping.Count; i++)
                {
                    if (dtToReturn.Lack1Pbck1Mapping[i].DECREE_DATE.HasValue)
                    {
                        dtToReturn.Lack1Pbck1Mapping[i].DisplayDecreeDate = SetDisplayPbck1DecreeDate(monthList,
                            dtToReturn.Lack1Pbck1Mapping[i].DECREE_DATE.Value);
                    }
                    else
                    {
                        dtToReturn.Lack1Pbck1Mapping[i].DisplayDecreeDate = "";
                    }
                }
            }


            //set header footer data by CompanyCode and FormTypeId
            var headerFooterData = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput()
            {
                FormTypeId = Enums.FormType.LACK1,
                CompanyCode = dbData.BUKRS
            });

            dtToReturn.HeaderFooter = headerFooterData;

            if (dtToReturn.SubmissionDate.HasValue)
            {
                int monthId = dtToReturn.SubmissionDate.Value.Month;
                int year = dtToReturn.SubmissionDate.Value.Year;
                var monthData = _monthBll.GetMonth(monthId);
                if (monthData != null)
                {
                    dtToReturn.SubmissionDateDisplayString = dtToReturn.SubmissionDate.Value.Day + " " +
                                                             monthData.MONTH_NAME_IND + " " + year;
                }
            }

            var userCreator = _userBll.GetUserById(dtToReturn.CreateBy);
            if (userCreator != null)
            {
                dtToReturn.ExcisableExecutiveCreator = userCreator.FIRST_NAME + " " + userCreator.LAST_NAME;
            }

            //set NppbkcCity
            if (dtToReturn.Lack1Level == Enums.Lack1Level.Nppbkc)
            {
                //get main plant
                var mainPlant = _t001WServices.GetMainPlantByNppbkcId(dtToReturn.NppbkcId);
                if (mainPlant != null)
                {
                    dtToReturn.NppbkcCity = mainPlant.ORT01;
                }
            }
            else
            {
                //get by plant id
                var plant = _t001WServices.GetById(dtToReturn.LevelPlantId);
                if (plant != null)
                {
                    dtToReturn.NppbkcCity = plant.ORT01;
                }
            }

            return dtToReturn;
        }

        public List<Lack1DetailsDto> GetPbck1RealizationList(Lack1GetPbck1RealizationListParamInput input)
        {
            return Mapper.Map<List<Lack1DetailsDto>>(_lack1Service.GetPbck1RealizationList(input));
        }

        #region ----------------Private Method-------------------

        [Obsolete("Old logic, now only from STATUS for regenerate condition")]
        private bool IsNeedToRegenerate(Lack1GenerateDataParamInput input, LACK1 lack1Data)
        {
            if (input.CompanyCode == lack1Data.BUKRS && input.PeriodMonth == lack1Data.PERIOD_MONTH
                && input.PeriodYear == lack1Data.PERIOD_YEAR && input.NppbkcId == lack1Data.NPPBKC_ID &&
                input.ExcisableGoodsType == lack1Data.EX_GOODTYP
                && input.SupplierPlantId == lack1Data.SUPPLIER_PLANT_WERKS)
            {
                //no need to regenerate
                if (input.Lack1Level == Enums.Lack1Level.Plant)
                {
                    var plant = lack1Data.LACK1_PLANT.FirstOrDefault();
                    if (plant != null && plant.PLANT_ID == input.ReceivedPlantId)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        private bool SetChangesHistory(Lack1DetailsDto origin, Lack1DetailsDto data, string userId)
        {
            var rc = false;
            var changesData = new Dictionary<string, bool>
            {
                { "BUKRS", origin.Bukrs == data.Bukrs },
                { "BUTXT", origin.Butxt == data.Butxt },
                { "PERIOD_MONTH", origin.PeriodMonth == data.PeriodMonth },
                { "PERIOD_YEAR", origin.PeriodYears == data.PeriodYears },
                { "SUBMISSION_DATE", origin.SubmissionDate == data.SubmissionDate },
                { "SUPPLIER_PLANT", origin.SupplierPlant == data.SupplierPlant },
                { "SUPPLIER_PLANT_WERKS", origin.SupplierPlantId == data.SupplierPlantId },
                { "SUPPLIER_PLANT_ADDRESS", origin.SupplierPlantAddress == data.SupplierPlantAddress },
                { "EX_GOODTYP", origin.ExGoodsType == data.ExGoodsType },
                { "EX_TYP_DESC", origin.ExGoodsTypeDesc == data.ExGoodsTypeDesc },
                { "WASTE_QTY", origin.WasteQty == data.WasteQty },
                { "WASTE_UOM", origin.WasteUom == data.WasteUom },
                { "RETURN_QTY", origin.ReturnQty == data.ReturnQty },
                { "RETURN_UOM", origin.ReturnUom == data.ReturnUom },
                { "NPPBKC_ID", origin.NppbkcId == data.NppbkcId },
                { "BEGINING_BALANCE", origin.BeginingBalance == data.BeginingBalance },
                { "TOTAL_INCOME", origin.TotalIncome == data.TotalIncome },
                { "USAGE", origin.Usage == data.Usage },
                { "NOTED", origin.Noted == data.Noted },
                { "SUPPLIER_COMPANY_NAME", origin.SupplierCompanyName == data.SupplierCompanyName },
                { "SUPPLIER_COMPANY_CODE", origin.SupplierCompanyCode == data.SupplierCompanyCode }
            };

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Enums.MenuList.LACK1,
                        FORM_ID = data.Lack1Id.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "BUKRS":
                            changes.OLD_VALUE = origin.Bukrs;
                            changes.NEW_VALUE = data.Bukrs;
                            break;
                        case "BUTXT":
                            changes.OLD_VALUE = origin.Butxt;
                            changes.NEW_VALUE = data.Butxt;
                            break;
                        case "PERIOD_MONTH":
                            changes.OLD_VALUE = origin.PeriodMonth.ToString();
                            changes.NEW_VALUE = data.PeriodMonth.ToString();
                            break;
                        case "PERIOD_YEAR":
                            changes.OLD_VALUE = origin.PeriodYears.ToString();
                            changes.NEW_VALUE = data.PeriodYears.ToString();
                            break;
                        case "SUBMISSION_DATE":
                            changes.OLD_VALUE = origin.SubmissionDate.ToString();
                            changes.NEW_VALUE = data.SubmissionDate.ToString();
                            break;
                        case "SUPPLIER_PLANT":
                            changes.OLD_VALUE = origin.SupplierPlant;
                            changes.NEW_VALUE = data.SupplierPlant;
                            break;
                        case "SUPPLIER_PLANT_WERKS":
                            changes.OLD_VALUE = origin.SupplierPlantId;
                            changes.NEW_VALUE = data.SupplierPlantId;
                            break;
                        case "SUPPLIER_PLANT_ADDRESS":
                            changes.OLD_VALUE = origin.SupplierPlantAddress;
                            changes.NEW_VALUE = data.SupplierPlantAddress;
                            break;
                        case "EX_GOODTYP":
                            changes.OLD_VALUE = origin.ExGoodsType;
                            changes.NEW_VALUE = data.ExGoodsType;
                            break;
                        case "EX_TYP_DESC":
                            changes.OLD_VALUE = origin.ExGoodsTypeDesc;
                            changes.NEW_VALUE = data.ExGoodsTypeDesc;
                            break;
                        case "WASTE_QTY":
                            changes.OLD_VALUE = origin.WasteQty.ToString();
                            changes.NEW_VALUE = data.WasteQty.ToString();
                            break;
                        case "WASTE_UOM":
                            changes.OLD_VALUE = origin.WasteUom;
                            changes.NEW_VALUE = data.WasteUom;
                            break;
                        case "RETURN_QTY":
                            changes.OLD_VALUE = origin.ReturnQty.ToString("N2");
                            changes.NEW_VALUE = data.ReturnQty.ToString("N2");
                            break;
                        case "RETURN_UOM":
                            changes.OLD_VALUE = origin.ReturnUom;
                            changes.NEW_VALUE = data.ReturnUom;
                            break;
                        case "NPPBKC_ID":
                            changes.OLD_VALUE = origin.NppbkcId;
                            changes.NEW_VALUE = data.NppbkcId;
                            break;
                        case "BEGINING_BALANCE":
                            changes.OLD_VALUE = origin.BeginingBalance.ToString("N2");
                            changes.NEW_VALUE = data.BeginingBalance.ToString("N2");
                            break;
                        case "TOTAL_INCOME":
                            changes.OLD_VALUE = origin.TotalIncome.ToString("N2");
                            changes.NEW_VALUE = data.TotalIncome.ToString("N2");
                            break;
                        case "USAGE":
                            changes.OLD_VALUE = origin.Usage.ToString("N2");
                            changes.NEW_VALUE = data.Usage.ToString("N2");
                            break;
                        case "NOTED":
                            changes.OLD_VALUE = origin.Noted;
                            changes.NEW_VALUE = data.Noted;
                            break;
                        case "SUPPLIER_COMPANY_NAME":
                            changes.OLD_VALUE = origin.SupplierCompanyName;
                            changes.NEW_VALUE = data.SupplierCompanyName;
                            break;
                        case "SUPPLIER_COMPANY_CODE":
                            changes.OLD_VALUE = origin.SupplierCompanyCode;
                            changes.NEW_VALUE = data.SupplierCompanyCode;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                    rc = true;
                }
            }
            return rc;
        }

        private string SetDisplayPbck1DecreeDate(IEnumerable<MONTH> months, DateTime dt)
        {
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            string monthName = "<<undefine month>>";

            var monthData = months.FirstOrDefault(c => c.MONTH_ID == month);
            if (monthData != null)
            {
                monthName = monthData.MONTH_NAME_IND;
            }
            return day + " " + monthName + " " + year;
        }

        private Lack1GeneratedOutput ValidationOnGenerateLack1Data(ref Lack1GenerateDataParamInput input)
        {

            #region Validation

            //Check Excisable Group Type if exists
            var checkExcisableGroupType = _exGroupTypeService.GetGroupTypeDetailByGoodsType(input.ExcisableGoodsType);
            if (checkExcisableGroupType == null)
            {
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound),
                    Data = null
                };
            }

            if (checkExcisableGroupType.EX_GROUP_TYPE_ID != null)
                input.ExGroupTypeId = checkExcisableGroupType.EX_GROUP_TYPE_ID.Value;

            #endregion

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };
        }

        private Lack1GeneratedOutput IsExistLack1Data(Lack1GenerateDataParamInput input)
        {
            //check if already exists with same selection criteria
            var lack1Check = _lack1Service.GetBySelectionCriteria(new Lack1GetBySelectionCriteriaParamInput()
            {
                CompanyCode = input.CompanyCode,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                ReceivingPlantId = input.ReceivedPlantId,
                SupplierPlantId = input.SupplierPlantId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                Lack1Level = input.Lack1Level
            });

            if (input.IsCreateNew)
            {
                if (lack1Check != null)
                {
                    return new Lack1GeneratedOutput()
                    {
                        Success = false,
                        ErrorCode = ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria.ToString(),
                        ErrorMessage =
                            EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria),
                        Data = null
                    };
                }
            }
            else
            {
                if (lack1Check != null && lack1Check.LACK1_ID != input.Lack1Id)
                {
                    return new Lack1GeneratedOutput()
                    {
                        Success = false,
                        ErrorCode = ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria.ToString(),
                        ErrorMessage =
                            EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria),
                        Data = null
                    };
                }
            }

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };
        }

        private Lack1GeneratedOutput GenerateLack1Data(Lack1GenerateDataParamInput input)
        {

            var outValidation = ValidationOnGenerateLack1Data(ref input);

            if (!outValidation.Success) return outValidation;

            var oReturn = new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            var rc = new Lack1GeneratedDto
            {
                CompanyCode = input.CompanyCode,
                CompanyName = input.CompanyName,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                ExcisableGoodsTypeDesc = input.ExcisableGoodsTypeDesc,
                SupplierPlantId = input.SupplierPlantId,
                BeginingBalance = 0 //set default
            };

            //Set Income List by selection Criteria
            //from CK5 data
            rc = SetIncomeListBySelectionCriteria(rc, input);

            if (rc.IncomeList.Count == 0)
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.MissingIncomeListItem.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingIncomeListItem),
                    Data = null
                };

            var stoReceiverNumberList = rc.IncomeList.Select(d => d.Ck5Type == Enums.CK5Type.Intercompany ? d.StoReceiverNumber : d.StoSenderNumber).ToList();

            //Get Data from Inventory_Movement
            var mvtTypeForUsage = new List<string>
            {
                EnumHelper.GetDescription(Enums.MovementTypeCode.Usage261),
                EnumHelper.GetDescription(Enums.MovementTypeCode.Usage261),
                EnumHelper.GetDescription(Enums.MovementTypeCode.Usage201),
                EnumHelper.GetDescription(Enums.MovementTypeCode.Usage202),
                EnumHelper.GetDescription(Enums.MovementTypeCode.Usage901),
                EnumHelper.GetDescription(Enums.MovementTypeCode.Usage902),
                EnumHelper.GetDescription(Enums.MovementTypeCode.UsageZ01),
                EnumHelper.GetDescription(Enums.MovementTypeCode.UsageZ02)
            };

            var plantIdList = new List<string>();
            if (input.Lack1Level == Enums.Lack1Level.Nppbkc)
            {
                //get plant list by nppbkcid
                var plantList = _t001WServices.GetByNppbkcId(input.NppbkcId);
                if (plantList.Count > 0)
                {
                    plantIdList = plantList.Select(c => c.WERKS).ToList();
                }
            }
            else
            {
                plantIdList = new List<string>() { input.ReceivedPlantId };
            }

            var invMovementInput = new InvMovementGetForLack1UsageMovementByParamInput()
            {
                NppbkcId = input.NppbkcId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                MvtCodeList = mvtTypeForUsage,
                PlantIdList = plantIdList,
                StoReceiverNumberList = stoReceiverNumberList
            };

            var invMovementOutput = _inventoryMovementService.GetForLack1UsageMovementByParam(invMovementInput);

            if (invMovementOutput.IncludeInCk5List.Count <= 0)
            {
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.TotalUsageLessThanEqualTpZero.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.TotalUsageLessThanEqualTpZero),
                    Data = null
                };
            }

            var totalUsageIncludeCk5 = (-1) * invMovementOutput.IncludeInCk5List.Sum(d => d.QTY.HasValue ? (!string.IsNullOrEmpty(d.BUN) && d.BUN.ToLower() == "kg" ? d.QTY.Value * 1000 : d.QTY.Value) : 0);
            var totalUsageExcludeCk5 = (-1) * invMovementOutput.ExcludeFromCk5List.Sum(d => d.QTY.HasValue ? (!string.IsNullOrEmpty(d.BUN) && d.BUN.ToLower() == "kg" ? d.QTY.Value * 1000 : d.QTY.Value) : 0);

            rc.TotalUsage = totalUsageIncludeCk5;

            rc.InvMovementReceivingCk5List = Mapper.Map<List<Lack1GeneratedTrackingDto>>(invMovementOutput.IncludeInCk5List);
            rc.InvMovementReceivingList = Mapper.Map<List<Lack1GeneratedTrackingDto>>(invMovementOutput.ReceivingList);
            rc.InvMovementAllList =
                Mapper.Map<List<Lack1GeneratedTrackingDto>>(invMovementOutput.AllUsageList);

            //set begining balance
            rc = SetBeginingBalanceBySelectionCritera(rc, input);

            //set Pbck-1 Data by selection criteria
            rc = SetPbck1DataBySelectionCriteria(rc, input);

            var productionList = GetProductionDetailBySelectionCriteria(input);

            if (productionList.Count == 0)
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.MissingProductionList.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingProductionList),
                    Data = null
                };

            rc.ProductionList = GetGroupedProductionlist(productionList, (totalUsageIncludeCk5 + totalUsageExcludeCk5), totalUsageIncludeCk5);

            //set summary
            rc.SummaryProductionList = GetSummaryGroupedProductionList(rc.ProductionList);

            rc.PeriodMonthId = input.PeriodMonth;

            var monthData = _monthBll.GetMonth(rc.PeriodMonthId);
            if (monthData != null)
            {
                rc.PeriodMonthName = monthData.MONTH_NAME_IND;
            }

            rc.PeriodYear = input.PeriodYear;
            rc.Noted = input.Noted;

            rc.EndingBalance = rc.BeginingBalance + rc.TotalIncome - rc.TotalUsage;

            oReturn.Data = rc;

            return oReturn;
        }

        /// <summary>
        /// Set Production Detail from CK4C Item table 
        /// for Generate LACK-1 data by Selection Criteria
        /// </summary>
        private List<Lack1GeneratedProductionDataDto> GetProductionDetailBySelectionCriteria(
            Lack1GenerateDataParamInput input)
        {

            var ck4CItemInput = Mapper.Map<CK4CItemGetByParamInput>(input);
            ck4CItemInput.IsHigherFromApproved = false;
            ck4CItemInput.IsCompletedOnly = true;
            var ck4CItemData = _ck4cItemService.GetByParam(ck4CItemInput);

            var prodTypeData = _prodTypeService.GetAll();

            //joined data
            var dataCk4CItemJoined = (from ck4CItem in ck4CItemData
                                      join prod in prodTypeData on ck4CItem.PROD_CODE equals prod.PROD_CODE
                                      select new Lack1GeneratedProductionDataDto()
                                      {
                                          ProdCode = prod.PROD_CODE,
                                          ProductType = prod.PRODUCT_TYPE,
                                          ProductAlias = prod.PRODUCT_ALIAS,
                                          Amount = ck4CItem.PROD_QTY,
                                          UomId = ck4CItem.UOM_PROD_QTY,
                                          UomDesc = ck4CItem.UOM != null ? ck4CItem.UOM.UOM_DESC : string.Empty
                                      });

            return dataCk4CItemJoined.ToList();
        }

        /// <summary>
        /// Get CK5 by selection criteria
        /// Set Income list on Generating LACK-1 by Selection Criteria
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private Lack1GeneratedDto SetIncomeListBySelectionCriteria(Lack1GeneratedDto rc, Lack1GenerateDataParamInput input)
        {
            var ck5Input = Mapper.Map<Ck5GetForLack1ByParamInput>(input);
            ck5Input.IsExcludeSameNppbkcId = true;
            var ck5Data = _ck5Service.GetForLack1ByParam(ck5Input);
            rc.IncomeList = Mapper.Map<List<Lack1GeneratedIncomeDataDto>>(ck5Data);

            if (ck5Data.Count > 0)
            {
                rc.TotalIncome = rc.IncomeList.Sum(d => d.Amount);
            }

            return rc;
        }

        /// <summary>
        /// Get Latest Saldo on Latest LACK-1 
        /// Use for generate LACK-1 data by selection criteria
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private Lack1GeneratedDto SetBeginingBalanceBySelectionCritera(Lack1GeneratedDto rc,
            Lack1GenerateDataParamInput input)
        {
            //validate period input
            if (input.PeriodMonth < 1 || input.PeriodMonth > 12)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.InvalidData);
            }

            //valid input
            var dtTo = new DateTime(input.PeriodYear, input.PeriodMonth, 1);
            var selected = _lack1Service.GetLatestLack1ByParam(new Lack1GetLatestLack1ByParamInput()
            {
                CompanyCode = input.CompanyCode,
                Lack1Level = input.Lack1Level,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                SupplierPlantId = input.SupplierPlantId,
                ReceivedPlantId = input.ReceivedPlantId,
                PeriodTo = dtTo
            });

            rc.BeginingBalance = 0;
            if (selected != null)
            {
                rc.BeginingBalance = selected.BEGINING_BALANCE + selected.TOTAL_INCOME - selected.USAGE;
            }

            return rc;
        }

        /// <summary>
        /// Set Pbck1 Data on Generating LACK-1 Data by Selection Criteria
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private Lack1GeneratedDto SetPbck1DataBySelectionCriteria(Lack1GeneratedDto rc,
            Lack1GenerateDataParamInput input)
        {
            var pbck1Input = Mapper.Map<Pbck1GetDataForLack1ParamInput>(input);
            var pbck1Data = _pbck1Service.GetForLack1ByParam(pbck1Input);

            if (pbck1Data.Count > 0)
            {
                var latestDecreeDate = pbck1Data.OrderByDescending(c => c.DECREE_DATE).FirstOrDefault();

                if (latestDecreeDate != null)
                {
                    var companyData = _t001KService.GetByBwkey(latestDecreeDate.SUPPLIER_PLANT_WERKS);
                    if (companyData != null)
                    {
                        rc.SupplierCompanyCode = companyData.BUKRS;
                        rc.SupplierCompanyName = companyData.T001.BUTXT;
                    }
                    rc.SupplierPlantAddress = latestDecreeDate.SUPPLIER_ADDRESS;
                    rc.SupplierPlantName = latestDecreeDate.SUPPLIER_PLANT;
                    rc.Lack1UomId = latestDecreeDate.REQUEST_QTY_UOM;
                }
                rc.Pbck1List = Mapper.Map<List<Lack1GeneratedPbck1DataDto>>(pbck1Data);

            }
            else
            {
                rc.Pbck1List = new List<Lack1GeneratedPbck1DataDto>();
            }
            return rc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="totalUsage"></param>
        /// <param name="totalUsageInCk5"></param>
        /// <returns></returns>
        private List<Lack1GeneratedProductionDataDto> GetGroupedProductionlist(List<Lack1GeneratedProductionDataDto> list, decimal totalUsage, decimal totalUsageInCk5)
        {
            if (list.Count <= 0) return new List<Lack1GeneratedProductionDataDto>();

            var groupedData = list.GroupBy(p => new
            {
                p.ProdCode,
                p.ProductType,
                p.ProductAlias,
                p.UomId,
                p.UomDesc
            }).Select(g => new Lack1GeneratedProductionDataDto()
            {
                ProdCode = g.Key.ProdCode,
                ProductType = g.Key.ProductType,
                ProductAlias = g.Key.ProductAlias,
                UomId = g.Key.UomId,
                UomDesc = g.Key.UomDesc,
                Amount = g.Sum(p => p.Amount)
            });

            //proporsional process
            var lack1GeneratedProductionDataDtos = groupedData as Lack1GeneratedProductionDataDto[] ?? groupedData.ToArray();
            var dToReturn = lack1GeneratedProductionDataDtos.Select(g => new Lack1GeneratedProductionDataDto()
            {
                ProdCode = g.ProdCode,
                ProductType = g.ProductType,
                ProductAlias = g.ProductAlias,
                UomId = g.UomId,
                UomDesc = g.UomDesc,
                //Amount = (g.Amount / totalAmount) * ((totalUsageInCk5 / totalUsage) * totalUsage)
                //Amount = (g.Amount) //just for testing
                Amount = Math.Round(((totalUsageInCk5 / totalUsage) * g.Amount), 2)
            });

            return dToReturn.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Lack1GeneratedSummaryProductionDataDto> GetSummaryGroupedProductionList(List<Lack1GeneratedProductionDataDto> list)
        {
            if (list.Count <= 0) return new List<Lack1GeneratedSummaryProductionDataDto>();
            var groupedData = list.GroupBy(p => new
            {
                p.UomId,
                p.UomDesc
            }).Select(g => new Lack1GeneratedSummaryProductionDataDto()
            {
                UomId = g.Key.UomId,
                UomDesc = g.Key.UomDesc,
                Amount = g.Sum(p => p.Amount)
            });

            return groupedData.ToList();
        }

        #endregion

        #region ------------------ Summary Report ----------------

        public List<Lack1SummaryReportDto> GetSummaryReportByParam(Lack1GetSummaryReportByParamInput input)
        {
            var lack1Data = _lack1Service.GetSummaryReportByParam(input);
            return Mapper.Map<List<Lack1SummaryReportDto>>(lack1Data);
        }

        public List<int> GetYearList()
        {
            return _lack1Service.GetYearList();
        }

        public List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbckListByCompanyCode(string companyCode)
        {
            var data = _lack1Service.GetByCompanyCode(companyCode);
            if (data.Count > 0)
            {
                var nppbkcList = Mapper.Map<List<ZAIDM_EX_NPPBKCCompositeDto>>(data);
                return nppbkcList.DistinctBy(c => c.NPPBKC_ID).ToList();
            }
            return new List<ZAIDM_EX_NPPBKCCompositeDto>();
        }

        #endregion

        #region --------------------- Detail Report ----------------

        public List<Lack1DetailReportDto> GetDetailReportByParam(Lack1GetDetailReportByParamInput input)
        {
            var dbData = _lack1Service.GetDetailReportByParamInput(input);

            var rc = new List<Lack1DetailReportDto>();

            foreach (var data in dbData)
            {
                var item = new Lack1DetailReportDto()
                {
                    Lack1Id = data.LACK1_ID,
                    Lack1Number = data.LACK1_NUMBER,
                    BeginingBalance = data.BEGINING_BALANCE,
                    EndingBalance = data.BEGINING_BALANCE + data.TOTAL_INCOME - data.USAGE,
                    TrackingConsolidations = new List<Lack1TrackingConsolidationDetailReportDto>()
                };

                var ck5MaterialList = new List<Lack1Ck5MaterialDetailReportDto>();
                foreach (var toInsert in data.LACK1_INCOME_DETAIL.Select(ck5 =>
                    (from x in ck5.CK5.CK5_MATERIAL
                     let ck5Id = x.CK5_ID
                     where ck5Id != null
                     select new Lack1Ck5MaterialDetailReportDto()
                     {
                         Ck5Id = ck5Id.Value,
                         Ck5Number = x.CK5.SUBMISSION_NUMBER,
                         Ck5RegistrationNumber = x.CK5.REGISTRATION_NUMBER,
                         Ck5RegistrationDate = x.CK5.REGISTRATION_DATE,
                         Ck5GrDate = x.CK5.GR_DATE,
                         StoNumber =
                             x.CK5.CK5_TYPE == Enums.CK5Type.Intercompany
                                 ? x.CK5.STO_RECEIVER_NUMBER
                                 : x.CK5.STO_SENDER_NUMBER,
                         GiDate = x.CK5.GI_DATE,
                         Qty = x.QTY.HasValue ? x.QTY.Value : 0,
                         UomId = x.UOM,
                         ConvertedUomId = x.CONVERTED_UOM,
                         MaterialId = x.BRAND,
                         ConvertedQty = x.CONVERTED_QTY.HasValue ? x.CONVERTED_QTY.Value : 0
                     })))
                {
                    ck5MaterialList.AddRange(toInsert.ToList());
                }

                if (data.LACK1_TRACKING != null && data.LACK1_TRACKING.Count > 0)
                {
                    var receivingMvtType = new List<string>()
                        {
                            EnumHelper.GetDescription(Enums.MovementTypeCode.Receiving101),
                            EnumHelper.GetDescription(Enums.MovementTypeCode.Receiving102)
                        };

                    var receiving =
                        data.LACK1_TRACKING.Where(
                                c =>receivingMvtType.Contains(c.INVENTORY_MOVEMENT.MVT))
                            .ToList();

                    var mvtTypeForUsage = new List<string>
                    {
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage261),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage261),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage201),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage202),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage901),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage902),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.UsageZ01),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.UsageZ02)
                    };

                    var usage =
                        data.LACK1_TRACKING.Where(c => mvtTypeForUsage.Contains(c.INVENTORY_MOVEMENT.MVT)).ToList();

                    var usageReceiving = (from rec in receiving
                                          join a in usage on new { rec.INVENTORY_MOVEMENT.BATCH, rec.INVENTORY_MOVEMENT.MATERIAL_ID } equals
                                              new { a.INVENTORY_MOVEMENT.BATCH, a.INVENTORY_MOVEMENT.MATERIAL_ID }
                                          select new Lack1TrackingConsolidationDetailReportDto()
                                          {
                                              PurchaseDoc = rec.INVENTORY_MOVEMENT.PURCH_DOC,
                                              MaterialCode = a.INVENTORY_MOVEMENT.MATERIAL_ID,
                                              UsageQty = a.INVENTORY_MOVEMENT.QTY.HasValue ? a.INVENTORY_MOVEMENT.QTY.Value : 0
                                          }).ToList();


                    var usageConsolidationData = new List<Lack1TrackingConsolidationDetailReportDto>();
                    foreach (var d in ck5MaterialList)
                    {
                        var rec =
                            usageReceiving.FirstOrDefault(
                                c => c.PurchaseDoc == d.StoNumber && c.MaterialCode == d.MaterialId);
                        if (rec == null)
                        {
                            usageConsolidationData.Add(new Lack1TrackingConsolidationDetailReportDto()
                            {
                                Ck5Id = d.Ck5Id,
                                Ck5Number = d.Ck5Number,
                                Ck5RegistrationNumber = d.Ck5RegistrationNumber,
                                Ck5RegistrationDate = d.Ck5RegistrationDate,
                                Ck5GrDate = d.Ck5GrDate,
                                Qty = d.Qty,
                                GiDate = d.GiDate,
                                PurchaseDoc = "",
                                UsageQty = null,
                                OriginalUomId = d.UomId,
                                ConvertedUomId = d.ConvertedUomId,
                                MaterialCode = d.MaterialId
                            });
                        }
                        else
                        {
                            usageConsolidationData.Add(new Lack1TrackingConsolidationDetailReportDto()
                            {
                                Ck5Id = d.Ck5Id,
                                Ck5Number = d.Ck5Number,
                                Ck5RegistrationNumber = d.Ck5RegistrationNumber,
                                Ck5RegistrationDate = d.Ck5RegistrationDate,
                                Ck5GrDate = d.Ck5GrDate,
                                Qty = d.Qty,
                                GiDate = d.GiDate,
                                PurchaseDoc = rec.PurchaseDoc,
                                UsageQty = rec.UsageQty,
                                OriginalUomId = d.UomId,
                                ConvertedUomId = d.ConvertedUomId,
                                MaterialCode = d.MaterialId
                            });
                        }
                    }
                    item.TrackingConsolidations.AddRange(usageConsolidationData);
                }
                else
                {
                    var usageConsolidationData = ck5MaterialList.Select(d => new Lack1TrackingConsolidationDetailReportDto()
                    {
                        Ck5Id = d.Ck5Id,
                        Ck5Number = d.Ck5Number,
                        Ck5RegistrationNumber = d.Ck5RegistrationNumber,
                        Ck5RegistrationDate = d.Ck5RegistrationDate,
                        Ck5GrDate = d.Ck5GrDate,
                        Qty = d.Qty,
                        GiDate = d.GiDate,
                        PurchaseDoc = "",
                        UsageQty = null,
                        OriginalUomId = d.UomId,
                        ConvertedUomId = d.ConvertedUomId,
                        MaterialCode = d.MaterialId
                    }).ToList();
                    item.TrackingConsolidations.AddRange(usageConsolidationData);
                }
                rc.Add(item);
            }
            return rc;
        }

        #endregion

    }
}
