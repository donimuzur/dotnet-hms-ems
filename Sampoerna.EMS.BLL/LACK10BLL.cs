using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
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
    public class LACK10BLL : ILACK10BLL
    {
        private IGenericRepository<LACK10> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IPOABLL _poaBll;
        private IUserBLL _userBll;
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private ILFA1BLL _lfaBll;

        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IMessageService _messageService;
        private IWorkflowBLL _workflowBll;
        private IMonthBLL _monthBll;
        private IPoaDelegationServices _poaDelegationServices;
        private IGenericRepository<ZAIDM_EX_BRAND> _repositoryBrand;
        private IGenericRepository<WASTE> _repositoryWaste;
        private IGenericRepository<T001W> _repositoryPlant;
        private ILACK10ItemBLL _lack10ItemBll;
        private ILACK10DecreeDocBLL _lack10DecreeDocBll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IPlantBLL _plantbll;

        private string includeTables = "MONTH, LACK10_ITEM, LACK10_DECREE_DOC";

        public LACK10BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK10>();
            _repositoryBrand = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryWaste = _uow.GetGenericRepository<WASTE>();
            _repositoryPlant = _uow.GetGenericRepository<T001W>();

            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _messageService = new MessageService(_logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow,_logger);
            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
            _poaDelegationServices = new PoaDelegationServices(_uow, _logger);
            _lack10ItemBll = new LACK10ItemBLL(_uow, _logger);
            _lack10DecreeDocBll = new LACK10DecreeDocBLL(_uow, _logger);
            _lfaBll = new LFA1BLL(_uow, _logger);
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, _logger);
            _plantbll = new PlantBLL(_uow, _logger);
        }

        public List<Lack10Dto> GetByParam(Lack10GetByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            if (input.UserRole != Enums.UserRole.Controller && input.UserRole != Enums.UserRole.Administrator)
            {
                //delegate 
                var delegateUser = _poaDelegationServices.GetPoaDelegationFromByPoaToAndDate(input.UserId, DateTime.Now);

                if (input.UserRole == Enums.UserRole.POA)
                {
                    //delegate
                    if (delegateUser.Count > 0)
                    {
                        delegateUser.Add(input.UserId);
                        queryFilter = queryFilter.And(c => (delegateUser.Contains(c.CREATED_BY) ||
                                    (input.ListNppbkc.Contains(c.NPPBKC_ID))));
                    }
                    else
                    {
                        queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId ||
                            (input.ListNppbkc.Contains(c.NPPBKC_ID))));
                    }
                }
                else
                {
                    queryFilter = queryFilter.And(c => input.ListUserPlant.Contains(c.PLANT_ID) ||
                                                        (input.ListNppbkc.Contains(c.NPPBKC_ID) && string.IsNullOrEmpty(c.PLANT_ID)));
                }
            }

            return Mapper.Map<List<Lack10Dto>>(GetLack10Data(queryFilter, input.ShortOrderColumn));
        }

        private Expression<Func<LACK10, bool>> ProcessQueryFilter(Lack10GetByParamInput input)
        {
            Expression<Func<LACK10, bool>> queryFilter = PredicateHelper.True<LACK10>();

            if (!string.IsNullOrEmpty(input.CompanyId))
            {
                queryFilter = queryFilter.And(c => c.COMPANY_ID == input.CompanyId);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);
            }
            if (input.Month > 0)
            {
                queryFilter = queryFilter.And(c => c.PERIOD_MONTH == input.Month);
            }
            if (input.Year > 0)
            {
                queryFilter = queryFilter.And(c => c.PERIOD_YEAR == input.Year);
            }

            if (input.IsOpenDocument)
            {
                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);
            }
            else
            {
                queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);
            }

            return queryFilter;
        }

        private List<LACK10> GetLack10Data(Expression<Func<LACK10, bool>> queryFilter, string orderColumn)
        {
            Func<IQueryable<LACK10>, IOrderedQueryable<LACK10>> orderBy = null;
            if (!string.IsNullOrEmpty(orderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK10>(orderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);

            return dbData.ToList();
        }


        public List<Lack10Item> GenerateWasteData(Lack10GetWasteDataInput input)
        {
            //get Hasil Tembakau
            var dbData = from w in _repositoryWaste.Get(w => w.COMPANY_CODE == input.CompanyId && w.WERKS == input.PlantId && w.WASTE_PROD_DATE.Month == input.Month && w.WASTE_PROD_DATE.Year == input.Year && w.USE_FOR_LACK10 == true)
                         join b in _repositoryBrand.Get(b => b.STATUS == true && (b.IS_DELETED == null || b.IS_DELETED == false)) on new { w.FA_CODE, w.WERKS } equals new { b.FA_CODE, b.WERKS }
                         join t in _repositoryPlant.GetQuery() on w.WERKS equals t.WERKS
                         select new Lack10Item()
                         {
                             FaCode = w.FA_CODE,
                             BrandDescription = b.BRAND_CE,
                             Werks = w.WERKS,
                             PlantName = t.NAME1,
                             Type = "Hasil Tembakau",
                             Uom = "Btg",
                             WasteValue = w.MARKER_REJECT_STICK_QTY.Value + w.PACKER_REJECT_STICK_QTY.Value
                         };

            //get TIS
            var dbDataTis = from w in _repositoryWaste.Get(w => w.COMPANY_CODE == input.CompanyId && w.WERKS == input.PlantId && w.WASTE_PROD_DATE.Month == input.Month && w.WASTE_PROD_DATE.Year == input.Year && w.USE_FOR_LACK10 == true)
                         join b in _repositoryBrand.Get(b => b.STATUS == true && (b.IS_DELETED == null || b.IS_DELETED == false)) on new { w.FA_CODE, w.WERKS } equals new { b.FA_CODE, b.WERKS }
                         join t in _repositoryPlant.GetQuery() on w.WERKS equals t.WERKS
                         select new Lack10Item()
                         {
                             FaCode = w.FA_CODE,
                             BrandDescription = b.BRAND_CE,
                             Werks = w.WERKS,
                             PlantName = t.NAME1,
                             Type = "TIS",
                             Uom = "Kg",
                             WasteValue = w.FLOOR_WASTE_GRAM_QTY.Value + w.DUST_WASTE_GRAM_QTY.Value + +w.STAMP_WASTE_QTY.Value
                         };

            if (input.NppbkcId != string.Empty && input.IsNppbkc)
            {
                dbData = from w in _repositoryWaste.Get(w => w.COMPANY_CODE == input.CompanyId && w.WASTE_PROD_DATE.Month == input.Month && w.WASTE_PROD_DATE.Year == input.Year && w.USE_FOR_LACK10 == true)
                         join b in _repositoryBrand.Get(b => b.STATUS == true && (b.IS_DELETED == null || b.IS_DELETED == false)) on new { w.FA_CODE, w.WERKS } equals new { b.FA_CODE, b.WERKS }
                         join n in _repositoryPlant.Get(n => n.NPPBKC_ID == input.NppbkcId) on w.WERKS equals n.WERKS
                         select new Lack10Item()
                         {
                             FaCode = w.FA_CODE,
                             BrandDescription = b.BRAND_CE,
                             Werks = w.WERKS,
                             PlantName = n.NAME1,
                             Type = "Hasil Tembakau",
                             Uom = "Btg",
                             WasteValue = w.MARKER_REJECT_STICK_QTY.Value + w.PACKER_REJECT_STICK_QTY.Value
                         };

                dbDataTis = from w in _repositoryWaste.Get(w => w.COMPANY_CODE == input.CompanyId && w.WASTE_PROD_DATE.Month == input.Month && w.WASTE_PROD_DATE.Year == input.Year && w.USE_FOR_LACK10 == true)
                            join b in _repositoryBrand.Get(b => b.STATUS == true && (b.IS_DELETED == null || b.IS_DELETED == false)) on new { w.FA_CODE, w.WERKS } equals new { b.FA_CODE, b.WERKS }
                            join n in _repositoryPlant.Get(n => n.NPPBKC_ID == input.NppbkcId) on w.WERKS equals n.WERKS
                            select new Lack10Item()
                            {
                                FaCode = w.FA_CODE,
                                BrandDescription = b.BRAND_CE,
                                Werks = w.WERKS,
                                PlantName = n.NAME1,
                                Type = "TIS",
                                Uom = "Kg",
                                WasteValue = w.FLOOR_WASTE_GRAM_QTY.Value + w.DUST_WASTE_GRAM_QTY.Value + +w.STAMP_WASTE_QTY.Value
                            };
            }

            var groupList = dbData
                .GroupBy(x => new { x.Werks, x.FaCode })
                .Select(p => new Lack10Item()
                {
                    FaCode = p.FirstOrDefault().FaCode,
                    BrandDescription = p.FirstOrDefault().BrandDescription,
                    Werks = p.FirstOrDefault().Werks,
                    PlantName = p.FirstOrDefault().PlantName,
                    Type = p.FirstOrDefault().Type,
                    Uom = p.FirstOrDefault().Uom,
                    WasteValue = p.Sum(c => c.WasteValue)
                });

            var groupListTis = dbDataTis
                .GroupBy(x => new { x.Werks, x.FaCode })
                .Select(p => new Lack10Item()
                {
                    FaCode = p.FirstOrDefault().FaCode,
                    BrandDescription = p.FirstOrDefault().BrandDescription,
                    Werks = p.FirstOrDefault().Werks,
                    PlantName = p.FirstOrDefault().PlantName,
                    Type = p.FirstOrDefault().Type,
                    Uom = p.FirstOrDefault().Uom,
                    WasteValue = p.Sum(c => c.WasteValue) / 1000
                });

            var allData = groupList.ToList();
            allData.AddRange(groupListTis);

            return allData.OrderBy(x => x.Werks).OrderBy(x => x.FaCode).ToList();
        }


        public Lack10Dto GetByItem(Lack10Dto item)
        {
            var dbData = _repository.Get(c => c.PLANT_ID == item.PlantId && c.NPPBKC_ID == item.NppbkcId
                                            && c.PERIOD_MONTH == item.PeriodMonth
                                            && c.PERIOD_YEAR == item.PeriodYears, null, includeTables).FirstOrDefault();

            var mapResult = Mapper.Map<Lack10Dto>(dbData);

            return mapResult;
        }


        public Lack10Dto Save(Lack10Dto item, string userId)
        {
            LACK10 model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                bool changed = false;

                if (item.Lack10Id > 0)
                {
                    //update
                    model = _repository.Get(c => c.LACK10_ID == item.Lack10Id).FirstOrDefault();

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    changed = SetChangesHistory(model, item, userId);

                    _lack10ItemBll.DeleteByLack10Id(item.Lack10Id);

                    Mapper.Map<Lack10Dto, LACK10>(item, model);
                    model.LACK10_ITEM = null;

                    model.LACK10_ITEM = Mapper.Map<List<LACK10_ITEM>>(item.Lack10Item);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = item.PeriodMonth.Value;
                    inputDoc.Year = item.PeriodYears.Value;
                    inputDoc.NppbkcId = item.NppbkcId;

                    item.Lack10Number = _docSeqNumBll.GenerateNumber(inputDoc);

                    model = Mapper.Map<LACK10>(item);
                    _repository.InsertOrUpdate(model);
                }

                _uow.SaveChanges();

                //set workflow history
                var getUserRole = _poaBll.GetUserRole(userId);
                var input = new Lack10WorkflowDocumentInput()
                {
                    DocumentId = model.LACK10_ID,
                    DocumentNumber = model.LACK10_NUMBER,
                    ActionType = Enums.ActionType.Modified,
                    UserId = userId,
                    UserRole = getUserRole
                };

                //delegate user
                input.Comment = _poaDelegationServices.CommentDelegatedUserSaveOrSubmit(model.CREATED_BY,
                    input.UserId, DateTime.Now);

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

            return Mapper.Map<Lack10Dto>(model);
        }

        private bool SetChangesHistory(LACK10 origin, Lack10Dto data, string userId)
        {
            var changed = false;

            var changeData = new Dictionary<string, bool>();
            changeData.Add("COMPANY_CODE", origin.COMPANY_ID == data.CompanyId);
            changeData.Add("PLANT", origin.PLANT_ID == data.PlantId);
            changeData.Add("NPPBKC", origin.NPPBKC_ID == data.NppbkcId);
            changeData.Add("SUBMISSION_DATE", origin.SUBMISSION_DATE == data.SubmissionDate);
            changeData.Add("PERIOD_MONTH", origin.PERIOD_MONTH == data.PeriodMonth);
            changeData.Add("PERIOD_YEAR", origin.PERIOD_YEAR == data.PeriodYears);
            changeData.Add("REPORT_TYPE", origin.REPORT_TYPE == data.ReportType);
            changeData.Add("REASON", origin.REASON == data.Reason);
            changeData.Add("REMARK", origin.REMARK == data.Remark);

            foreach (var listChange in changeData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Enums.MenuList.LACK10,
                        FORM_ID = data.Lack10Id.ToString(),
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
                        case "SUBMISSION_DATE":
                            changes.OLD_VALUE = origin.SUBMISSION_DATE.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = data.SubmissionDate.Value.ToString("dd MMM yyyy");
                            changes.FIELD_NAME = "Submission Date";
                            break;
                        case "PERIOD_MONTH":
                            changes.OLD_VALUE = origin.MONTH.MONTH_NAME_IND;
                            changes.NEW_VALUE = data.MonthNameIndo;
                            changes.FIELD_NAME = "Period Month";
                            break;
                        case "PERIOD_YEAR":
                            changes.OLD_VALUE = origin.PERIOD_YEAR.ToString();
                            changes.NEW_VALUE = data.PeriodYears.ToString();
                            changes.FIELD_NAME = "Period Year";
                            break;
                        case "REPORT_TYPE":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.REPORT_TYPE);
                            changes.NEW_VALUE = EnumHelper.GetDescription(data.ReportType);
                            changes.FIELD_NAME = "Report Type";
                            break;
                        case "REASON":
                            changes.OLD_VALUE = origin.REASON;
                            changes.NEW_VALUE = data.Reason;
                            changes.FIELD_NAME = "Reason";
                            break;
                        case "REMARK":
                            changes.OLD_VALUE = origin.REMARK;
                            changes.NEW_VALUE = data.Remark;
                            changes.FIELD_NAME = "Remark";
                            break;
                        default: break;
                    }
                    _changesHistoryBll.AddHistory(changes);

                    changed = true;
                }
            }

            return changed;
        }

        private void AddWorkflowHistory(Lack10WorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.FORM_TYPE_ID = Enums.FormType.LACK10;

            _workflowHistoryBll.Save(dbData);

        }


        public void Lack10Workflow(Lack10WorkflowDocumentInput input)
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

        private void CreateDocument(Lack10WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.LACK10_NUMBER ;

            AddWorkflowHistory(input);
        }

        private void SubmitDocument(Lack10WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.Draft && dbData.STATUS != Enums.DocumentStatus.Rejected)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingForApproval);

            switch (input.UserRole)
            {
                case Enums.UserRole.User:
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;
                    break;
                case Enums.UserRole.POA:
                    //first code when manager exists
                    //dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;
                    break;
                default:
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }

            input.DocumentNumber = dbData.LACK10_NUMBER;

            //delegate
            input.Comment = _poaDelegationServices.CommentDelegatedUserSaveOrSubmit(dbData.CREATED_BY, input.UserId,
                DateTime.Now);

            AddWorkflowHistory(input);

        }

        private void WorkflowStatusAddChanges(Lack10WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            try
            {
                //set changes log
                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Enums.MenuList.LACK10,
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

        private void ApproveDocument(Lack10WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var nppbkcId = dbData.NPPBKC_ID;

            var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
            {
                CreatedUser = dbData.CREATED_BY,
                CurrentUser = input.UserId,
                DocumentStatus = dbData.STATUS,
                UserRole = input.UserRole,
                NppbkcId = nppbkcId,
                DocumentNumber = dbData.LACK10_NUMBER
            });

            if (!isOperationAllow)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            if (input.UserRole == Enums.UserRole.POA)
            {
                if (dbData.STATUS == Enums.DocumentStatus.WaitingForApproval)
                {
                    WorkflowPoaChanges(input, dbData.APPROVED_BY, input.UserId);
                    WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingForApprovalController);

                    //first code when manager exists
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalController;
                    //dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                    dbData.APPROVED_BY = input.UserId;
                    dbData.APPROVED_DATE = DateTime.Now;
                }
                else
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
                }
            }
            else
            {
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);

                dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                dbData.APPROVED_BY_MANAGER = input.UserId;
                dbData.APPROVED_BY_MANAGER_DATE = DateTime.Now;
            }

            input.DocumentNumber = dbData.LACK10_NUMBER;

            //delegate
            input.Comment = CommentDelegateUser(dbData, input);
            //end delegate

            AddWorkflowHistory(input);

        }

        private void WorkflowPoaChanges(Lack10WorkflowDocumentInput input, string oldPoa, string newPoa)
        {
            try
            {
                //set changes log
                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Enums.MenuList.LACK10,
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

        private string CommentDelegateUser(LACK10 dbData, Lack10WorkflowDocumentInput input)
        {
            string comment = "";

            var inputHistory = new GetByFormTypeAndFormIdInput();
            inputHistory.FormId = dbData.LACK10_ID;
            inputHistory.FormType = Enums.FormType.LACK10;

            var rejectedPoa = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(inputHistory);
            if (rejectedPoa != null)
            {
                comment = _poaDelegationServices.CommentDelegatedByHistory(rejectedPoa.COMMENT,
                    rejectedPoa.ACTION_BY, input.UserId, input.UserRole, dbData.CREATED_BY, DateTime.Now);
            }
            else
            {
                var isPoaCreatedUser = _poaBll.GetActivePoaById(dbData.CREATED_BY);
                List<string> listPoa;
                if (isPoaCreatedUser != null) //if creator = poa
                {
                    listPoa = _poaBll.GetPoaActiveByNppbkcId(dbData.NPPBKC_ID).Select(c => c.POA_ID).ToList();
                }
                else
                {
                    listPoa = _poaBll.GetPoaActiveByPlantId(dbData.PLANT_ID).Select(c => c.POA_ID).ToList();
                }

                comment = _poaDelegationServices.CommentDelegatedUserApproval(listPoa, input.UserId, DateTime.Now);

            }

            return comment;
        }

        private void RejectDocument(Lack10WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //first code when manager exists
            if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalController &&
                dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);

            dbData.STATUS = Enums.DocumentStatus.Rejected;

            input.DocumentNumber = dbData.LACK10_NUMBER;

            //delegate
            string commentReject = CommentDelegateUser(dbData, input);

            if (!string.IsNullOrEmpty(commentReject))
                input.Comment += " [" + commentReject + "]";
            //end delegate

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(Lack10WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //delete data doc first
            _lack10DecreeDocBll.DeleteByLack10Id(dbData.LACK10_ID);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
            WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGovType2.Approved);

            dbData.STATUS = Enums.DocumentStatus.Completed;

            //todo: update remaining quota and necessary data
            dbData.LACK10_DECREE_DOC = null;
            dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
            dbData.LACK10_DECREE_DOC = Mapper.Map<List<LACK10_DECREE_DOC>>(input.AdditionalDocumentData.Lack10DecreeDoc);
            dbData.GOV_STATUS = Enums.DocumentStatusGovType2.Approved;
            dbData.MODIFIED_DATE = DateTime.Now;

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.LACK10_NUMBER;

            //delegate
            if (dbData.CREATED_BY != input.UserId)
            {
                if (input.UserRole != Enums.UserRole.Administrator)
                {
                    var workflowHistoryDto =
                        _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                    input.Comment = _poaDelegationServices.CommentDelegatedByHistory(workflowHistoryDto.COMMENT,
                        workflowHistoryDto.ACTION_BY, input.UserId, input.UserRole, dbData.CREATED_BY, DateTime.Now);
                }
            }
            //end delegate

            AddWorkflowHistory(input);

        }

        private void WorkflowStatusGovAddChanges(Lack10WorkflowDocumentInput input, Enums.DocumentStatusGovType2? oldStatus, Enums.DocumentStatusGovType2 newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK10,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "STATUS_GOV",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void GovRejectedDocument(Lack10WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //delete data doc first
            _lack10DecreeDocBll.DeleteByLack10Id(dbData.LACK10_ID);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.GovRejected);
            WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGovType2.Rejected);

            dbData.STATUS = Enums.DocumentStatus.GovRejected;
            dbData.GOV_STATUS = Enums.DocumentStatusGovType2.Rejected;

            input.DocumentNumber = dbData.LACK10_NUMBER;

            //delegate
            if (dbData.CREATED_BY != input.UserId)
            {
                var workflowHistoryDto =
                    _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                var commentReject = _poaDelegationServices.CommentDelegatedByHistory(workflowHistoryDto.COMMENT,
                    workflowHistoryDto.ACTION_BY, input.UserId, input.UserRole, dbData.CREATED_BY, DateTime.Now);

                if (!string.IsNullOrEmpty(commentReject))
                    input.Comment += " [" + commentReject + "]";

            }
            //end delegate

            AddWorkflowHistory(input);

        }

        private void EditCompletedDocument(Lack10WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.Completed)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            
            _lack10DecreeDocBll.DeleteByLack10Id(dbData.LACK10_ID);

            dbData.LACK10_DECREE_DOC = null;
            dbData.LACK10_DECREE_DOC = Mapper.Map<List<LACK10_DECREE_DOC>>(input.AdditionalDocumentData.Lack10DecreeDoc);

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.LACK10_NUMBER;

            //delegate
            input.Comment = _poaDelegationServices.CommentDelegatedUserSaveOrSubmit(dbData.CREATED_BY, input.UserId,
                DateTime.Now);

            AddWorkflowHistory(input);

        }

        private void SendEmailWorkflow(Lack10WorkflowDocumentInput input)
        {
            var lack10Data = Mapper.Map<Lack10Dto>(_repository.Get(c => c.LACK10_ID == input.DocumentId, null, includeTables).FirstOrDefault());

            var mailProcess = ProsesMailNotificationBody(lack10Data, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private Lack10MailNotification ProsesMailNotificationBody(Lack10Dto lack10Data, Lack10WorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new Lack10MailNotification();
            var plant = _repositoryPlant.GetQuery(x => x.WERKS == lack10Data.PlantId).FirstOrDefault();
            var nppbkc = lack10Data.NppbkcId;
            var firstText = input.ActionType == Enums.ActionType.Reject ? " Document" : string.Empty;
            //var approveRejectedPoa = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(lack10Data.Number);
            var approveRejectedPoa = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(new GetByFormTypeAndFormIdInput() { FormId = lack10Data.Lack10Id, FormType = Enums.FormType.LACK10 });

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var lack10Type = "Plant";
            if (plant == null) lack10Type = "NPPBKC";

            var userData = _userBll.GetUserById(lack10Data.CreatedBy);
            var controllerList = _userBll.GetControllers();

            rc.Subject = "LACK-10 " + lack10Data.Lack10Number + " is " + EnumHelper.GetDescription(lack10Data.Status);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, LACK-10" + firstText + " is " + EnumHelper.GetDescription(lack10Data.Status) + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Creator </td><td>: " + userData.LAST_NAME + ", " + userData.FIRST_NAME + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Company Code </td><td>: " + lack10Data.CompanyId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Plant ID </td><td>: " + lack10Data.PlantId + " - " + lack10Data.PlantName + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Period </td><td>: " + _monthBll.GetMonth(lack10Data.PeriodMonth.Value).MONTH_NAME_ENG + " " + lack10Data.PeriodYears + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>KPPBC </td><td>: " + _lfaBll.GetById(_nppbkcbll.GetById(nppbkc).KPPBC_ID).NAME2 + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + nppbkc + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + lack10Data.Lack10Number + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : LACK-10 level " + lack10Type + "</td></tr>");
            bodyMail.AppendLine();
            if (input.ActionType == Enums.ActionType.Reject || input.ActionType == Enums.ActionType.GovReject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("<tr colspan='2'><td><i>To VIEW, Please click this <a href='" + webRootUrl + "/LACK10/Detail/" + lack10Data.Lack10Id + "'>link</a></i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr colspan='2'><td><i>To APPROVE, Please click this <a href='" + webRootUrl + "/LACK10/Edit/" + lack10Data.Lack10Id + "'>link</a></i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");

            var poaList = new List<POADto>();

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (lack10Data.Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (approveRejectedPoa != null)
                        {
                            var poaApproveId = _userBll.GetUserById(approveRejectedPoa.ACTION_BY);

                            rc.To.Add(poaApproveId.EMAIL);
                        }
                        else
                        {
                            var creatorPoa = _poaBll.GetById(lack10Data.CreatedBy);



                            if (creatorPoa != null)
                            {
                                poaList = _poaBll.GetPoaActiveByNppbkcId(nppbkc)
                                           .Where(x => x.POA_ID != lack10Data.CreatedBy).ToList();
                            }
                            else
                            {
                                poaList = plant != null ? _poaBll.GetPoaActiveByPlantId(lack10Data.PlantId) :
                                                        _poaBll.GetPoaActiveByNppbkcId(nppbkc);
                            }

                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(userData.EMAIL);
                    }
                    //else if (lack10Data.Status == Enums.DocumentStatus.WaitingForApprovalController)
                    //{
                    //    foreach (var item in controllerList)
                    //    {
                    //        rc.To.Add(item.EMAIL);
                    //    }

                    //    rc.CC.Add(userData.EMAIL);

                    //    if (plant != null) poaList = _poabll.GetPoaActiveByPlantId(lack10Data.PlantId);
                    //    foreach (var poaDto in poaList)
                    //    {
                    //        if (userData.USER_ID != poaDto.POA_ID)
                    //            rc.CC.Add(poaDto.POA_EMAIL);
                    //    }
                    //}
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    if (lack10Data.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(lack10Data.CreatedBy);
                        var poaApproved = _userBll.GetUserById(lack10Data.ApprovedBy);
                        if (poaData != null)
                        {
                            rc.To.Add(poaApproved.EMAIL);
                            //creator is poa user
                            rc.CC.Add(poaData.POA_EMAIL);
                            //first code when manager exists
                            //rc.CC.Add(GetManagerEmail(lack10Data.CreatedBy));
                            foreach (var item in controllerList)
                            {
                                rc.CC.Add(item.EMAIL);
                            }
                        }
                        else
                        {
                            //creator is excise executive
                            rc.CC.Add(userData.EMAIL);
                            //first code when manager exists
                            //rc.CC.Add(GetManagerEmail(lack10Data.ApprovedByPoa));
                            rc.To.Add(poaApproved.EMAIL);
                            foreach (var item in controllerList)
                            {
                                rc.CC.Add(item.EMAIL);
                            }
                        }
                    }
                    //first code when manager exists
                    else if (lack10Data.Status == Enums.DocumentStatus.WaitingForApprovalController)
                    {
                        var poaUser = lack10Data.ApprovedBy == null ? lack10Data.CreatedBy : lack10Data.ApprovedBy;
                        var poaApproveId = _userBll.GetUserById(lack10Data.ApprovedBy);

                        foreach (var item in controllerList)
                        {
                            rc.To.Add(item.EMAIL);
                        }

                        rc.CC.Add(_userBll.GetUserById(lack10Data.CreatedBy).EMAIL);

                        if (poaApproveId != null)
                            rc.CC.Add(poaApproveId.EMAIL);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var poaApprove = approveRejectedPoa == null ? null : _userBll.GetUserById(approveRejectedPoa.ACTION_BY);
                    //var poaId = approveRejectedPoa == null ? lack10Data.CreatedBy : approveRejectedPoa.ACTION_BY;

                    rc.To.Add(userData.EMAIL);
                    if (poaApprove != null)
                        rc.CC.Add(poaApprove.EMAIL);
                    //first code when manager exists
                    //rc.CC.Add(managerMail);
                    foreach (var item in controllerList)
                    {
                        rc.CC.Add(item.EMAIL);
                    }

                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovApprove:
                    var poaData3 = _poaBll.GetActivePoaById(lack10Data.CreatedBy);
                    if (poaData3 != null)
                    {
                        //creator is poa user
                        rc.To.Add(_userBll.GetUserById(poaData3.POA_ID).EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lack10Data.CreatedBy));
                        foreach (var item in controllerList)
                        {
                            rc.CC.Add(item.EMAIL);
                        }
                    }
                    else
                    {
                        //creator is excise executive
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_userBll.GetUserById(lack10Data.ApprovedBy).EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lack10Data.ApprovedByPoa));
                        foreach (var item in controllerList)
                        {
                            rc.CC.Add(item.EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovReject:
                    var poaData5 = _poaBll.GetActivePoaById(lack10Data.CreatedBy);
                    if (poaData5 != null)
                    {
                        //creator is poa user
                        rc.To.Add(_userBll.GetUserById(poaData5.POA_ID).EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lack10Data.CreatedBy));
                        foreach (var item in controllerList)
                        {
                            rc.CC.Add(item.EMAIL);
                        }
                    }
                    else
                    {
                        //creator is excise executive
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_userBll.GetUserById(lack10Data.ApprovedBy).EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lack10Data.ApprovedBy));
                        foreach (var item in controllerList)
                        {
                            rc.CC.Add(item.EMAIL);
                        }

                    }
                    rc.IsCCExist = true;
                    break;
            }
            //delegate

            var inputDelegate = new GetEmailDelegateUserInput();
            inputDelegate.FormType = Enums.FormType.LACK10;
            inputDelegate.FormId = lack10Data.Lack10Id;
            inputDelegate.FormNumber = lack10Data.Lack10Number;
            inputDelegate.ActionType = input.ActionType;

            inputDelegate.CurrentUser = input.UserId;
            inputDelegate.CreatedUser = lack10Data.CreatedBy;
            inputDelegate.Date = DateTime.Now;

            inputDelegate.WorkflowHistoryDto = approveRejectedPoa;
            inputDelegate.UserApprovedPoa = poaList.Select(c => c.POA_ID).ToList();
            string emailResult = "";
            emailResult = _poaDelegationServices.GetEmailDelegateOrOriginalUserByAction(inputDelegate);

            if (!string.IsNullOrEmpty(emailResult))
            {
                rc.IsCCExist = true;
                rc.CC.Add(emailResult);
            }
            //end delegate

            rc.Body = bodyMail.ToString();
            return rc;
        }

        private class Lack10MailNotification
        {
            public Lack10MailNotification()
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


        public Lack10Dto GetById(long id)
        {
            var dbData = _repository.Get(c => c.LACK10_ID == id, null, includeTables).FirstOrDefault();

            var mapResult = Mapper.Map<Lack10Dto>(dbData);

            return mapResult;
        }


        public bool AllowEditCompletedDocument(Lack10Dto item, string userId)
        {
            var isAllow = false;

            if (item.CreatedBy == userId || item.ApprovedBy == userId)
                isAllow = true;

            return isAllow;
        }


        public void UpdateSubmissionDate(Lack10UpdateSubmissionDate input)
        {
            LACK10 dbData = _repository.Get(c => c.LACK10_ID == input.Id, null, includeTables).FirstOrDefault();
            dbData.SUBMISSION_DATE = input.SubmissionDate.Value;

            if (input.DecreeDate.HasValue)
            {
                dbData.DECREE_DATE = input.DecreeDate;
            }

            _uow.SaveChanges();
        }


        public Lack10ExportDto GetLack10ExportById(int id)
        {
            var rc = _repository.Get(c => c.LACK10_ID == id, null, includeTables).FirstOrDefault();

            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var data = Mapper.Map<Lack10ExportDto>(rc);

            data.CompanyAddress = _plantbll.GetMainPlantByNppbkcId(rc.NPPBKC_ID).ADDRESS;

            //get address of plant
            if (!string.IsNullOrEmpty(rc.PLANT_ID))
            {
                data.CompanyAddress = _repositoryPlant.GetQuery(x => x.WERKS == rc.PLANT_ID).FirstOrDefault().ADDRESS;
            }

            return data;
        }


        public List<Lack10SummaryReportDto> GetSummaryReportsByParam(Lack10GetSummaryReportByParamInput input)
        {
            Expression<Func<LACK10, bool>> queryFilter = PredicateHelper.True<LACK10>();

            if (input.UserRole == Enums.UserRole.POA)
            {
                queryFilter = queryFilter.And(c => input.ListNppbkc.Contains(c.NPPBKC_ID));
            }
            else if (input.UserRole == Enums.UserRole.Administrator)
            {
                queryFilter = queryFilter.And(c => c.COMPANY_ID != null);
            }
            else
            {
                queryFilter = queryFilter.And(c => input.ListUserPlant.Contains(c.PLANT_ID) ||
                                                    (input.ListNppbkc.Contains(c.NPPBKC_ID) && string.IsNullOrEmpty(c.PLANT_ID)));
            }

            if (!string.IsNullOrEmpty(input.Lack10No))
            {
                queryFilter = queryFilter.And(c => c.LACK10_NUMBER == input.Lack10No);
            }


            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);
            }

            if (!string.IsNullOrEmpty(input.Poa))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }

            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (input.Month > 0)
            {
                queryFilter = queryFilter.And(c => c.PERIOD_MONTH == input.Month);
            }
            if (input.Year > 0)
            {
                queryFilter = queryFilter.And(c => c.PERIOD_YEAR == input.Year);
            }

            var rc = _repository.Get(queryFilter, null, includeTables).ToList();
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var data = SetDataSummaryReport(rc);

            if (input.isForExport)
                data = SetDataSummaryForExport(rc);

            data = data.OrderBy(x => x.LicenseNumber).ToList();

            return data;
        }

        private List<Lack10SummaryReportDto> SetDataSummaryReport(List<LACK10> listLack10)
        {
            var result = new List<Lack10SummaryReportDto>();

            foreach (var dtData in listLack10)
            {
                var summaryDto = new Lack10SummaryReportDto();

                summaryDto.Lack10No = dtData.LACK10_NUMBER;
                summaryDto.CeOffice = dtData.COMPANY_ID;
                summaryDto.BasedOn = dtData.PLANT_ID == null ? "NPPBKC" : "PLANT";
                summaryDto.LicenseNumber = dtData.NPPBKC_ID;
                summaryDto.Kppbc = _lfaBll.GetById(_nppbkcbll.GetDetailsById(dtData.NPPBKC_ID).KPPBC_ID).NAME1;
                summaryDto.SubmissionDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.SUBMISSION_DATE);
                summaryDto.Month = dtData.MONTH.MONTH_NAME_ENG;
                summaryDto.Year = dtData.PERIOD_YEAR.ToString();
                summaryDto.PoaApproved = dtData.APPROVED_BY == null ? "-" : dtData.APPROVED_BY;
                summaryDto.Status = EnumHelper.GetDescription(dtData.STATUS);
                summaryDto.CompletedDate = dtData.STATUS == Enums.DocumentStatus.Completed ?
                    (dtData.MODIFIED_DATE == null ? ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.DECREE_DATE) :
                    ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.MODIFIED_DATE)) : "-";
                summaryDto.Creator = dtData.CREATED_BY;

                var faCode = new List<string>();
                var brandDesc = new List<string>();
                var werks = new List<string>();
                var plantName = new List<string>();
                var type = new List<string>();
                var wasteValue = new List<string>();
                var uom = new List<string>();

                foreach (var lack10Item in dtData.LACK10_ITEM)
                {
                    faCode.Add(lack10Item.FA_CODE);
                    brandDesc.Add(lack10Item.BRAND_DESCRIPTION);
                    werks.Add(lack10Item.WERKS);
                    plantName.Add(lack10Item.PLANT_NAME);
                    type.Add(lack10Item.TYPE);
                    wasteValue.Add(String.Format("{0:n}", lack10Item.WASTE_VALUE));
                    uom.Add(lack10Item.UOM);
                }

                summaryDto.FaCode = faCode;
                summaryDto.BrandDesc = brandDesc;
                summaryDto.Werks = werks;
                summaryDto.PlantName = plantName;
                summaryDto.Type = type;
                summaryDto.WasteValue = wasteValue;
                summaryDto.Uom = uom;

                result.Add(summaryDto);
            }

            return result;
        }

        private List<Lack10SummaryReportDto> SetDataSummaryForExport(List<LACK10> listLack10)
        {
            var result = new List<Lack10SummaryReportDto>();

            foreach (var dtData in listLack10)
            {
                foreach (var lack10Item in dtData.LACK10_ITEM)
                {
                    var summaryDto = new Lack10SummaryReportDto();

                    summaryDto.Lack10No = dtData.LACK10_NUMBER;
                    summaryDto.CeOffice = dtData.COMPANY_ID;
                    summaryDto.BasedOn = dtData.PLANT_ID == null ? "NPPBKC" : "PLANT";
                    summaryDto.LicenseNumber = dtData.NPPBKC_ID;
                    summaryDto.Kppbc = _lfaBll.GetById(_nppbkcbll.GetDetailsById(dtData.NPPBKC_ID).KPPBC_ID).NAME1;
                    summaryDto.SubmissionDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.SUBMISSION_DATE);
                    summaryDto.Month = dtData.MONTH.MONTH_NAME_ENG;
                    summaryDto.Year = dtData.PERIOD_YEAR.ToString();
                    summaryDto.PoaApproved = dtData.APPROVED_BY == null ? "-" : dtData.APPROVED_BY;
                    summaryDto.Status = EnumHelper.GetDescription(dtData.STATUS);
                    summaryDto.CompletedDate = dtData.STATUS == Enums.DocumentStatus.Completed ?
                        (dtData.MODIFIED_DATE == null ? ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.DECREE_DATE) :
                        ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.MODIFIED_DATE)) : "-";
                    summaryDto.Creator = dtData.CREATED_BY;

                    var faCode = new List<string>();
                    var brandDesc = new List<string>();
                    var werks = new List<string>();
                    var plantName = new List<string>();
                    var type = new List<string>();
                    var wasteValue = new List<string>();
                    var uom = new List<string>();

                    faCode.Add(lack10Item.FA_CODE);
                    brandDesc.Add(lack10Item.BRAND_DESCRIPTION);
                    werks.Add(lack10Item.WERKS);
                    plantName.Add(lack10Item.PLANT_NAME);
                    type.Add(lack10Item.TYPE);
                    wasteValue.Add(String.Format("{0:n}", lack10Item.WASTE_VALUE));
                    uom.Add(lack10Item.UOM);

                    summaryDto.FaCode = faCode;
                    summaryDto.BrandDesc = brandDesc;
                    summaryDto.Werks = werks;
                    summaryDto.PlantName = plantName;
                    summaryDto.Type = type;
                    summaryDto.WasteValue = wasteValue;
                    summaryDto.Uom = uom;

                    result.Add(summaryDto);
                }
            }

            return result;
        }
    }
}
