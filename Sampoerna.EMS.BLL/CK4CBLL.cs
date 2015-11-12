using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.MessagingService;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class CK4CBLL : ICK4CBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<CK4C> _repository;
        private IMonthBLL _monthBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IPOABLL _poabll;
        private IWorkflowBLL _workflowBll;
        private ICK4CItemBLL _ck4cItemBll;
        private IPlantBLL _plantBll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IHeaderFooterBLL _headerFooterBll;
        private IBrandRegistrationBLL _brandBll;
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IMessageService _messageService;
        private IUserBLL _userBll;
        private IBrandRegistrationService _brandRegistrationService;
        private ICK4CDecreeDocBLL _ck4cDecreeDocBll;
        private IWasteBLL _wasteBll;
        private IProductionBLL _productionBll;
        private IPOAMapBLL _poaMapBll;
        private IUserPlantMapBLL _userPlantBll;
        private IDocumentSequenceNumberBLL _documentSequenceNumberBll;

        private string includeTables = "MONTH, CK4C_ITEM, CK4C_DECREE_DOC";

        public CK4CBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK4C>();
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _poabll = new POABLL(_uow, _logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _ck4cItemBll = new CK4CItemBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _plantBll = new PlantBLL(_uow, _logger);
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _headerFooterBll = new HeaderFooterBLL(_uow, _logger);
            _brandBll = new BrandRegistrationBLL(_uow, _logger);
            _prodTypeBll = new ZaidmExProdTypeBLL(_uow, _logger);
            _messageService = new MessageService(_logger);
            _userBll = new UserBLL(_uow, _logger);
            _brandRegistrationService = new BrandRegistrationService(_uow, _logger);
            _ck4cDecreeDocBll = new CK4CDecreeDocBLL(_uow, _logger);
            _wasteBll = new WasteBLL(_logger, _uow);
            _productionBll = new ProductionBLL(_logger, _uow);
            _poaMapBll = new POAMapBLL(_uow, _logger);
            _userPlantBll = new UserPlantMapBLL(_uow, _logger);
            _documentSequenceNumberBll = new DocumentSequenceNumberBLL(_uow, _logger);
        }

        public List<Ck4CDto> GetAllByParam(Ck4CDashboardParamInput input)
        {
            Expression<Func<CK4C, bool>> queryFilter = PredicateHelper.True<CK4C>();

            if (input.Month > 0)
            {
                queryFilter = queryFilter.And(c => c.REPORTED_MONTH == input.Month);
            }
            if (input.Year > 0)
            {
                queryFilter = queryFilter.And(c => c.REPORTED_YEAR == input.Year);
            }
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty(input.Poa))
            {
                var nppbkc = _poaMapBll.GetNppbkcByPoaId(input.Poa);

                queryFilter = queryFilter.And(c => nppbkc.Contains(c.NPPBKC_ID));
            }

            if (input.UserRole == Enums.UserRole.POA)
            {
                var listNppbkc = _userPlantBll.GetNppbkcByUserId(input.UserId);

                var nppbkc = _poaMapBll.GetNppbkcByPoaId(input.UserId);

                queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId || (c.STATUS != Enums.DocumentStatus.Draft && (nppbkc.Contains(c.NPPBKC_ID) || listNppbkc.Contains(c.NPPBKC_ID))) || c.STATUS == Enums.DocumentStatus.Completed));
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poabll.GetPOAIdByManagerId(input.UserId);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

                queryFilter = queryFilter.And(c => (c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.NUMBER)) || c.STATUS == Enums.DocumentStatus.Completed);
            }
            else
            {
                var listNppbkc = _userPlantBll.GetNppbkcByUserId(input.UserId);

                queryFilter = queryFilter.And(c => listNppbkc.Contains(c.NPPBKC_ID) || c.STATUS == Enums.DocumentStatus.Completed);
            }

            return Mapper.Map<List<Ck4CDto>>(GetCk4cData(queryFilter, null));
        }

        public List<Ck4CDto> GetOpenDocument()
        {
            var dtData = _repository.Get(x => x.STATUS != Enums.DocumentStatus.Completed, null, includeTables).ToList();

            return Mapper.Map<List<Ck4CDto>>(dtData);
        }

        public Ck4CDto Save(Ck4CDto item, string userId)
        {
            CK4C model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }
            
            try
            {
                bool changed = false;

                if (item.Ck4CId > 0)
                {
                    //update
                    model = _repository.Get(c => c.CK4C_ID == item.Ck4CId, null, includeTables).FirstOrDefault();

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    changed = SetChangesHistory(model, item, userId);

                    _ck4cItemBll.DeleteByCk4cId(item.Ck4CId);

                    Mapper.Map<Ck4CDto, CK4C>(item, model);
                    model.CK4C_ITEM = null;

                    model.CK4C_ITEM = Mapper.Map<List<CK4C_ITEM>>(item.Ck4cItem);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = item.ReportedMonth;
                    inputDoc.Year = item.ReportedYears;
                    inputDoc.NppbkcId = item.NppbkcId;

                    item.Number = _documentSequenceNumberBll.GenerateNumber(inputDoc);

                    model = Mapper.Map<CK4C>(item);
                    _repository.InsertOrUpdate(model);
                }
                
                _uow.SaveChanges();

                //set workflow history
                var getUserRole = _poabll.GetUserRole(userId);
                var input = new Ck4cWorkflowDocumentInput()
                {
                    DocumentId = model.CK4C_ID,
                    DocumentNumber = model.NUMBER,
                    ActionType = Enums.ActionType.Modified,
                    UserId = userId,
                    UserRole = getUserRole
                };

                if (changed)
                {
                    AddWorkflowHistory(input);
                }
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Mapper.Map<Ck4CDto>(model);
        }

        public Ck4CDto GetById(long id)
        {
            var dbData = _repository.Get(c => c.CK4C_ID == id, null, includeTables).FirstOrDefault();

            var mapResult = Mapper.Map<Ck4CDto>(dbData);

            return mapResult;
        }
        
        public void Ck4cWorkflow(Ck4cWorkflowDocumentInput input)
        {
            var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Created:
                    CreateDocument(input);
                    isNeedSendNotif = false;
                    break;
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
                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocument(input);
                    break;
                case Enums.ActionType.Completed:
                    EditCompletedDocument(input);
                    isNeedSendNotif = false;
                    break;
            }

            //todo sent mail
            if (isNeedSendNotif) SendEmailWorkflow(input);
                
            _uow.SaveChanges();
        }

        private void SendEmailWorkflow(Ck4cWorkflowDocumentInput input)
        {
            var ck4cData = Mapper.Map<Ck4CDto>(_repository.Get(c => c.CK4C_ID == input.DocumentId, null, includeTables).FirstOrDefault());

            var mailProcess = ProsesMailNotificationBody(ck4cData, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private Ck4cMailNotification ProsesMailNotificationBody(Ck4CDto ck4cData, Ck4cWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new Ck4cMailNotification();
            var plant = _plantBll.GetT001WById(ck4cData.PlantId);
            var nppbkc = ck4cData.NppbkcId;
            var firstText = input.ActionType == Enums.ActionType.Reject ? " Document" : string.Empty;
            var approveRejectedPoa = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(ck4cData.Number);

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            rc.Subject = "CK-4C " + ck4cData.Number + " is " + EnumHelper.GetDescription(ck4cData.Status);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, CK-4C" + firstText + " is " + EnumHelper.GetDescription(ck4cData.Status) + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + ck4cData.CompanyId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + nppbkc + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + ck4cData.Number + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : CK-4C</td></tr>");
            bodyMail.AppendLine();
            if (input.ActionType == Enums.ActionType.Reject || input.ActionType == Enums.ActionType.GovReject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/CK4C/Details/" + ck4cData.Ck4CId + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (ck4cData.Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (approveRejectedPoa != "")
                        {
                            var poaApproveId = _userBll.GetUserById(approveRejectedPoa);

                            rc.To.Add(poaApproveId.EMAIL);
                        }
                        else
                        {
                            var poaList = _poabll.GetPoaByNppbkcId(nppbkc);
                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }
                        
                        rc.CC.Add(_userBll.GetUserById(ck4cData.CreatedBy).EMAIL);
                    }
                    else if (ck4cData.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var userData = _userBll.GetUserById(ck4cData.CreatedBy);
                        rc.To.Add(GetManagerEmail(ck4cData.CreatedBy));
                        rc.CC.Add(userData.EMAIL);

                        var poaList = _poabll.GetPoaByNppbkcIdAndMainPlant(nppbkc);
                        foreach (var poaDto in poaList)
                        {
                            if (userData.USER_ID != poaDto.POA_ID)
                                rc.CC.Add(poaDto.POA_EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    if (ck4cData.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var poaUser = ck4cData.ApprovedByPoa == null ? ck4cData.CreatedBy : ck4cData.ApprovedByPoa;
                        var poaApproveId = _userBll.GetUserById(ck4cData.ApprovedByPoa);

                        rc.To.Add(_userBll.GetUserById(ck4cData.CreatedBy).EMAIL);

                        if (poaApproveId != null)
                            rc.CC.Add(poaApproveId.EMAIL);

                        rc.CC.Add(GetManagerEmail(poaUser));
                    }
                    else if (ck4cData.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poabll.GetById(ck4cData.CreatedBy);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(ck4cData.CreatedBy));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(ck4cData.CreatedBy);
                            var poaApproved = _userBll.GetUserById(ck4cData.ApprovedByPoa);

                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(GetManagerEmail(ck4cData.ApprovedByPoa));
                            rc.CC.Add(poaApproved.EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(ck4cData.CreatedBy);
                    var poaApprove = _userBll.GetUserById(approveRejectedPoa);
                    var poaId = approveRejectedPoa == "" ? ck4cData.CreatedBy : approveRejectedPoa;
                    var managerMail = GetManagerEmail(poaId) == "" ? GetManagerEmail(input.UserId) : GetManagerEmail(poaId);

                    rc.To.Add(userDetail.EMAIL);
                    if (poaApprove != null)
                        rc.CC.Add(poaApprove.EMAIL);
                    rc.CC.Add(managerMail);

                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovApprove:
                    var poaData3 = _poabll.GetById(ck4cData.CreatedBy);
                    if (poaData3 != null)
                    {
                        //creator is poa user
                        rc.To.Add(_userBll.GetUserById(poaData3.POA_ID).EMAIL);
                        rc.CC.Add(GetManagerEmail(ck4cData.CreatedBy));
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(ck4cData.CreatedBy);
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_userBll.GetUserById(ck4cData.ApprovedByPoa).EMAIL);
                        rc.CC.Add(GetManagerEmail(ck4cData.ApprovedByPoa));
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovReject:
                    var poaData5 = _poabll.GetById(ck4cData.CreatedBy);
                    if (poaData5 != null)
                    {
                        //creator is poa user
                        rc.To.Add(_userBll.GetUserById(poaData5.POA_ID).EMAIL);
                        rc.CC.Add(GetManagerEmail(ck4cData.CreatedBy));
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(ck4cData.CreatedBy);
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_userBll.GetUserById(ck4cData.ApprovedByPoa).EMAIL);
                        rc.CC.Add(GetManagerEmail(ck4cData.ApprovedByPoa));
                    }
                    rc.IsCCExist = true;
                    break;
            }
            rc.Body = bodyMail.ToString();
            return rc;
        }

        private string GetManagerEmail(string poaId)
        {
            var managerMail = string.Empty;

            var managerId = _poabll.GetManagerIdByPoaId(poaId);
            var managerDetail = _userBll.GetUserById(managerId);

            managerMail = managerDetail == null ? string.Empty : managerDetail.EMAIL;

            return managerMail;
        }

        private class Ck4cMailNotification
        {
            public Ck4cMailNotification()
            {
                To = new List<string>();
                CC = new List<string>();
                IsCCExist = false;
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public bool IsCCExist { get; set; }
        }

        private void CreateDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);
        }

        private void SubmitDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.Draft && dbData.STATUS != Enums.DocumentStatus.Rejected)
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

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void WorkflowStatusAddChanges(Ck4cWorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            try
            {
                //set changes log
                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Enums.MenuList.CK4C,
                    FORM_ID = input.DocumentId.ToString(),
                    FIELD_NAME = "STATUS",
                    NEW_VALUE = EnumHelper.GetDescription(newStatus),
                    OLD_VALUE = EnumHelper.GetDescription(oldStatus),
                    MODIFIED_BY = input.UserId,
                    MODIFIED_DATE = DateTime.Now
                };
                _changesHistoryBll.AddHistory(changes);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void WorkflowPoaChanges(Ck4cWorkflowDocumentInput input, string oldPoa, string newPoa)
        {
            try
            {
                //set changes log
                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Enums.MenuList.CK4C,
                    FORM_ID = input.DocumentId.ToString(),
                    FIELD_NAME = "POA Approved",
                    NEW_VALUE = newPoa,
                    OLD_VALUE = oldPoa,
                    MODIFIED_BY = input.UserId,
                    MODIFIED_DATE = DateTime.Now
                };
                _changesHistoryBll.AddHistory(changes);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void AddWorkflowHistory(Ck4cWorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.FORM_TYPE_ID = Enums.FormType.CK4C;

            _workflowHistoryBll.Save(dbData);

        }

        private void ApproveDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var plant = _plantBll.GetT001WById(dbData.PLANT_ID);
            var nppbkcId = dbData.NPPBKC_ID;

            var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
            {
                CreatedUser = dbData.CREATED_BY,
                CurrentUser = input.UserId,
                DocumentStatus = dbData.STATUS,
                UserRole = input.UserRole,
                NppbkcId = nppbkcId,
                DocumentNumber = dbData.NUMBER
            });

            if (!isOperationAllow)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //todo: gk boleh loncat approval nya, creator->poa->manager atau poa(creator)->manager
            //dbData.APPROVED_BY_POA = input.UserId;
            //dbData.APPROVED_DATE_POA = DateTime.Now;
            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);

            if (input.UserRole == Enums.UserRole.POA)
            {
                if(dbData.STATUS == Enums.DocumentStatus.WaitingForApproval)
                {
                    WorkflowPoaChanges(input, dbData.APPROVED_BY_POA, input.UserId);

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
                dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                dbData.APPROVED_BY_MANAGER = input.UserId;
                dbData.APPROVED_DATE_MANAGER = DateTime.Now;
            }

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void RejectDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);

            dbData.STATUS = Enums.DocumentStatus.Rejected;

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //delete data doc first
            _ck4cDecreeDocBll.DeleteByCk4cId(dbData.CK4C_ID);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
            WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.StatusGovCk4c.Approved);

            dbData.STATUS = Enums.DocumentStatus.Completed;

            //todo: update remaining quota and necessary data
            dbData.CK4C_DECREE_DOC = null;
            dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
            dbData.CK4C_DECREE_DOC = Mapper.Map<List<CK4C_DECREE_DOC>>(input.AdditionalDocumentData.Ck4cDecreeDoc);
            dbData.GOV_STATUS = Enums.StatusGovCk4c.Approved;

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void GovRejectedDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //delete data doc first
            _ck4cDecreeDocBll.DeleteByCk4cId(dbData.CK4C_ID);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.GovRejected);
            WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.StatusGovCk4c.Rejected);

            dbData.STATUS = Enums.DocumentStatus.GovRejected;
            dbData.GOV_STATUS = Enums.StatusGovCk4c.Rejected;

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void EditCompletedDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.Completed)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);

            dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;

            //todo: update remaining quota and necessary data
            dbData.CK4C_DECREE_DOC = null;
            dbData.DECREE_DATE = null;
            dbData.GOV_STATUS = null;

            _ck4cDecreeDocBll.DeleteByCk4cId(dbData.CK4C_ID);

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.NUMBER;
            input.ActionType = Enums.ActionType.Modified;

            AddWorkflowHistory(input);

        }

        private void WorkflowStatusGovAddChanges(Ck4cWorkflowDocumentInput input, Enums.StatusGovCk4c? oldStatus, Enums.StatusGovCk4c newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.CK4C,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "STATUS_GOV",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        public void UpdateReportedOn(Ck4cUpdateReportedOn input)
        {
            CK4C dbData = _repository.Get(c => c.CK4C_ID == input.Id, null, includeTables).FirstOrDefault();
            dbData.REPORTED_ON = input.ReportedOn;
            _uow.SaveChanges();
        }

        public List<Ck4CDto> GetCompletedDocumentByParam(Ck4cGetCompletedDocumentByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);

            return Mapper.Map<List<Ck4CDto>>(GetCk4cData(queryFilter, input.ShortOrderColumn));
        }

        public List<Ck4CDto> GetOpenDocumentByParam(Ck4cGetOpenDocumentByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            if (input.UserRole == Enums.UserRole.POA)
            {
                var listNppbkc = _userPlantBll.GetNppbkcByUserId(input.UserId);

                var nppbkc = _poaMapBll.GetNppbkcByPoaId(input.UserId);

                queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId || (c.STATUS != Enums.DocumentStatus.Draft && (nppbkc.Contains(c.NPPBKC_ID) || listNppbkc.Contains(c.NPPBKC_ID)))));
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poabll.GetPOAIdByManagerId(input.UserId);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.NUMBER));
            }
            else
            {
                var listNppbkc = _userPlantBll.GetNppbkcByUserId(input.UserId);

                queryFilter = queryFilter.And(c => listNppbkc.Contains(c.NPPBKC_ID));
            }

            queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);

            return Mapper.Map<List<Ck4CDto>>(GetCk4cData(queryFilter, input.ShortOrderColumn));
        }

        private Expression<Func<CK4C, bool>> ProcessQueryFilter(Ck4CGetByParamInput input)
        {
            Expression<Func<CK4C, bool>> queryFilter = PredicateHelper.True<CK4C>();

            if (!string.IsNullOrEmpty(input.DateProduction))
            {
                var dt = Convert.ToDateTime(input.DateProduction);
                queryFilter = queryFilter.And(c => c.REPORTED_ON == dt);
            }

            if (!string.IsNullOrEmpty(input.Company))
            {
                queryFilter = queryFilter.And(c => c.COMPANY_ID == input.Company);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty(input.DocumentNumber))
            {
                queryFilter = queryFilter.And(c => c.NUMBER == input.DocumentNumber);
            }
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);
            }

            return queryFilter;
        }

        private List<CK4C> GetCk4cData(Expression<Func<CK4C, bool>> queryFilter, string orderColumn)
        {
            Func<IQueryable<CK4C>, IOrderedQueryable<CK4C>> orderBy = null;
            if (!string.IsNullOrEmpty(orderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<CK4C>(orderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);

            return dbData.ToList();
        }
        
        public List<Ck4CDto> GetCompletedDocument()
        {
            var dtData = _repository.Get(x => x.STATUS == Enums.DocumentStatus.Completed, null, includeTables).ToList();

            return Mapper.Map<List<Ck4CDto>>(dtData);
        }
        
        public Ck4cReportDto GetCk4cReportDataById(int id)
        {
            var ck4cItemGroupByDate = new Dictionary<string, List<Ck4cReportItemDto>>();
            var dtData = _repository.Get(c => c.CK4C_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var saldoDate = new DateTime();

            var result = new Ck4cReportDto();
            result.Detail.Ck4cId = dtData.CK4C_ID;
            result.Detail.Number = dtData.NUMBER;

            var ck4cReportedOn = dtData.REPORTED_ON.Value;
            var ck4cMonth = _monthBll.GetMonth(ck4cReportedOn.Month).MONTH_NAME_IND;
            result.Detail.ReportedOn = string.Format("{0} {1} {2}", ck4cReportedOn.Day, ck4cMonth, ck4cReportedOn.Year);
            result.Detail.ReportedOnDay = ck4cReportedOn.Day.ToString();
            result.Detail.ReportedOnMonth = ck4cMonth;
            result.Detail.ReportedOnYear = ck4cReportedOn.Year.ToString();

            if (dtData.REPORTED_PERIOD == 1)
            {
                result.Detail.ReportedPeriodStart = "1";
                result.Detail.ReportedPeriodEnd = "14";
                saldoDate = new DateTime(dtData.REPORTED_YEAR.Value, dtData.REPORTED_MONTH.Value, 1);
            }
            else if (dtData.REPORTED_PERIOD == 2)
            {
                var endDate = new DateTime(dtData.REPORTED_YEAR.Value, dtData.REPORTED_MONTH.Value, 1).AddMonths(1).AddDays(-1).Day.ToString();
                result.Detail.ReportedPeriodStart = "15";
                result.Detail.ReportedPeriodEnd = endDate;
                saldoDate = new DateTime(dtData.REPORTED_YEAR.Value, dtData.REPORTED_MONTH.Value, 15);
            }

            var ck4cPeriodMonth = _monthBll.GetMonth(dtData.REPORTED_MONTH.Value).MONTH_NAME_IND;
            result.Detail.ReportedMonth = ck4cPeriodMonth;

            result.Detail.ReportedYear = dtData.REPORTED_YEAR.Value.ToString();
            result.Detail.CompanyName = dtData.COMPANY_NAME;

            var addressPlant = _plantBll.GetPlantByNppbkc(dtData.NPPBKC_ID).Select(x => x.WERKS).Distinct().ToArray();
            var addressList = addressPlant;
            var address = string.Empty;

            if(dtData.PLANT_ID != null)
            {
                addressList = dtData.CK4C_ITEM.Select(x => x.WERKS).Distinct().ToArray();
            }

            foreach(var data in addressList)
            {
                address += "- " + _plantBll.GetT001WById(data).ADDRESS + Environment.NewLine;
            }

            string prodTypeDistinct = string.Empty;
            string currentProdType = string.Empty;
            List<Ck4cReportItemDto> tempListck4c1 = new List<Ck4cReportItemDto>();
            //add data details of CK-4C sebelumnya
            foreach (var item in addressPlant)
            {
                Int32 isInt;
                var activeBrand = _brandBll.GetBrandCeBylant(item).Where(x => Int32.TryParse(x.BRAND_CONTENT, out isInt) && x.EXC_GOOD_TYP == "01").OrderBy(x => x.PROD_CODE);

                foreach (var data in activeBrand)
                {
                    if(currentProdType != data.PROD_CODE)
                    {
                        currentProdType = data.PROD_CODE;
                        prodTypeDistinct += "|" + data.PROD_CODE;
                    }

                    var ck4cItem = new Ck4cReportItemDto();

                    var unpackedQty = _ck4cItemBll.GetDataByPlantAndFacode(item, data.FA_CODE).Where(c => c.ProdDate < saldoDate).LastOrDefault();

                    var oldData = _productionBll.GetOldSaldo(dtData.COMPANY_ID, item, data.FA_CODE, saldoDate).LastOrDefault();

                    var oldUnpacked = oldData == null ? 0 : oldData.QtyUnpacked.Value;

                    var strOldUnpacked = oldUnpacked == 0 ? "Nihil" : String.Format("{0:n}", oldUnpacked);

                    ck4cItem.CollumNo = 0;
                    ck4cItem.No = string.Empty;
                    ck4cItem.NoProd = string.Empty;
                    ck4cItem.ProdDate = string.Empty;

                    var prodType = _prodTypeBll.GetById(data.PROD_CODE);
                    ck4cItem.ProdCode = data.PROD_CODE;
                    ck4cItem.ProdType = prodType.PRODUCT_ALIAS;

                    ck4cItem.SumBtg = "Nihil";
                    ck4cItem.BtgGr = "Nihil";

                    var brand = _brandBll.GetById(item, data.FA_CODE);
                    ck4cItem.Merk = brand.BRAND_CE;

                    ck4cItem.Isi = Convert.ToInt32(brand.BRAND_CONTENT) == 0 ? "Nihil" : String.Format("{0:n}", Convert.ToInt32(brand.BRAND_CONTENT));
                    ck4cItem.Hje = brand.HJE_IDR == null ? "Nihil" : String.Format("{0:n}", brand.HJE_IDR);
                    ck4cItem.Total = "Nihil";
                    ck4cItem.ProdWaste = unpackedQty == null ? strOldUnpacked : (unpackedQty.UnpackedQty == 0 ? "Nihil" : String.Format("{0:n}", unpackedQty.UnpackedQty));
                    ck4cItem.Comment = "Saldo CK-4C Sebelumnya";

                    //result.Ck4cItemList.Add(ck4cItem);
                    tempListck4c1.Add(ck4cItem);
                }
                
            }
            //distinct var tempListck4c1
            var tempCk4cDto = tempListck4c1.Select(c => new
            {
                c.CollumNo,
                c.No,
                c.NoProd,
                c.ProdDate,
                c.ProdCode,
                c.ProdType,
                c.SumBtg,
                c.BtgGr,
                c.Merk,
                c.Isi,
                c.Hje,
                c.Total,
                c.ProdWaste,
                c.Comment
            });

            var distinctTempCk4cDto = tempCk4cDto.Distinct().ToList();

            var newDistinctCk4cReportItemDto = distinctTempCk4cDto.Select(c => new Ck4cReportItemDto
            {
                CollumNo = c.CollumNo,
                No = c.No,
                NoProd = c.NoProd,
                ProdDate = c.ProdDate,
                ProdCode = c.ProdCode,
                ProdType = c.ProdType,
                SumBtg = c.SumBtg,
                BtgGr = c.BtgGr,
                Merk = c.Merk,
                Isi = c.Isi,
                Hje = c.Hje,
                Total = c.Total,
                ProdWaste = c.ProdWaste,
                Comment = c.Comment

            }).ToList();

            //add to dictionary group by date empty
            ck4cItemGroupByDate.Add(String.Empty, newDistinctCk4cReportItemDto);
            result.Detail.CompanyAddress = address;

            var plant = _plantBll.GetT001WById(dtData.PLANT_ID);
            var nppbkc = dtData.NPPBKC_ID;
            result.Detail.Nppbkc = nppbkc;

            var poaUser = dtData.APPROVED_BY_POA == null ? dtData.CREATED_BY : dtData.APPROVED_BY_POA;

            var poa = _poabll.GetDetailsById(poaUser);
            if (poa != null)
            {
                result.Detail.Poa = poa.PRINTED_NAME;
            }

            var nBatang = dtData.CK4C_ITEM.Where(c => c.UOM_PROD_QTY == "Btg").Sum(c => c.PROD_QTY);
            var nGram = dtData.CK4C_ITEM.Where(c => c.UOM_PROD_QTY == "G").Sum(c => c.PROD_QTY);
            var nThousand = dtData.CK4C_ITEM.Where(c => c.UOM_PROD_QTY == "TH").Sum(c => c.PROD_QTY) * 1000;
            var nKGram = dtData.CK4C_ITEM.Where(c => c.UOM_PROD_QTY == "KG").Sum(c => c.PROD_QTY) * 1000;
            nGram = nGram + nKGram;
            nBatang = nBatang + nThousand;

            result.Detail.NBatang = nBatang.ToString();
            result.Detail.NGram = nGram.ToString();

            var prodTotal = string.Empty;
            if (nBatang != 0 && nGram != 0)
            {
                prodTotal = String.Format("{0:n}",nBatang) + " batang dan " + String.Format("{0:n}",nGram) + " gram";
            }
            else if (nBatang == 0 && nGram != 0)
            {
                prodTotal = String.Format("{0:n}",nGram) + " gram";
            }
            else if (nBatang != 0 && nGram == 0)
            {
                prodTotal = String.Format("{0:n}",nBatang) + " batang";
            }

            var city = plant == null ? _nppbkcbll.GetById(dtData.NPPBKC_ID).CITY : plant.ORT01;
            result.Detail.City = city;

            var headerFooterData = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput()
            {
                FormTypeId = Enums.FormType.CK4C,
                CompanyCode = dtData.COMPANY_ID
            });

            result.HeaderFooter = headerFooterData;
            var i = 0;
            List<Ck4cUnpacked> unpackedList = new List<Ck4cUnpacked>();

            //add data details of current CK-4C
            for (var j = Convert.ToInt32(result.Detail.ReportedPeriodStart); j <= Convert.ToInt32(result.Detail.ReportedPeriodEnd); j++)
            {
                i = i + 1;
                var prodDate = j + "-" + result.Detail.ReportedMonth.Substring(0, 3) + "-" + result.Detail.ReportedYear;
                var prodDateFormat = new DateTime(Convert.ToInt32(result.Detail.ReportedYear), Convert.ToInt32(dtData.REPORTED_MONTH), j);
                var lastProdDate = prodDateFormat.AddDays(-1);
                var dateStart = new DateTime(Convert.ToInt32(result.Detail.ReportedYear), Convert.ToInt32(dtData.REPORTED_MONTH), Convert.ToInt32(result.Detail.ReportedPeriodStart));
                List<Ck4cReportItemDto> tempListck4c2 = new List<Ck4cReportItemDto>();
                foreach (var item in addressPlant)
                {
                    Int32 isInt;
                    var activeBrand = _brandBll.GetBrandCeBylant(item).Where(x => Int32.TryParse(x.BRAND_CONTENT, out isInt) && x.EXC_GOOD_TYP == "01");

                    foreach (var data in activeBrand.Distinct())
                    {
                        var ck4cItem = new Ck4cReportItemDto();
                        var brand = _brandBll.GetById(item, data.FA_CODE);
                        var prodType = _prodTypeBll.GetById(data.PROD_CODE);
                        var itemCk4c = dtData.CK4C_ITEM.Where(c => c.WERKS == item && c.FA_CODE == data.FA_CODE && c.PROD_DATE == prodDateFormat);
                        var prodQty = itemCk4c.Sum(x => x.PROD_QTY);
                        var packedQty = itemCk4c.Sum(x => x.PACKED_QTY);
                        var unpackedQty = itemCk4c.Sum(x => x.UNPACKED_QTY);
                        var remarks = itemCk4c.FirstOrDefault();
                        var total = brand.BRAND_CONTENT == null ? 0 : packedQty / Convert.ToInt32(brand.BRAND_CONTENT);

                        if (unpackedQty == 0)
                        {
                            var wasteData = _wasteBll.GetExistDto(dtData.COMPANY_ID, item, data.FA_CODE, prodDateFormat);

                            var oldWaste = wasteData == null ? 0 : wasteData.PACKER_REJECT_STICK_QTY;

                            var lastUnpacked = unpackedList.Where(c => c.PlantId == item && c.Facode == data.FA_CODE && c.ProdDate == lastProdDate).Sum(x => x.Unpacked);

                            var lastSaldo = _ck4cItemBll.GetDataByPlantAndFacode(item, data.FA_CODE).Where(c => c.ProdDate < saldoDate).LastOrDefault();

                            var oldData = _productionBll.GetOldSaldo(dtData.COMPANY_ID, item, data.FA_CODE, saldoDate).LastOrDefault();

                            var oldUnpacked = oldData == null ? 0 : oldData.QtyUnpacked.Value;

                            var lastSaldoUnpacked = lastSaldo == null ? oldUnpacked : lastSaldo.UnpackedQty;

                            unpackedQty = (lastUnpacked == 0 ? lastSaldoUnpacked : lastUnpacked) - oldWaste;
                        }

                        ck4cItem.CollumNo = i;
                        ck4cItem.No = i.ToString();
                        ck4cItem.NoProd = i.ToString();
                        ck4cItem.ProdDate = prodDate;
                        ck4cItem.ProdCode = data.PROD_CODE;
                        ck4cItem.ProdType = prodType.PRODUCT_ALIAS;
                        ck4cItem.SumBtg = prodQty == 0 ? "Nihil" : String.Format("{0:n}", prodQty);
                        ck4cItem.BtgGr = packedQty == null || packedQty == 0 ? "Nihil" : String.Format("{0:n}", packedQty);
                        ck4cItem.Merk = brand.BRAND_CE;
                        ck4cItem.Isi = Convert.ToInt32(brand.BRAND_CONTENT) == 0 ? "Nihil" : String.Format("{0:n}", Convert.ToInt32(brand.BRAND_CONTENT));
                        ck4cItem.Hje = brand.HJE_IDR == null || brand.HJE_IDR == 0 ? "Nihil" : String.Format("{0:n}", brand.HJE_IDR);
                        ck4cItem.Total = total == null || total == 0 ? "Nihil" : String.Format("{0:n}", total);
                        ck4cItem.ProdWaste = unpackedQty == null || unpackedQty == 0 ? "Nihil" : String.Format("{0:n}", unpackedQty);
                        ck4cItem.Comment = remarks == null ? string.Empty : remarks.REMARKS;

                        //result.Ck4cItemList.Add(ck4cItem);
                        tempListck4c2.Add(ck4cItem);

                        var unpackedItem = new Ck4cUnpacked();
                        unpackedItem.PlantId = item;
                        unpackedItem.Facode = data.FA_CODE;
                        unpackedItem.ProdDate = prodDateFormat;
                        unpackedItem.Unpacked = unpackedQty == null ? 0 : unpackedQty.Value;

                        unpackedList.Add(unpackedItem);
                    }
                    
                }
                //distinct var tempListck4c2
                var tempCk4cDto2 = tempListck4c2.Select(c => new
                {
                    c.CollumNo,
                    c.No,
                    c.NoProd,
                    c.ProdDate,
                    c.ProdCode,
                    c.ProdType,
                    c.SumBtg,
                    c.BtgGr,
                    c.Merk,
                    c.Isi,
                    c.Hje,
                    c.Total,
                    c.ProdWaste,
                    c.Comment
                });

                var distinctTempCk4cDto2 = tempCk4cDto2.Distinct().ToList();

                var newDistinctCk4cReportItemDto2 = distinctTempCk4cDto2.Select(c => new Ck4cReportItemDto
                {
                    CollumNo = c.CollumNo,
                    No = c.No,
                    NoProd = c.NoProd,
                    ProdDate = c.ProdDate,
                    ProdCode = c.ProdCode,
                    ProdType = c.ProdType,
                    SumBtg = c.SumBtg,
                    BtgGr = c.BtgGr,
                    Merk = c.Merk,
                    Isi = c.Isi,
                    Hje = c.Hje,
                    Total = c.Total,
                    ProdWaste = c.ProdWaste,
                    Comment = c.Comment

                }).ToList();

                //add to dictionary group by date
                ck4cItemGroupByDate.Add(prodDate, newDistinctCk4cReportItemDto2);
            }

             //order brand by prod alias using Dictionary<string, List<Ck4cReportItemDto>> each date
            foreach (var item in ck4cItemGroupByDate)
            {
               //insert result.ck4itemList again ordered brand
                var listItem = ck4cItemGroupByDate.Where(c => c.Key == item.Key).Select(c => c.Value.OrderBy(d => d.Merk).OrderBy(d => d.ProdCode)).FirstOrDefault().ToList();
                result.Ck4cItemList.AddRange(listItem);
            }

            var prodAlias = string.Empty;
            var sumTotal = string.Empty;
            var btgTotal = string.Empty;

            if(prodTypeDistinct != string.Empty)
            {
                var brandType = prodTypeDistinct.Substring(1).Split('|').Distinct();

                foreach (var data in brandType)
                {
                    var sum = dtData.CK4C_ITEM.Where(x => x.PROD_CODE == data).Sum(x => x.PROD_QTY);
                    var total = dtData.CK4C_ITEM.Where(x => x.PROD_CODE == data).Sum(x => x.PACKED_QTY);

                    prodAlias += _prodTypeBll.GetById(data).PRODUCT_ALIAS + Environment.NewLine;
                    sumTotal += (sum == 0 ? "Nihil" : String.Format("{0:n}", sum)) + Environment.NewLine;
                    btgTotal += (total == 0 ? "Nihil" : String.Format("{0:n}", total)) + Environment.NewLine;
                }
            }

            result.Ck4cTotal.ProdType = prodAlias;
            result.Ck4cTotal.ProdTotal = sumTotal;
            result.Ck4cTotal.ProdBtg = btgTotal;

            if (nBatang == 0 && nGram != 0)
            {
                if (result.Ck4cItemList.Where(x => x.ProdType == "SKM" || x.ProdType == "SPM" || x.ProdType == "CRT" || x.ProdType == "SKT").Count() > 0)
                    prodTotal = "Nihil batang dan " + String.Format("{0:n}", nGram) + " gram";
            }
            else if (nBatang != 0 && nGram == 0)
            {
                if (result.Ck4cItemList.Where(x => x.ProdType == "TIS").Count() > 0)
                    prodTotal = String.Format("{0:n}", nBatang) + " batang dan Nihil gram";
            }

            result.Detail.ProdTotal = prodTotal;

            return result;
        }

        private bool SetChangesHistory(CK4C origin, Ck4CDto data, string userId)
        {
            var changed = false;

            var changeData = new Dictionary<string, bool>();
            changeData.Add("COMPANY_CODE", origin.COMPANY_ID == data.CompanyId);
            changeData.Add("PLANT", origin.PLANT_ID == data.PlantId);
            changeData.Add("NPPBKC", origin.NPPBKC_ID == data.NppbkcId);
            changeData.Add("REPORTED_ON", origin.REPORTED_ON == data.ReportedOn);
            changeData.Add("REPORTED_PERIOD", origin.REPORTED_PERIOD == data.ReportedPeriod);
            changeData.Add("REPORTED_MONTH", origin.REPORTED_MONTH == data.ReportedMonth);
            changeData.Add("REPORTED_YEAR", origin.REPORTED_YEAR == data.ReportedYears);

            foreach (var listChange in changeData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Enums.MenuList.CK4C,
                        FORM_ID = data.Ck4CId.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };

                    switch (listChange.Key)
                    {
                        case "COMPANY_CODE":
                            changes.OLD_VALUE = origin.COMPANY_NAME;
                            changes.NEW_VALUE = data.CompanyName;
                            changes.FIELD_NAME = "Company";
                            break;
                        case "PLANT":
                            changes.OLD_VALUE = origin.PLANT_ID + "-" + origin.PLANT_NAME;
                            changes.NEW_VALUE = data.PlantId + "-" + data.PlantName;
                            changes.FIELD_NAME = "Plant";
                            break;
                        case "NPPBKC":
                            changes.OLD_VALUE = origin.NPPBKC_ID;
                            changes.NEW_VALUE = data.NppbkcId;
                            changes.FIELD_NAME = "Nppbkc";
                            break;
                        case "REPORTED_ON":
                            changes.OLD_VALUE = origin.REPORTED_ON.Value.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = data.ReportedOn.Value.ToString("dd MMM yyyy");
                            changes.FIELD_NAME = "Reported On";
                            break;
                        case "REPORTED_PERIOD":
                            changes.OLD_VALUE = origin.REPORTED_PERIOD.ToString();
                            changes.NEW_VALUE = data.ReportedPeriod.ToString();
                            changes.FIELD_NAME = "Reported Period";
                            break;
                        case "REPORTED_MONTH":
                            changes.OLD_VALUE = origin.MONTH.MONTH_NAME_IND;
                            changes.NEW_VALUE = data.MonthNameIndo;
                            changes.FIELD_NAME = "Reported Month";
                            break;
                        case "REPORTED_YEAR":
                            changes.OLD_VALUE = origin.REPORTED_YEAR.ToString();
                            changes.NEW_VALUE = data.ReportedYears.ToString();
                            changes.FIELD_NAME = "Reported Year";
                            break;
                        default: break;
                    }
                    _changesHistoryBll.AddHistory(changes);

                    changed = true;
                }
            }

            return changed;
        }

        public bool AllowEditCompletedDocument(Ck4CDto item, string userId)
        {
            var isAllow = false;

            if(item.CreatedBy == userId || item.ApprovedByPoa == userId)
                isAllow = true;

            return isAllow;
        }

        public Ck4CDto GetByItem(Ck4CDto item)
        {
            var dbData = _repository.Get(c => c.PLANT_ID == item.PlantId && c.NPPBKC_ID == item.NppbkcId
                                            && c.REPORTED_PERIOD == item.ReportedPeriod && c.REPORTED_MONTH == item.ReportedMonth
                                            && c.REPORTED_YEAR == item.ReportedYears, null, includeTables).FirstOrDefault();

            var mapResult = Mapper.Map<Ck4CDto>(dbData);

            return mapResult;
        }

        #region SummaryReport

        public List<Ck4CSummaryReportDto> GetSummaryReportsByParam(Ck4CGetSummaryReportByParamInput input)
        {

            Expression<Func<CK4C, bool>> queryFilter = PredicateHelper.True<CK4C>();

            if (!string.IsNullOrEmpty(input.Ck4CNo))
            {
                queryFilter = queryFilter.And(c => c.NUMBER == input.Ck4CNo);
            }

         
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID.Contains(input.PlantId));
            }
            
            var rc = _repository.Get(queryFilter, null, includeTables).ToList();
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var data = SetDataSummaryReport(rc);

            if (input.isForExport)
                data = SetDataSummaryForExport(rc);

            return data;
        }

        private List<Ck4CSummaryReportDto> SetDataSummaryReport(List<CK4C> listCk4C)
        {
            var result = new List<Ck4CSummaryReportDto>();

            foreach (var dtData in listCk4C)
            {
                var summaryDto = new Ck4CSummaryReportDto();

                summaryDto.Ck4CNo = dtData.NUMBER;
                summaryDto.CeOffice = dtData.COMPANY_ID;
                summaryDto.BasedOn = dtData.PLANT_ID == null ? "NPPBKC" : "PLANT";
                summaryDto.PlantId = dtData.PLANT_ID;
                summaryDto.PlantDescription = dtData.PLANT_NAME;
                summaryDto.LicenseNumber = dtData.NPPBKC_ID;
                summaryDto.ReportPeriod = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.REPORTED_ON);
                summaryDto.Period = dtData.REPORTED_PERIOD.ToString();
                summaryDto.Month = dtData.MONTH.MONTH_NAME_ENG;
                summaryDto.Year = dtData.REPORTED_YEAR.ToString();
                summaryDto.PoaApproved = dtData.APPROVED_BY_POA == null ? "-" : dtData.APPROVED_BY_POA;
                summaryDto.ManagerApproved = dtData.APPROVED_BY_MANAGER == null ? "-" : dtData.APPROVED_BY_MANAGER;
                summaryDto.Status = EnumHelper.GetDescription(dtData.STATUS);

                var prodDate = new List<string>();
                var faCode = new List<string>();
                var tobacco = new List<string>();
                var brandDesc = new List<string>();
                var hje = new List<string>();
                var tariff = new List<string>();
                var content = new List<string>();
                var packedQty = new List<string>();
                var packedQtyInPack = new List<string>();
                var unpackedQty = new List<string>();
                var prodQty = new List<string>();
                var uomProdQty = new List<string>();
                var remarks = new List<string>();

                foreach (var ck4CItem in dtData.CK4C_ITEM)
                {
                    prodDate.Add(ConvertHelper.ConvertDateToStringddMMMyyyy(ck4CItem.PROD_DATE));
                    faCode.Add(ck4CItem.FA_CODE);
                    
                    var dbBrand = _brandRegistrationService.GetByPlantIdAndFaCode(ck4CItem.WERKS, ck4CItem.FA_CODE);

                    if (dbBrand != null)
                    {
                        tobacco.Add(dbBrand.ZAIDM_EX_PRODTYP != null
                            ? dbBrand.ZAIDM_EX_PRODTYP.PRODUCT_TYPE
                            : string.Empty);
                        brandDesc.Add(dbBrand.BRAND_CE);
                    }

                    var contentPerPack = ck4CItem.CONTENT_PER_PACK == null ? 0 : ck4CItem.CONTENT_PER_PACK.Value;
                    var packedInPack = ck4CItem.PACKED_IN_PACK == null ? 0 : ck4CItem.PACKED_IN_PACK.Value;

                    hje.Add(String.Format("{0:n}", ck4CItem.HJE_IDR.Value));
                    tariff.Add(String.Format("{0:n}", ck4CItem.TARIFF.Value));
                    content.Add(String.Format("{0:n}", contentPerPack));
                    packedQty.Add(String.Format("{0:n}", ck4CItem.PACKED_QTY.Value));
                    packedQtyInPack.Add(String.Format("{0:n}", packedInPack));
                    unpackedQty.Add(String.Format("{0:n}", ck4CItem.UNPACKED_QTY.Value));
                    prodQty.Add(String.Format("{0:n}", ck4CItem.PROD_QTY));
                    uomProdQty.Add(ck4CItem.UOM_PROD_QTY);
                    remarks.Add(ck4CItem.REMARKS);
                }

                summaryDto.ProductionDate = prodDate;
                summaryDto.FaCode = faCode;
                summaryDto.TobaccoProductType = tobacco;
                summaryDto.BrandDescription = brandDesc;
                summaryDto.Hje = hje;
                summaryDto.Tariff = tariff;
                summaryDto.Content = content;
                summaryDto.PackedQty = packedQty;
                summaryDto.PackedQtyInPack = packedQtyInPack;
                summaryDto.UnPackQty = unpackedQty;
                summaryDto.ProducedQty = prodQty;
                summaryDto.UomProducedQty = uomProdQty;
                summaryDto.Remarks = remarks;

                result.Add(summaryDto);
            }

            return result;
        }

        private List<Ck4CSummaryReportDto> SetDataSummaryForExport(List<CK4C> listCk4C)
        {
            var result = new List<Ck4CSummaryReportDto>();

            foreach (var dtData in listCk4C)
            {
                foreach (var ck4CItem in dtData.CK4C_ITEM)
                {
                    var summaryDto = new Ck4CSummaryReportDto();

                    summaryDto.Ck4CNo = dtData.NUMBER;
                    summaryDto.CeOffice = dtData.COMPANY_ID;
                    summaryDto.BasedOn = dtData.PLANT_ID == null ? "NPPBKC" : "PLANT";
                    summaryDto.PlantId = dtData.PLANT_ID;
                    summaryDto.PlantDescription = dtData.PLANT_NAME;
                    summaryDto.LicenseNumber = dtData.NPPBKC_ID;
                    summaryDto.ReportPeriod = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.REPORTED_ON);
                    summaryDto.Period = dtData.REPORTED_PERIOD.ToString();
                    summaryDto.Month = dtData.MONTH.MONTH_NAME_ENG;
                    summaryDto.Year = dtData.REPORTED_YEAR.ToString();
                    summaryDto.PoaApproved = dtData.APPROVED_BY_POA == null ? "-" : dtData.APPROVED_BY_POA;
                    summaryDto.ManagerApproved = dtData.APPROVED_BY_MANAGER == null ? "-" : dtData.APPROVED_BY_MANAGER;
                    summaryDto.Status = EnumHelper.GetDescription(dtData.STATUS);

                    var prodDate = new List<string>();
                    var faCode = new List<string>();
                    var tobacco = new List<string>();
                    var brandDesc = new List<string>();
                    var hje = new List<string>();
                    var tariff = new List<string>();
                    var content = new List<string>();
                    var packedQty = new List<string>();
                    var packedQtyInPack = new List<string>();
                    var unpackedQty = new List<string>();
                    var prodQty = new List<string>();
                    var uomProdQty = new List<string>();
                    var remarks = new List<string>();

                    prodDate.Add(ConvertHelper.ConvertDateToStringddMMMyyyy(ck4CItem.PROD_DATE));
                    faCode.Add(ck4CItem.FA_CODE);

                    var dbBrand = _brandRegistrationService.GetByPlantIdAndFaCode(ck4CItem.WERKS, ck4CItem.FA_CODE);

                    if (dbBrand != null)
                    {
                        tobacco.Add(dbBrand.ZAIDM_EX_PRODTYP != null
                            ? dbBrand.ZAIDM_EX_PRODTYP.PRODUCT_TYPE
                            : string.Empty);
                        brandDesc.Add(dbBrand.BRAND_CE);
                    }

                    var contentPerPack = ck4CItem.CONTENT_PER_PACK == null ? 0 : ck4CItem.CONTENT_PER_PACK.Value;
                    var packedInPack = ck4CItem.PACKED_IN_PACK == null ? 0 : ck4CItem.PACKED_IN_PACK.Value;

                    hje.Add(String.Format("{0:n}", ck4CItem.HJE_IDR.Value));
                    tariff.Add(String.Format("{0:n}", ck4CItem.TARIFF.Value));
                    content.Add(String.Format("{0:n}", contentPerPack));
                    packedQty.Add(String.Format("{0:n}", ck4CItem.PACKED_QTY.Value));
                    packedQtyInPack.Add(String.Format("{0:n}", packedInPack));
                    unpackedQty.Add(String.Format("{0:n}", ck4CItem.UNPACKED_QTY.Value));
                    prodQty.Add(String.Format("{0:n}", ck4CItem.PROD_QTY));
                    uomProdQty.Add(ck4CItem.UOM_PROD_QTY);
                    remarks.Add(ck4CItem.REMARKS);

                    summaryDto.ProductionDate = prodDate;
                    summaryDto.FaCode = faCode;
                    summaryDto.TobaccoProductType = tobacco;
                    summaryDto.BrandDescription = brandDesc;
                    summaryDto.Hje = hje;
                    summaryDto.Tariff = tariff;
                    summaryDto.Content = content;
                    summaryDto.PackedQty = packedQty;
                    summaryDto.PackedQtyInPack = packedQtyInPack;
                    summaryDto.UnPackQty = unpackedQty;
                    summaryDto.ProducedQty = prodQty;
                    summaryDto.UomProducedQty = uomProdQty;
                    summaryDto.Remarks = remarks;

                    result.Add(summaryDto);
                }
            }

            return result;
        }

        #endregion
    }
}
