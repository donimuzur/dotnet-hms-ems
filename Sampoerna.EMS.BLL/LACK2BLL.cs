using System.Configuration;
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
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class LACK2BLL : ILACK2BLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IMonthBLL _monthBll;
        private IPOABLL _poaBll;
        private IUserBLL _userBll;

        private IChangesHistoryBLL _changesHistoryBll;
        private string includeTables = "MONTH";
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IPOABLL _poabll;
        private IPlantBLL _plantBll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IMessageService _messageService;
        private IWorkflowBLL _workflowBll;

        private ILack2Service _lack2Service;
        
        public LACK2BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            
            _lack2Service = new Lack2Service(_uow, _logger);

            _monthBll = new MonthBLL(_uow, _logger);

            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _poabll = new POABLL(_uow, _logger);
            _plantBll = new PlantBLL(_uow, _logger);
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _messageService = new MessageService(_logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
        }

        public List<Lack2Dto> GetAll()
        {
            var data = _lack2Service.GetAll();
            return Mapper.Map<List<Lack2Dto>>(data);
        }

        private void SetChangesHistory(Lack2Dto origin, Lack2Dto data, string userId)
        {
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
                }
            }
        }

        #region workflow

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
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocument(input);
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.GovPartialApprove:
                    GovPartialApproveDocument(input);
                    isNeedSendNotif = false;
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

            _workflowHistoryBll.Save(dbData);

        }

        private void SubmitDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                //var dbData = _lack1Service.GetById(input.DocumentId.Value);
                var dbData = _repository.GetByID(input.DocumentId.Value);

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

                var dbData = _repository.GetByID(input.DocumentId.Value);

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
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);

                if (input.UserRole == Enums.UserRole.POA)
                {
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                    //dbData.APPROVED_BY_POA = input.UserId;
                    //dbData.APPROVED_DATE_POA = DateTime.Now;
                    dbData.APPROVED_BY = input.UserId;
                    dbData.APPROVED_DATE = DateTime.Now;
                }
                else
                {
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
                //var dbData = _lack1Service.GetById(input.DocumentId.Value);
                var dbData = _repository.GetByID(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                    dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                    dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Draft);

                //change back to draft
                dbData.STATUS = Enums.DocumentStatus.Draft;

                //todo ask
                dbData.APPROVED_BY = null;
                dbData.APPROVED_DATE = null;

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _repository.GetByID(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.FullApproved);

                dbData.LACK2_DOCUMENT = null;
                dbData.STATUS = Enums.DocumentStatus.Completed;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
                dbData.LACK2_DOCUMENT = Mapper.Map<List<LACK2_DOCUMENT>>(input.AdditionalDocumentData.Lack2DecreeDoc);
                dbData.GOV_STATUS = Enums.DocumentStatusGov.FullApproved;

                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;

                dbData.APPROVED_BY = input.UserId;
                dbData.APPROVED_DATE = DateTime.Now;

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);

        }

        private void GovPartialApproveDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _repository.GetByID(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.PartialApproved);

                input.DocumentNumber = dbData.LACK2_NUMBER;

                dbData.LACK2_DOCUMENT = null;
                dbData.STATUS = Enums.DocumentStatus.Completed;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
                dbData.LACK2_DOCUMENT = Mapper.Map<List<LACK2_DOCUMENT>>(input.AdditionalDocumentData.Lack2DecreeDoc);
                dbData.GOV_STATUS = Enums.DocumentStatusGov.PartialApproved;

                dbData.APPROVED_BY = input.UserId;
                dbData.APPROVED_DATE = DateTime.Now;

                input.DocumentNumber = dbData.LACK2_NUMBER;
            }

            AddWorkflowHistory(input);
        }

        private void GovRejectedDocument(Lack2WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _repository.GetByID(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.GovRejected);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.Rejected);

                dbData.STATUS = Enums.DocumentStatus.GovRejected;
                dbData.GOV_STATUS = Enums.DocumentStatusGov.Rejected;
                dbData.LACK2_DOCUMENT = Mapper.Map<List<LACK2_DOCUMENT>>(input.AdditionalDocumentData.Lack2DecreeDoc);
                dbData.APPROVED_BY = input.UserId;
                dbData.APPROVED_DATE = DateTime.Now;

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

        private void SendEmailWorkflow(Lack2WorkflowDocumentInput input)
        {
            //todo: body message from email template
            //todo: to = ?
            //todo: subject = from email template
            //var to = "irmansulaeman41@gmail.com";
            //var subject = "this is subject for " + input.DocumentNumber;
            //var body = "this is body message for " + input.DocumentNumber;
            //var from = "a@gmail.com";

            if (input.DocumentId != null)
            {
                var lack2Data = Mapper.Map<Lack2Dto>(_repository.GetByID(input.DocumentId.Value));

                var mailProcess = ProsesMailNotificationBody(lack2Data, input.ActionType);

                _messageService.SendEmailToList(mailProcess.To, mailProcess.Subject, mailProcess.Body, true);
            }
        }
        
        private string GetManagerEmail(string poaId)
        {
            var managerId = _poaBll.GetManagerIdByPoaId(poaId);
            var managerDetail = _userBll.GetUserById(managerId);
            return managerDetail.EMAIL;
        }

        private Lack2MailNotification ProsesMailNotificationBody(Lack2Dto lackData, Enums.ActionType actionType)
        {
            var bodyMail = new StringBuilder();
            var rc = new Lack2MailNotification();

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
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/Lack2/Detail/" + lackData.Lack2Id + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (actionType)
            {
                case Enums.ActionType.Submit:
                    if (lackData.Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        var poaList = _poaBll.GetPoaByNppbkcId(lackData.NppbkcId);
                        foreach (var poaDto in poaList)
                        {
                            rc.To.Add(poaDto.POA_EMAIL);
                        }
                    }
                    else if (lackData.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var managerId = _poaBll.GetManagerIdByPoaId(lackData.CreatedBy);
                        var managerDetail = _userBll.GetUserById(managerId);
                        rc.To.Add(managerDetail.EMAIL);
                    }
                    break;
                case Enums.ActionType.Approve:
                    if (lackData.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(lackData.ApprovedBy));
                    }
                    else if (lackData.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(lackData.CreatedBy);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(lackData.CreatedBy);
                            rc.To.Add(userData.EMAIL);
                        }
                    }
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(lackData.CreatedBy);
                    rc.To.Add(userDetail.EMAIL);
                    break;
            }
            rc.Body = bodyMail.ToString();
            return rc;
        }

        private class Lack2MailNotification
        {
            public Lack2MailNotification()
            {
                To = new List<string>();
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
        }

        #endregion

        private Lack2GeneratedOutput GenerateLack2Data(Lack2GenerateDataParamInput input)
        {
            var rc = new Lack2GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            #region validation



            #endregion

            return rc;
        }

    }
}
