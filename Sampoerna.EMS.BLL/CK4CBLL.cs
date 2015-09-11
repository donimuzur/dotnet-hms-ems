using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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

        private string includeTables = "POA, MONTH, CK4C_ITEM";

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
        }

        public List<Ck4CDto> GetAllByParam(Ck4CGetByParamInput input)
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

            Func<IQueryable<CK4C>, IOrderedQueryable<CK4C>> orderBy = null;
            if (!string.IsNullOrEmpty(input.ShortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<CK4C>(input.ShortOrderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            var mapResult = Mapper.Map<List<Ck4CDto>>(dbData.ToList());

            return mapResult;
        }

        public List<Ck4CDto> GetAll()
        {
            var dtData = _repository.Get(null, null, includeTables).ToList();

            return Mapper.Map<List<Ck4CDto>>(dtData);
        }

        public Ck4CDto Save(Ck4CDto item)
        {
            CK4C model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }
            
            try
            {
                if (item.Ck4CId > 0)
                {
                    //update
                    model = _repository.Get(c => c.CK4C_ID == item.Ck4CId, null, includeTables).FirstOrDefault();

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    _ck4cItemBll.DeleteByCk4cId(item.Ck4CId);

                    Mapper.Map<Ck4CDto, CK4C>(item, model);
                    model.CK4C_ITEM = null;

                    model.CK4C_ITEM = Mapper.Map<List<CK4C_ITEM>>(item.Ck4cItem);
                }
                else
                {
                    model = Mapper.Map<CK4C>(item);
                    _repository.InsertOrUpdate(model);
                }
                
                _uow.SaveChanges();
                var history = new WorkflowHistoryDto();
                history.FORM_ID = model.CK4C_ID;
                history.ACTION = GetActionType(model.STATUS, item.ModifiedBy);
                history.ACTION_BY = GetActionBy(item);
                history.ACTION_DATE = DateTime.Now;
                history.FORM_NUMBER = item.Number;
                history.FORM_TYPE_ID = Enums.FormType.CK4C;
                history.COMMENT = item.Comment;
                //set workflow history
                var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
                history.ROLE = getUserRole;
                _workflowHistoryBll.AddHistory(history);
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return item;
        }

        private Enums.ActionType GetActionType(Enums.DocumentStatus docStatus, string modifiedBy)
        {
            if (docStatus == Enums.DocumentStatus.Draft)
            {
                if (modifiedBy != null)
                {
                    return Enums.ActionType.Modified;
                }
                return Enums.ActionType.Created;
            }
            if (docStatus == Enums.DocumentStatus.WaitingForApproval)
            {
                return Enums.ActionType.Submit;
            }

            if (docStatus == Enums.DocumentStatus.WaitingForApprovalManager)
            {
                return Enums.ActionType.Approve;
            }

            if (docStatus == Enums.DocumentStatus.WaitingGovApproval)
            {
                return Enums.ActionType.Approve;
            }
            if (docStatus == Enums.DocumentStatus.GovApproved)
            {
                return Enums.ActionType.GovPartialApprove;
            }
            if (docStatus == Enums.DocumentStatus.Completed)
            {
                return Enums.ActionType.GovApprove;
            }
            return Enums.ActionType.Reject;
        }

        private string GetActionBy(Ck4CDto ck4c)
        {
            if (ck4c.Status == Enums.DocumentStatus.Draft)
            {
                if (ck4c.ModifiedBy != null)
                {
                    return ck4c.ModifiedBy;
                }
                return ck4c.CreatedBy;
            }
            if (ck4c.Status == Enums.DocumentStatus.WaitingForApproval)
            {
                return ck4c.CreatedBy;
            }
            if (ck4c.Status == Enums.DocumentStatus.WaitingForApprovalManager)
            {
                return ck4c.ApprovedByManager;
            }
            if (ck4c.Status == Enums.DocumentStatus.WaitingGovApproval)
            {
                return ck4c.ApprovedByManager;
            }

            return ck4c.CreatedBy;
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
                case Enums.ActionType.Submit:
                    SubmitDocument(input);
                    break;
                case Enums.ActionType.Approve:
                    ApproveDocument(input);
                    break;
                case Enums.ActionType.Reject:
                    RejectDocument(input);
                    break;
            }

            //todo sent mail
            if (isNeedSendNotif)
                //SendEmailWorkflow(input);
            _uow.SaveChanges();
        }

        private void SubmitDocument(Ck4cWorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

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
            var nppbkcId = plant == null ? dbData.NPPBKC_ID : plant.NPPBKC_ID;

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
                dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                dbData.APPROVED_BY_POA = input.UserId;
                dbData.APPROVED_DATE_POA = DateTime.Now;
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
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Draft);

            //change back to draft
            dbData.STATUS = Enums.DocumentStatus.Draft;

            //todo ask
            dbData.APPROVED_BY_POA = null;
            dbData.APPROVED_DATE_POA = null;

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }
    }
}
