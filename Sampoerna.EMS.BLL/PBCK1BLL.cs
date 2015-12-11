﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.MessagingService;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

using Sampoerna.EMS.LinqExtensions;

namespace Sampoerna.EMS.BLL
{
    public class PBCK1BLL : IPBCK1BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<PBCK1> _repository;
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IMonthBLL _monthBll;
        private IPbck1ProdConverterBLL _prodConverterBll;
        private IPbck1ProdPlanBLL _prodPlanBll;
        private IPbck1DecreeDocBLL _decreeDocBll;
        private IPOABLL _poaBll;
        private IWorkflowBLL _workflowBll;
        private IMessageService _messageService;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IZaidmExKPPBCBLL _kppbcbll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IHeaderFooterBLL _headerFooterBll;
        private IUserBLL _userBll;
        private ILFA1BLL _lfaBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private ILACK1BLL _lack1Bll;
        private IT001KBLL _t001Kbll;
        private IPlantBLL _plantBll;


        private string includeTables = "UOM, UOM1, MONTH, MONTH1, USER, USER1, USER2";

        public PBCK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1>();
            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _prodTypeBll = new ZaidmExProdTypeBLL(_uow, _logger);
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _prodPlanBll = new Pbck1ProdPlanBLL(_uow, _logger);
            _prodConverterBll = new Pbck1ProdConverterBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
            _decreeDocBll = new Pbck1DecreeDocBLL(_uow, _logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _messageService = new MessageService(_logger);
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, _logger);
            _kppbcbll = new ZaidmExKPPBCBLL(_logger, _uow);
            _headerFooterBll = new HeaderFooterBLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
            _lfaBll = new LFA1BLL(_uow, _logger);
            _brandRegistrationBll = new BrandRegistrationBLL(_uow, _logger);
            _lack1Bll = new LACK1BLL(_uow, _logger);
            _t001Kbll = new T001KBLL(_uow, _logger);
            _plantBll = new PlantBLL(_uow, _logger);
            _goodTypeBll = new ZaidmExGoodTypeBLL(_uow, _logger);
        }

        public List<Pbck1Dto> GetAllByParam(Pbck1GetByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            return Mapper.Map<List<Pbck1Dto>>(GetPbck1Data(queryFilter, input.SortOrderColumn));
        }

        public List<Pbck1Dto> GetOpenDocumentByParam(Pbck1GetOpenDocumentByParamInput input)
        {

            var queryFilter = ProcessQueryFilter(input);
            if(input.UserRole == Enums.UserRole.POA){
                var nppbkc = _nppbkcbll.GetNppbkcMainPlantOnlyByPoa(input.UserId).Select(d => d.NPPBKC_ID).ToList();

                queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId || (c.STATUS != Enums.DocumentStatus.Draft && nppbkc.Contains(c.NPPBKC_ID))));
            }
            else if (input.UserRole == Enums.UserRole.Manager) {
                var poaList = _poaBll.GetPOAIdByManagerId(input.UserId);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.NUMBER));
            }else{
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.UserId);
            }

            queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);

            return Mapper.Map<List<Pbck1Dto>>(GetPbck1Data(queryFilter, input.SortOrderColumn));

        }

        public List<Pbck1Dto> GetCompletedDocumentByParam(Pbck1GetCompletedDocumentByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);

            return Mapper.Map<List<Pbck1Dto>>(GetPbck1Data(queryFilter, input.SortOrderColumn));
        }

        private Expression<Func<PBCK1, bool>> ProcessQueryFilter(Pbck1GetByParamInput input)
        {
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);
            }

            if (input.Pbck1Type.HasValue)
            {
                queryFilter = queryFilter.And(c => c.PBCK1_TYPE == input.Pbck1Type.Value);
            }

            if (!string.IsNullOrEmpty(input.Poa))
            {
                queryFilter = queryFilter.And(c => (!string.IsNullOrEmpty(c.APPROVED_BY_POA) && c.APPROVED_BY_POA == input.Poa) || c.CREATED_BY == input.Poa);
            }

            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }

            if (!string.IsNullOrEmpty(input.GoodTypeId))
            {
                queryFilter = queryFilter.And(c => c.EXC_GOOD_TYP == input.GoodTypeId);
            }

            if (input.Year.HasValue)
            {
                queryFilter = queryFilter.And(c => (c.PERIOD_FROM.HasValue && c.PERIOD_FROM.Value.Year == input.Year.Value)
                    || (c.PERIOD_TO.HasValue && c.PERIOD_TO.Value.Year == input.Year.Value));
            }

            if (!string.IsNullOrEmpty(input.UserId))
            {
                if (input.UserRole == Enums.UserRole.POA)
                {
                    var nppbkc = _nppbkcbll.GetNppbkcMainPlantOnlyByPoa(input.UserId).Select(d => d.NPPBKC_ID).ToList();

                    queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId || (c.STATUS != Enums.DocumentStatus.Draft && nppbkc.Contains(c.NPPBKC_ID)) || c.STATUS == Enums.DocumentStatus.Completed));
                }
                else if (input.UserRole == Enums.UserRole.Manager)
                {
                    var poaList = _poaBll.GetPOAIdByManagerId(input.UserId);
                    var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

                    queryFilter = queryFilter.And(c => (c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.NUMBER)) || c.STATUS == Enums.DocumentStatus.Completed);
                }
                else
                {
                    queryFilter = queryFilter.And(c => c.CREATED_BY == input.UserId || c.STATUS == Enums.DocumentStatus.Completed);
                }
            }

            return queryFilter;
        }

        private List<PBCK1> GetPbck1Data(Expression<Func<PBCK1, bool>> queryFilter, string orderColumn)
        {
            Func<IQueryable<PBCK1>, IOrderedQueryable<PBCK1>> orderBy = null;
            if (!string.IsNullOrEmpty(orderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK1>(orderColumn));
            }

            includeTables += ", CK5";
            var dbData = _repository.Get(queryFilter, orderBy, includeTables);

            return dbData.ToList();
        }

        public Pbck1Dto GetById(long id)
        {
            includeTables += ", PBCK12, PBCK11, PBCK1_PROD_CONVERTER, PBCK1_PROD_PLAN, PBCK1_PROD_PLAN.MONTH1, PBCK1_PROD_PLAN.UOM, PBCK1_PROD_CONVERTER.UOM, PBCK1_DECREE_DOC";
            var dbData = _repository.Get(c => c.PBCK1_ID == id, null, includeTables).FirstOrDefault();
            var mapResult = Mapper.Map<Pbck1Dto>(dbData);
            if (dbData != null)
            {
                mapResult.Pbck1Parent = Mapper.Map<Pbck1Dto>(dbData.PBCK12);
                mapResult.Pbck1Childs = Mapper.Map<List<Pbck1Dto>>(dbData.PBCK11);
            }
            return mapResult;
        }

        public SavePbck1Output Save(Pbck1SaveInput input)
        {
            PBCK1 dbData;
            bool changed = true;

            if (input.Pbck1.Pbck1Id > 0)
            {
                includeTables += ", PBCK12";

                //update
                dbData = _repository.Get(c => c.PBCK1_ID == input.Pbck1.Pbck1Id, null, includeTables).FirstOrDefault();

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //delete first
                _prodConverterBll.DeleteByPbck1Id(input.Pbck1.Pbck1Id);
                _prodPlanBll.DeleteByPbck1Id(input.Pbck1.Pbck1Id);
                _decreeDocBll.DeleteByPbck1Id(input.Pbck1.Pbck1Id);

                //set changes history
                var origin = Mapper.Map<Pbck1Dto>(dbData);
                origin.Pbck1Parent = Mapper.Map<Pbck1Dto>(dbData.PBCK12);
                if (input.Pbck1.Pbck1Reference != null)
                {
                    input.Pbck1.Pbck1Parent = GetById((long)input.Pbck1.Pbck1Reference);
                }

                changed = SetChangesHistory(origin, input.Pbck1, input.UserId);

                if (input.Pbck1.Pbck1Reference == null)
                {
                    dbData.PBCK1_REF = null;
                }

                Mapper.Map<Pbck1Dto, PBCK1>(input.Pbck1, dbData);
                dbData.PBCK1_PROD_CONVERTER = null;
                dbData.PBCK1_PROD_PLAN = null;
                dbData.PBCK1_DECREE_DOC = null;

                dbData.PBCK1_PROD_CONVERTER = Mapper.Map<List<PBCK1_PROD_CONVERTER>>(input.Pbck1.Pbck1ProdConverter);
                dbData.PBCK1_PROD_PLAN = Mapper.Map<List<PBCK1_PROD_PLAN>>(input.Pbck1.Pbck1ProdPlan);
                dbData.PBCK1_DECREE_DOC = Mapper.Map<List<PBCK1_DECREE_DOC>>(input.Pbck1.Pbck1DecreeDoc);
                if (dbData.STATUS == Enums.DocumentStatus.Rejected || dbData.STATUS == Enums.DocumentStatus.GovRejected)
                {
                    dbData.STATUS = Enums.DocumentStatus.Draft;
                }

                ChangeDocNumberByReportedOn(dbData);
            }
            else
            {
                //Insert
                var generateNumberInput = new GenerateDocNumberInput()
                {
                    Year = input.Pbck1.ReportedOn.Value.Year,
                    Month = input.Pbck1.ReportedOn.Value.Month,
                    NppbkcId = input.Pbck1.NppbkcId
                };

                input.Pbck1.Pbck1Number = _docSeqNumBll.GenerateNumber(generateNumberInput);
                input.Pbck1.Status = Enums.DocumentStatus.Draft;
                input.Pbck1.CreatedDate = DateTime.Now;
                dbData = new PBCK1();
                Mapper.Map<Pbck1Dto, PBCK1>(input.Pbck1, dbData);

                _repository.Insert(dbData);

            }

            var output = new SavePbck1Output();

            _uow.SaveChanges();

            output.Success = true;
            output.Id = dbData.PBCK1_ID;
            output.Pbck1Number = dbData.NUMBER;

            //set workflow history
            var getUserRole = _poaBll.GetUserRole(input.UserId);
            var inputAddWorkflowHistory = new Pbck1WorkflowDocumentInput()
            {
                DocumentId = output.Id,
                DocumentNumber = output.Pbck1Number,
                ActionType = input.WorkflowActionType,
                UserId = input.UserId,
                UserRole = getUserRole
            };

            if (changed)
            {
                AddWorkflowHistory(inputAddWorkflowHistory);
            }
            _uow.SaveChanges();

            return output;

        }

        public SavePbck1Output Save(Pbck1WorkflowDocumentInput input)
        {
            PBCK1 dbData = null;
            bool changed = true;

            if (input.DocumentId > 0)
            {
                if (input.ActionType == Enums.ActionType.Reject)
                    _decreeDocBll.DeleteByPbck1Id(input.DocumentId);

                dbData = _repository.GetByID(input.DocumentId);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                var origin = Mapper.Map<Pbck1Dto>(dbData);

                Enums.DocumentStatusGov? status = Enums.DocumentStatusGov.FullApproved;

                switch (input.ActionType) { 
                    case Enums.ActionType.GovApprove:
                        status = Enums.DocumentStatusGov.FullApproved;
                        break;
                    case Enums.ActionType.GovPartialApprove:
                        status = Enums.DocumentStatusGov.PartialApproved;
                        break;
                    case Enums.ActionType.GovReject:
                        status = Enums.DocumentStatusGov.Rejected;
                        break;
                    case Enums.ActionType.Reject:
                        status = null;
                        dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                        break;
                }

                dbData.QTY_APPROVED = input.AdditionalDocumentData.QtyApproved;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;

                foreach (var item in input.AdditionalDocumentData.Pbck1DecreeDoc)
                {
                    dbData.PBCK1_DECREE_DOC.Add(Mapper.Map<PBCK1_DECREE_DOC>(item));
                }

                dbData.STATUS_GOV = status;

                //todo: update remaining quota and necessary data
                

                var inputNew = Mapper.Map<Pbck1Dto>(dbData);

                origin.Pbck1Parent = Mapper.Map<Pbck1Dto>(dbData.PBCK12);

                changed = SetChangesHistory(origin, inputNew, input.UserId);
                
            }

            var output = new SavePbck1Output();

            _uow.SaveChanges();

            output.Success = true;
            output.Id = dbData.PBCK1_ID;
            output.Pbck1Number = dbData.NUMBER;

            //set workflow history
            var getUserRole = _poaBll.GetUserRole(input.UserId);
            var inputAddWorkflowHistory = new Pbck1WorkflowDocumentInput()
            {
                DocumentId = output.Id,
                DocumentNumber = output.Pbck1Number,
                ActionType = Enums.ActionType.Modified,
                UserId = input.UserId,
                UserRole = getUserRole
            };

            if (changed)
            {
                AddWorkflowHistory(inputAddWorkflowHistory);
            }
            _uow.SaveChanges();

            return output;

        }

        public void Delete(long id)
        {
            var dbData = _repository.GetByID(id);

            if (dbData == null)
            {
                _logger.Error(new BLLException(ExceptionCodes.BLLExceptions.DataNotFound));
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            else
            {
                _repository.Delete(dbData);
                _uow.SaveChanges();
            }
        }

        private bool SetChangesHistory(Pbck1Dto origin, Pbck1Dto data, string userId)
        {
            bool changed = false;
            var changesData = new Dictionary<string, bool>();
            changesData.Add("PBCK1_REF", origin.Pbck1Reference == data.Pbck1Reference);
            changesData.Add("PBCK1_TYPE", origin.Pbck1Type == data.Pbck1Type);
            changesData.Add("PERIOD_FROM", origin.PeriodFrom.Equals(data.PeriodFrom));
            changesData.Add("PERIOD_TO", origin.PeriodTo == data.PeriodTo);
            changesData.Add("REPORTED_ON", origin.ReportedOn == data.ReportedOn);
            changesData.Add("NPPBKC_ID", origin.NppbkcId == data.NppbkcId);
            changesData.Add("POA_LIST", origin.NppbkcId == data.NppbkcId);
            changesData.Add("COMPANY", origin.NppbkcId == data.NppbkcId);

            changesData.Add("IS_NPPBKC_IMPORT", origin.IsNppbkcImport == data.IsNppbkcImport);
            changesData.Add("EXC_GOOD_TYP", origin.GoodType == data.GoodType);
            changesData.Add("SUPPLIER_PLANT", origin.SupplierPlant == data.SupplierPlant);
            changesData.Add("SUPPLIER_PORT_ID", origin.SupplierPortId == data.SupplierPortId);
            changesData.Add("SUPPLIER_COMPANY", origin.SupplierCompany == data.SupplierCompany);
            changesData.Add("SUPPLIER_NPPBKC", origin.SupplierNppbkcId == data.SupplierNppbkcId);
            changesData.Add("SUPPLIER_KPPBC", origin.SupplierKppbcName == data.SupplierKppbcName);
            changesData.Add("SUPPLIER_ADDRESS", origin.SupplierAddress == data.SupplierAddress);
            changesData.Add("SUPPLIER_PHONE", origin.SupplierPhone == data.SupplierPhone);
            changesData.Add("PLAN_PROD_FROM", origin.PlanProdFrom == data.PlanProdFrom);
            changesData.Add("PLAN_PROD_TO", origin.PlanProdTo == data.PlanProdTo);
            changesData.Add("REQUEST_QTY", origin.RequestQty == data.RequestQty);
            changesData.Add("REQUEST_QTY_UOM", origin.RequestQtyUomId == data.RequestQtyUomId);
            changesData.Add("LACK1_FROM_MONTH", origin.Lack1FromMonthId == data.Lack1FromMonthId);
            changesData.Add("LACK1_FROM_YEAR", origin.Lack1FormYear == data.Lack1FormYear);
            changesData.Add("LACK1_TO_MONTH", origin.Lack1ToMonthId == data.Lack1ToMonthId);
            changesData.Add("LACK1_TO_YEAR", origin.Lack1ToYear == data.Lack1ToYear);
            changesData.Add("STATUS", origin.Status == data.Status);
            changesData.Add("STATUS_GOV", origin.StatusGov == data.StatusGov);
            changesData.Add("QTY_APPROVED", origin.QtyApproved == data.QtyApproved);
            changesData.Add("DECREE_DATE", origin.DecreeDate == data.DecreeDate);
            changesData.Add("LATEST_SALDO", origin.LatestSaldo == data.LatestSaldo);
            changesData.Add("LATEST_SALDO_UOM", origin.LatestSaldoUomId == data.LatestSaldoUomId);

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var skip = false;
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Enums.MenuList.PBCK1,
                        FORM_ID = data.Pbck1Id.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "PBCK1_REF":
                            changes.OLD_VALUE = origin.Pbck1Reference.HasValue && origin.Pbck1Parent != null
                                ? origin.Pbck1Parent.Pbck1Number
                                : "NULL";
                            changes.NEW_VALUE = data.Pbck1Reference.HasValue && data.Pbck1Parent != null
                                ? data.Pbck1Parent.Pbck1Number
                                : "NULL";
                            changes.FIELD_NAME = "References";
                            break;
                        case "PBCK1_TYPE":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.Pbck1Type);
                            changes.NEW_VALUE = EnumHelper.GetDescription(data.Pbck1Type);
                            changes.FIELD_NAME = "PBCK Type";
                            break;
                        case "PERIOD_FROM":
                            changes.OLD_VALUE = origin.PeriodFrom.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = data.PeriodFrom.ToString("dd MMM yyyy");
                            changes.FIELD_NAME = "Period From";
                            break;
                        case "PERIOD_TO":
                            changes.OLD_VALUE = origin.PeriodTo.HasValue
                                ? origin.PeriodTo.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.NEW_VALUE = data.PeriodTo.HasValue
                                ? data.PeriodTo.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.FIELD_NAME = "Period To";
                            break;
                        case "REPORTED_ON":
                            changes.OLD_VALUE = origin.ReportedOn.HasValue
                                ? origin.ReportedOn.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.NEW_VALUE = data.ReportedOn.HasValue
                                ? data.ReportedOn.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.FIELD_NAME = "Reported On";
                            break;
                        case "NPPBKC_ID":
                            changes.OLD_VALUE = origin.NppbkcId;
                            changes.NEW_VALUE = data.NppbkcId;
                            changes.FIELD_NAME = "NPPBKC ID";
                            break;
                        case "POA_LIST":
                            List<string> List_POA_OLD = new List<string>();
                            List<string> List_POA_NEW = new List<string>();

                            foreach (var old in _poaBll.GetPoaByNppbkcIdAndMainPlant(origin.NppbkcId))
                            {
                                List_POA_OLD.Add(old.PRINTED_NAME);
                            }


                            foreach (var n in _poaBll.GetPoaByNppbkcIdAndMainPlant(data.NppbkcId))
                            {
                                List_POA_NEW.Add(n.PRINTED_NAME);
                            }

                            string POA_OLD = string.Join(", ", List_POA_OLD.ToArray());
                            string POA_NEW = string.Join(", ", List_POA_NEW.ToArray());

                            if (POA_OLD == POA_NEW)
                                skip = true;

                            changes.OLD_VALUE = POA_OLD;
                            changes.NEW_VALUE = POA_NEW;

                            changes.FIELD_NAME = "POA";
                            break;
                        case "COMPANY":
                            var data_old = _plantBll.GetMainPlantByNppbkcId(origin.NppbkcId);
                            var data_new = _plantBll.GetMainPlantByNppbkcId(data.NppbkcId);

                            if ((data_old == null && data_new == null))
                                skip = true;

                            if ((data_old != null && data_new != null) && (data_old.CompanyName == data_new.CompanyName))
                                skip = true;

                            if (!skip)
                            {
                                changes.OLD_VALUE = data_old == null? "" :data_old.CompanyName;
                                changes.NEW_VALUE = data_new == null ? "" : data_new.CompanyName;
                                changes.FIELD_NAME = "COMPANY";
                            }
                            break;
                        case "IS_NPPBKC_IMPORT":
                            changes.OLD_VALUE = origin.IsNppbkcImport != null ? origin.IsNppbkcImport ? "TRUE" : "FALSE" : "FALSE";
                            changes.NEW_VALUE = data.IsNppbkcImport != null ? data.IsNppbkcImport ? "TRUE" : "FALSE" : "FALSE";
                            changes.FIELD_NAME = "PBCK for Import";
                            break;
                        case "EXC_GOOD_TYP":
                            changes.OLD_VALUE = origin.GoodTypeDesc;
                            changes.NEW_VALUE = data.GoodTypeDesc;
                            changes.FIELD_NAME = "Exciseable Goods Description";
                            break;
                        case "SUPPLIER_PLANT":
                            changes.OLD_VALUE = origin.SupplierPlantWerks != null && origin.SupplierPlantWerks != "" ? origin.SupplierPlantWerks + "-" + origin.SupplierPlant : origin.SupplierPlant;
                            changes.NEW_VALUE = data.SupplierPlantWerks != null && data.SupplierPlantWerks != ""? data.SupplierPlantWerks + "-"+ data.SupplierPlant:data.SupplierPlant;
                            changes.FIELD_NAME = "Supplier Plant";
                            break;
                        case "SUPPLIER_PORT_ID":
                            changes.OLD_VALUE = origin.SupplierPortName;
                            changes.NEW_VALUE = data.SupplierPortName;
                            changes.FIELD_NAME = "Supplier Port";
                            break;
                        case "SUPPLIER_COMPANY":
                            changes.OLD_VALUE = origin.SupplierCompany;
                            changes.NEW_VALUE = data.SupplierCompany;
                            changes.FIELD_NAME = "Supplier Company";
                            break;
                        case "SUPPLIER_NPPBKC":
                            changes.OLD_VALUE = origin.SupplierNppbkcId;
                            changes.NEW_VALUE = data.SupplierNppbkcId;
                            changes.FIELD_NAME = "Supplier NPPBKC";
                            break;
                        case "SUPPLIER_KPPBC":
                            changes.OLD_VALUE = origin.SupplierKppbcName;
                            changes.NEW_VALUE = data.SupplierKppbcName;
                            changes.FIELD_NAME = "Supplier KPPBC";
                            break;
                        case "SUPPLIER_ADDRESS":
                            changes.OLD_VALUE = origin.SupplierAddress;
                            changes.NEW_VALUE = data.SupplierAddress;
                            changes.FIELD_NAME = "Supplier Address";
                            break;
                        case "SUPPLIER_PHONE":
                            changes.OLD_VALUE = origin.SupplierPhone;
                            changes.NEW_VALUE = data.SupplierPhone;
                            changes.FIELD_NAME = "Supplier Phone";
                            break;
                        case "PLAN_PROD_FROM":
                            changes.OLD_VALUE = origin.PlanProdFrom.HasValue ? origin.PlanProdFrom.Value.ToString("dd MMM yyyy") : "NULL";
                            changes.NEW_VALUE = data.PlanProdFrom.HasValue ? data.PlanProdFrom.Value.ToString("dd MMM yyyy") : "NULL";
                            changes.FIELD_NAME = "Plan Production From";
                            break;
                        case "PLAN_PROD_TO":
                            changes.OLD_VALUE = origin.PlanProdTo.HasValue ? origin.PlanProdTo.Value.ToString("dd MMM yyyy") : "NULL";
                            changes.NEW_VALUE = data.PlanProdTo.HasValue ? data.PlanProdTo.Value.ToString("dd MMM yyyy") : "NULL";
                            changes.FIELD_NAME = "Plan Production to";
                            break;
                        case "REQUEST_QTY":
                            changes.OLD_VALUE = origin.RequestQty.HasValue ? origin.RequestQty.Value.ToString("N0") : "NULL";
                            changes.NEW_VALUE = data.RequestQty.HasValue ? data.RequestQty.Value.ToString("N0") : "NULL";
                            changes.FIELD_NAME = "Request Qty";
                            break;
                        case "REQUEST_QTY_UOM":
                            changes.OLD_VALUE = !string.IsNullOrEmpty(origin.RequestQtyUomId) ? origin.RequestQtyUomName : "NULL";
                            changes.NEW_VALUE = _uomBll.GetUomNameById(data.RequestQtyUomId);
                            changes.FIELD_NAME = "Request Qty UOM";
                            break;
                        case "LACK1_FROM_MONTH":
                            changes.OLD_VALUE = origin.Lack1FromMonthId.HasValue ? origin.Lack1FromMonthName : "NULL";
                            changes.NEW_VALUE = _monthBll.GetMonth(data.Lack1FromMonthId.Value).MONTH_NAME_ENG;
                            changes.FIELD_NAME = "LACK-1 From Month";
                            break;
                        case "LACK1_FROM_YEAR":
                            changes.OLD_VALUE = origin.Lack1FormYear.HasValue ? origin.Lack1FormYear.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.Lack1FormYear.Value.ToString();
                            changes.FIELD_NAME = "LACK-1 From Year";
                            break;
                        case "LACK1_TO_MONTH":
                            changes.OLD_VALUE = origin.Lack1ToMonthId.HasValue ? origin.Lack1ToMonthName : "NULL";
                            changes.NEW_VALUE = _monthBll.GetMonth(data.Lack1ToMonthId.Value).MONTH_NAME_ENG;
                            changes.FIELD_NAME = "LACK-1 To Month";
                            break;
                        case "LACK1_TO_YEAR":
                            changes.OLD_VALUE = origin.Lack1ToYear.HasValue ? origin.Lack1ToYear.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.Lack1ToYear.Value.ToString();
                            changes.FIELD_NAME = "LACK-1 To Year";
                            break;
                        case "STATUS":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.Status);
                            changes.NEW_VALUE = EnumHelper.GetDescription(data.Status);
                            changes.FIELD_NAME = "Status";
                            break;
                        case "STATUS_GOV":
                            changes.OLD_VALUE = origin.StatusGov.HasValue ? EnumHelper.GetDescription(origin.StatusGov) : "NULL";
                            changes.NEW_VALUE = data.StatusGov.HasValue ? EnumHelper.GetDescription(data.StatusGov) : "NULL";
                            changes.FIELD_NAME = "Status Goverment";
                            break;
                        case "QTY_APPROVED":
                            changes.OLD_VALUE = origin.QtyApproved.HasValue
                                ? origin.QtyApproved.Value.ToString("N0")
                                : "NULL";
                            changes.NEW_VALUE = data.QtyApproved.HasValue ? data.QtyApproved.Value.ToString("N0") : "NULL";
                            changes.FIELD_NAME = "Qty Approved";
                            break;
                        case "DECREE_DATE":
                            changes.OLD_VALUE = origin.DecreeDate.HasValue
                                ? origin.DecreeDate.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.NEW_VALUE = data.DecreeDate.HasValue
                                ? data.DecreeDate.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.FIELD_NAME = "Decree Date";
                            break;
                        case "LATEST_SALDO":
                            changes.OLD_VALUE = origin.LatestSaldo.HasValue
                                ? origin.LatestSaldo.Value.ToString("N0")
                                : "NULL";
                            changes.NEW_VALUE = data.LatestSaldo.HasValue
                                ? data.LatestSaldo.Value.ToString("N0")
                                : "NULL";
                            changes.FIELD_NAME = "Latest Saldo";
                            break;
                        case "LATEST_SALDO_UOM":
                            changes.OLD_VALUE = !string.IsNullOrEmpty(origin.LatestSaldoUomId)
                                ? origin.LatestSaldoUomName
                                : "NULL";
                            changes.NEW_VALUE = _uomBll.GetUomNameById(data.LatestSaldoUomId);
                            changes.FIELD_NAME = "Latest Saldo UOM";
                            break;
                    }
                    if(!skip)
                        _changesHistoryBll.AddHistory(changes);
                    changed = true;
                }
            }

            return changed;
        }

        public string GetPbckNumberById(long id)
        {
            var dbData = _repository.GetByID(id);
            return dbData == null ? string.Empty : dbData.NUMBER;
        }

        public List<Pbck1ProdConverterOutput> ValidatePbck1ProdConverterUpload(List<Pbck1ProdConverterInput> inputs, string nppbkc, bool isNppbkcImportChekced)
        {
            var messageList = new List<string>();
            var outputList = new List<Pbck1ProdConverterOutput>();

            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                var output = Mapper.Map<Pbck1ProdConverterOutput>(inputItem);
                output.IsValid = true;

                var checkCountDataProductCode = inputs.Where(c => c.ProductCode == output.ProductCode && c.BrandCE == output.BrandCE).ToList();
                if (checkCountDataProductCode.Count > 1)
                {
                    //double product code
                    output.IsValid = false;
                    messageList.Add("Duplicate Product Code [" + output.ProductCode + "]");
                }

                //Product Code Validation
                #region -------------- Product Code Validation --------------
                List<string> messages;
                ZAIDM_EX_PRODTYP prodTypeData = null;
                //if (ValidateProductCode(output.ProductCode, out messages, out prodTypeData))

                //use product alias instead of product code
                if (ValidateProductAlias(output.ProductCode, out messages, out prodTypeData))
                {
                    output.ProductCode = prodTypeData.PROD_CODE;
                    output.ProdTypeAlias = prodTypeData.PRODUCT_ALIAS;
                    output.ProdTypeName = prodTypeData.PRODUCT_TYPE;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                //Converted UOM Validation
                #region -------------------- Converted UOM Validation ------------

                if (!ValidateDecimal(output.ConverterOutput, "Converted Output", out messages))
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                //UOM Validation
                #region -------------- UOM Validation --------------------

                string uomName;
                string uomId;
                //validate by UOM Name
                if (!ValidateUomId(output.ConverterUom, out messages, out uomName, out uomId))
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }
                else
                {
                    output.ConverterUomId = uomId;
                    output.ConverterUom = uomName;
                }

                #endregion

                #region -------------- Brand Validation --------------------
                if (!ValidateBrand(output.BrandCE, prodTypeData.PROD_CODE, nppbkc, prodTypeData.PRODUCT_ALIAS, out messages, isNppbkcImportChekced))
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }
                #endregion

                #region -------------- Set Message Info if exists ---------------

                if (messageList.Count > 0)
                {
                    output.IsValid = false;
                    output.Message = "";
                    foreach (var message in messageList)
                    {
                        output.Message += message + ";";
                    }
                }
                else
                {
                    output.IsValid = true;
                }

                #endregion

                outputList.Add(output);

            }
            return outputList;
        }

        public ValidatePbck1ProdPlanUploadOutput ValidatePbck1ProdPlanUpload(ValidatePbck1ProdPlanUploadParamInput input)
        {

            var rc = new ValidatePbck1ProdPlanUploadOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            var messageList = new List<string>();
            var outputList = new List<Pbck1ProdPlanOutput>();

            //input validation
            var periodFromMonth = 0;
            var periodToMonth = 0;
            var outValidation = ValidatePbck1ProdPlanUploadInput(input, out periodFromMonth, out periodToMonth);
            if (!outValidation.Success) return outValidation;

            foreach (var inputItem in input.ProdPlanData) //   <--- go back to here --------+
            {
                messageList.Clear();

                var output = Mapper.Map<Pbck1ProdPlanOutput>(inputItem);
                output.IsValid = true;

                #region ------------- Product Code Validation ----------
                List<string> messages;
                ZAIDM_EX_PRODTYP prodTypeData = null;
                //if (ValidateProductCode(output.ProductCode, out messages, out prodTypeData))

                //use product alias instead of product code
                if (ValidateProductAlias(output.ProductCode, out messages, out prodTypeData))
                {
                    output.ProductCode = prodTypeData.PROD_CODE;
                    output.ProdTypeAlias = prodTypeData.PRODUCT_ALIAS;
                    output.ProdTypeName = prodTypeData.PRODUCT_TYPE;
                }
                else
                {
                    output.ProductCode = "";
                    output.ProdTypeAlias = output.ProductCode;
                    output.ProdTypeName = "";
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region -------------- Month Validation -----------

                string monthName;

                if (ValidateMonth(output.Month, out messages, out monthName))
                {
                    var monthNumber = int.Parse(output.Month);
                    if (monthNumber >= periodFromMonth && monthNumber <= periodToMonth)
                    {
                        output.MonthName = monthName;
                    }
                    else
                    {
                        //not valid
                        output.IsValid = false;
                        messageList.Add("Month is not in range of Period Plan");
                    }
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region --------------- Amount Validation -----------

                if (!ValidateDecimal(output.Amount, "Amount", out messages))
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region ------------------ BKC Required -------

                if (!ValidateDecimal(output.BkcRequired, "BKCRequired", out messages))
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region ------------------ BKC Required UOM -------

                string uomName;
                string uomId;
                //validate by Uom Id
                if (!ValidateUomId(output.BkcRequiredUomId, input.GoodType, out messages, out uomName, out uomId))
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }
                else
                {
                    output.BkcRequiredUomName = uomName;
                    output.BkcRequiredUomId = uomId;
                }


                #endregion

                #region -------------- Set Message Info if exists ---------------

                if (messageList.Count > 0)
                {
                    output.IsValid = false;
                    output.Message = "";
                    foreach (var message in messageList)
                    {
                        output.Message += message + ";";
                    }
                }
                else
                {
                    output.IsValid = true;
                }

                #endregion

                outputList.Add(output);

            }
            rc.Data = outputList;
            return rc;
        }

        private ValidatePbck1ProdPlanUploadOutput ValidatePbck1ProdPlanUploadInput(
            ValidatePbck1ProdPlanUploadParamInput input, out int periodMonthFrom, out int periodMonthTo)
        {
            periodMonthFrom = 0;
            periodMonthTo = 0;

            if (!input.ProdPlanPeriodFrom.HasValue)
            {
                return new ValidatePbck1ProdPlanUploadOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Pbck1ProdPlanUploadInvalidParameter.ToString(),
                    ErrorMessage = "Please specify Production Plan Period From"
                };
            }

            if (!input.ProdPlanPeriodTo.HasValue)
            {
                return new ValidatePbck1ProdPlanUploadOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Pbck1ProdPlanUploadInvalidParameter.ToString(),
                    ErrorMessage = "Please specify Production Plan Period To"
                };
            }

            if (string.IsNullOrEmpty(input.GoodType))
            {
                return new ValidatePbck1ProdPlanUploadOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Pbck1ProdPlanUploadInvalidParameter.ToString(),
                    ErrorMessage = "Please specify Excisable Goods Type"
                };
            }

            var periodFromMonth = input.ProdPlanPeriodFrom.Value.Month;
            var periodToMonth = input.ProdPlanPeriodTo.Value.Month;
            var periodToYear = input.ProdPlanPeriodFrom.Value.Year;
            var periodFromYear = input.ProdPlanPeriodFrom.Value.Year;

            if (periodFromYear != periodToYear)
            {
                return new ValidatePbck1ProdPlanUploadOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Pbck1ProdPlanUploadInvalidParameter.ToString(),
                    ErrorMessage = "Period Plan year is not same"
                };
            }

            if (periodFromMonth >= periodToMonth)
            {
                return new ValidatePbck1ProdPlanUploadOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Pbck1ProdPlanUploadInvalidParameter.ToString(),
                    ErrorMessage = "Period Plan From is greater than or equals with Period Plan To"
                };
            }

            periodMonthFrom = periodFromMonth;
            periodMonthTo = periodToMonth;

            return new ValidatePbck1ProdPlanUploadOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };
        }

        //private bool ValidateProductCode(string productCode, out List<string> message, out ZAIDM_EX_PRODTYP productData)
        //{
        //    productData = null;
        //    var valResult = false;
        //    var messageList = new List<string>();
        //    #region ------------Product Code Validation-------------
        //    if (!string.IsNullOrWhiteSpace(productCode))
        //    {

        //        productData = _prodTypeBll.GetByCode(productCode);
        //        if (productData == null)
        //        {
        //            messageList.Add("ProductCode not valid");
        //        }
        //        else
        //        {
        //            valResult = true;
        //        }
        //    }
        //    else
        //    {
        //        messageList.Add("ProductCode is empty");
        //    }

        //    #endregion

        //    message = messageList;

        //    return valResult;
        //}

        private bool ValidateProductAlias(string productAlias, out List<string> message,
            out ZAIDM_EX_PRODTYP productData)
        {
            productData = null;
            var valResult = false;
            var messageList = new List<string>();
            #region ------------Product Code Validation-------------
            if (!string.IsNullOrWhiteSpace(productAlias))
            {

                productData = _prodTypeBll.GetByAlias(productAlias);
                if (productData == null)
                {
                    messageList.Add("Product Alias [" + productAlias + "] not valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Product Alias is empty");
            }

            #endregion

            message = messageList;

            return valResult;
        }

        private bool ValidateMonth(string month, out List<string> message, out string monthName)
        {
            monthName = string.Empty;
            var valResult = false;
            var messageList = new List<string>();

            if (string.IsNullOrEmpty(month))
            {
                messageList.Add("Month is empty");
            }
            else
            {
                int monthNumber = 0;
                if (!int.TryParse(month, out monthNumber))
                {
                    //not valid
                    messageList.Add("Month [" + month + "] is not valid");
                }
                else
                {
                    //valid, get month name
                    var monthData = _monthBll.GetMonth(monthNumber);
                    if (monthData == null)
                    {
                        messageList.Add("Month [" + month + "] is not valid");
                    }
                    else
                    {
                        valResult = true;
                        monthName = monthData.MONTH_NAME_ENG;
                    }
                }
            }

            message = messageList;

            return valResult;

        }

        private bool ValidateDecimal(string nominal, string fieldMessage, out List<string> message)
        {
            var valResult = false;
            var messageList = new List<string>();

            if (!string.IsNullOrWhiteSpace(nominal))
            {
                decimal amountConvert;
                if (!decimal.TryParse(nominal, out amountConvert))
                {
                    messageList.Add(string.Format("{0} is not valid", fieldMessage));
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add(string.Format("{0} is empty", fieldMessage));
            }

            message = messageList;

            return valResult;
        }

        private bool ValidateUomId(string uom, out List<string> message, out string uomName, out string uomId)
        {
            var valResult = false;
            var messageList = new List<string>();
            uomName = string.Empty;
            uomId = string.Empty;
            if (!string.IsNullOrWhiteSpace(uom))
            {
                var uomData = _uomBll.GetById(uom);
                if (uomData != null)
                {
                    uomName = uomData.UOM_DESC;
                    uomId = uomData.UOM_ID;
                    valResult = true;
                }
                else
                {
                    messageList.Add("UOM Id [" + uom + "] not valid");
                }
            }
            else
            {
                messageList.Add("UOM is empty");
            }

            message = messageList;

            return valResult;
        }


        private bool ValidateUomId(string uom, string goodtype, out List<string> message, out string uomName, out string uomId)
        {
            var valResult = false;
            var messageList = new List<string>();
            uomName = string.Empty;
            uomId = string.Empty;
            if (!string.IsNullOrWhiteSpace(uom))
            {
                var uomData = _uomBll.GetById(uom);
                var gtData = _goodTypeBll.GetById(goodtype);                
                if (uomData != null)
                {
                    if (goodtype == "02" && uom == "G")
                    {
                        uomName = uomData.UOM_DESC;
                        uomId = uomData.UOM_ID;
                        valResult = true;
                    }
                    else if (goodtype == "04" && uom == "L")
                    {
                        uomName = uomData.UOM_DESC;
                        uomId = uomData.UOM_ID;
                        valResult = true;
                    }
                    else {
                        messageList.Add("UOM Id [" + uom + "] not valid for " + gtData.EXT_TYP_DESC);
                    }
                }
                else
                {
                    messageList.Add("UOM Id [" + uom + "] not valid for " + gtData.EXT_TYP_DESC);
                }
            }
            else
            {
                messageList.Add("UOM is empty");
            }

            message = messageList;

            return valResult;
        }

        private bool ValidateUomName(string uomName, out List<string> message, out string uomNameFromDb,
            out string uomId)
        {
            var valResult = false;
            var messageList = new List<string>();
            uomNameFromDb = string.Empty;
            uomId = string.Empty;
            if (!string.IsNullOrWhiteSpace(uomName))
            {
                var uomData = _uomBll.GetByName(uomName);
                if (uomData != null)
                {
                    uomNameFromDb = uomData.UOM_DESC;
                    uomId = uomData.UOM_ID;
                    valResult = true;
                }
                else
                {
                    messageList.Add("UOM Name [" + uomName + "] not valid");
                }
            }
            else
            {
                messageList.Add("UOM is empty");
            }

            message = messageList;

            return valResult;
        }

        private bool ValidateBrand(string brand, string prodCode, string nppbkc,string alias,out List<string> message, bool isCheckedPbck1Import)
        {
            var valResult = false;
            var messageList = new List<string>();

            if (!string.IsNullOrWhiteSpace(brand))
            {
                var brandData = _brandRegistrationBll.GetBrandForProdConv(brand, prodCode, nppbkc);

                if (brandData != null)
                {
                    if (isCheckedPbck1Import)
                    {
                        var nppbckImport =
                            _plantBll.GetPlantByNppbkc(nppbkc)
                                .Where(c => c.NPPBKC_IMPORT_ID != null)
                                .Select(c => c.NPPBKC_IMPORT_ID)
                                .ToList();

                        if (nppbckImport.Any())
                        {
                            var listBrandNppbkcImport = new List<ZAIDM_EX_BRAND>();
                            foreach (var item in nppbckImport)
                            {
                                var brandDataNppbckImport = _brandRegistrationBll.GetBrandForProdConv(brand, prodCode,
                                    item);
                                if (brandDataNppbckImport != null) listBrandNppbkcImport.Add(brandDataNppbckImport);
                            }
                            if (listBrandNppbkcImport.Any()) valResult = true;
                            else messageList.Add("Brand [" + brand + "] for [" + alias + "] and NPPBKC [" + nppbkc + "] not valid");
                        }
                    }
                    else
                    {
                        valResult = true;
                    }
                }
                else
                {
                    messageList.Add("Brand [" + brand + "] for [" + alias + "] and NPPBKC [" + nppbkc + "] not valid");
                }
            }
            else
            {
                messageList.Add("Brand is empty");
            }

            message = messageList;

            return valResult;
        }

        public void Pbck1Workflow(Pbck1WorkflowDocumentInput input)
        {
            
            var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Modified:
                    if(input.DocumentStatus == Enums.DocumentStatus.Completed){
                        //SubmitDocument(input);
                        isNeedSendNotif = false;
                    }
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
                    //isNeedSendNotif = false;
                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocument(input);
                    //isNeedSendNotif = false;
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

        #region workflow

        private void AddWorkflowHistory(Pbck1WorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.FORM_TYPE_ID = Enums.FormType.PBCK1;

            _workflowHistoryBll.Save(dbData);

        }

        private void SubmitDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.Draft && dbData.STATUS != Enums.DocumentStatus.Rejected && dbData.STATUS != Enums.DocumentStatus.GovRejected)
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

        private void ApproveDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
                                    {
                                        CreatedUser = dbData.CREATED_BY,
                                        CurrentUser = input.UserId,
                                        DocumentStatus = dbData.STATUS,
                                        UserRole = input.UserRole,
                                        NppbkcId = dbData.NPPBKC_ID,
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
                if (dbData.STATUS == Enums.DocumentStatus.WaitingForApproval)
                {
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

        private void RejectDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
            //                        {
            //                            CreatedUser = dbData.CREATED_BY,
            //                            CurrentUser = input.UserId,
            //                            DocumentStatus = dbData.STATUS,
            //                            UserRole = input.UserRole,
            //                            DocumentNumber = dbData.NUMBER,
            //                            NppbkcId = dbData.NPPBKC_ID
            //                        });

            if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);

            //change back to draft
            dbData.STATUS = Enums.DocumentStatus.Rejected;

            //todo ask
            //dbData.APPROVED_BY_POA = null;
            //dbData.APPROVED_DATE_POA = null;

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);
            var origin = Mapper.Map<Pbck1Dto>(dbData);
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            //WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
            //WorkflowStatusGovAddChanges(input, dbData.STATUS_GOV, Enums.DocumentStatusGov.FullApproved);

            dbData.STATUS = Enums.DocumentStatus.Completed;

            //todo: update remaining quota and necessary data
            dbData.PBCK1_DECREE_DOC = null;
            dbData.QTY_APPROVED = input.AdditionalDocumentData.QtyApproved;
            dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
            dbData.PBCK1_DECREE_DOC = Mapper.Map<List<PBCK1_DECREE_DOC>>(input.AdditionalDocumentData.Pbck1DecreeDoc);
            dbData.STATUS_GOV = Enums.DocumentStatusGov.FullApproved;

            //dbData.APPROVED_BY_POA = input.UserId;
            //dbData.APPROVED_DATE_POA = DateTime.Now;

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.NUMBER;

            var inputNew = Mapper.Map<Pbck1Dto>(dbData);

            SetChangesHistory(origin, inputNew, input.UserId);

            AddWorkflowHistory(input);
        }

        private void GovPartialApproveDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);
            var origin = Mapper.Map<Pbck1Dto>(dbData);
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            //WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
            //WorkflowStatusGovAddChanges(input, dbData.STATUS_GOV, Enums.DocumentStatusGov.PartialApproved);

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.NUMBER;

            //todo: update remaining quota and necessary data
            dbData.PBCK1_DECREE_DOC = null;
            dbData.STATUS = Enums.DocumentStatus.Completed;
            dbData.QTY_APPROVED = input.AdditionalDocumentData.QtyApproved;
            dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
            dbData.PBCK1_DECREE_DOC = Mapper.Map<List<PBCK1_DECREE_DOC>>(input.AdditionalDocumentData.Pbck1DecreeDoc);
            dbData.STATUS_GOV = Enums.DocumentStatusGov.PartialApproved;

            //dbData.APPROVED_BY_POA = input.UserId;
            //dbData.APPROVED_DATE_POA = DateTime.Now;

            
            var inputNew = Mapper.Map<Pbck1Dto>(dbData);

            SetChangesHistory(origin, inputNew, input.UserId);

            AddWorkflowHistory(input);
        }

        private void GovRejectedDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.GovRejected);
            WorkflowStatusGovAddChanges(input, dbData.STATUS_GOV, Enums.DocumentStatusGov.Rejected);

            dbData.STATUS = Enums.DocumentStatus.GovRejected;
            dbData.STATUS_GOV = Enums.DocumentStatusGov.Rejected;

            //dbData.APPROVED_BY_POA = input.UserId;
            //dbData.APPROVED_DATE_POA = DateTime.Now;

            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void WorkflowStatusAddChanges(Pbck1WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.PBCK1,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "Status",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = EnumHelper.GetDescription(oldStatus),
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };
            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowStatusGovAddChanges(Pbck1WorkflowDocumentInput input, Enums.DocumentStatusGov? oldStatus, Enums.DocumentStatusGov newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.PBCK1,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "Status Goverment",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void SendEmailWorkflow(Pbck1WorkflowDocumentInput input)
        {
            //todo: body message from email template
            //todo: to = ?
            //todo: subject = from email template
            //var to = "irmansulaeman41@gmail.com";
            //var subject = "this is subject for " + input.DocumentNumber;
            //var body = "this is body message for " + input.DocumentNumber;
            //var from = "a@gmail.com";

            includeTables += ", PBCK12, PBCK11, PBCK1_PROD_CONVERTER, PBCK1_PROD_PLAN, PBCK1_PROD_PLAN.MONTH1, PBCK1_PROD_PLAN.UOM, PBCK1_PROD_CONVERTER.UOM, PBCK1_DECREE_DOC";

            var pbck1Data = Mapper.Map<Pbck1Dto>(_repository.Get(c => c.PBCK1_ID == input.DocumentId, null, includeTables).FirstOrDefault());

            var mailProcess = ProsesMailNotificationBody(pbck1Data, input.ActionType, input.Comment);

            //distinct To email
            var ListTo = mailProcess.To.Distinct().ToList();

            //distinct CC email
            var ListCC = mailProcess.CC.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, ListCC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        #endregion

        #region Summary Reports

        public List<Pbck1SummaryReportDto> GetSummaryReportByParam(Pbck1GetSummaryReportByParamInput input)
        {
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            //===== Fixing Bug PBCK1 No.164 ============
            //queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);
            //==========================================

            if (input.YearFrom.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_FROM.HasValue && c.PERIOD_FROM.Value.Year >= input.YearFrom.Value);
            if (input.YearTo.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_TO.HasValue && c.PERIOD_TO.Value.Year <= input.YearTo.Value);
            if (!string.IsNullOrEmpty(input.CompanyCode))
                queryFilter = queryFilter.And(c => c.NPPBKC_BUKRS == input.CompanyCode);
            if (!string.IsNullOrEmpty(input.NppbkcId))
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);
            if (!string.IsNullOrEmpty(input.pbck1Number))
                queryFilter = queryFilter.And(c => c.NUMBER == input.pbck1Number);

            var pbck1Data = GetPbck1Data(queryFilter, input.SortOrderColumn);

            if (pbck1Data == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //todo: ask the cleanest way
            var rc = Mapper.Map<List<Pbck1SummaryReportDto>>(pbck1Data);
            foreach (var item in rc)
            {
                item.Pbck1Parent = Mapper.Map<Pbck1SummaryReportDto>(pbck1Data.Where(c => c.PBCK1_ID == item.Pbck1Id).Select(c => c.PBCK12).FirstOrDefault());
                item.Pbck1Childs = Mapper.Map<List<Pbck1SummaryReportDto>>(pbck1Data.Where(c => c.PBCK1_ID == item.Pbck1Id).Select(c => c.PBCK11).FirstOrDefault());
            }
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < rc.Count; i++)
            {
                var nppbckData = _nppbkcbll.GetDetailsById(rc[i].NppbkcId);
                if (nppbckData != null)
                {
                    //rc[i].NppbkcKppbcId = nppbckData.KPPBC_ID;
                    rc[i].NppbkcPlants = Mapper.Map<List<T001WDto>>(nppbckData.T001W);
                }
            }

            return rc;
        }

        #endregion

        #region Monitoring Usages

        public List<Pbck1MonitoringUsageDto> GetMonitoringUsageByParam(Pbck1GetMonitoringUsageByParamInput input)
        {
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed
                && c.PBCK1_TYPE == Enums.PBCK1Type.New);

            if (input.YearFrom.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_FROM.HasValue && c.PERIOD_FROM.Value.Year >= input.YearFrom.Value);

            if (input.YearTo.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_TO.HasValue && c.PERIOD_TO.Value.Year <= input.YearTo.Value);

            if (!string.IsNullOrEmpty(input.CompanyCode))
                queryFilter = queryFilter.And(c => c.NPPBKC_BUKRS == input.CompanyCode);

            if (!string.IsNullOrEmpty(input.NppbkcId))
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);

            var pbck1Data = GetPbck1Data(queryFilter, input.SortOrderColumn);

            if (pbck1Data == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var listData = Mapper.Map<List<Pbck1MonitoringUsageDto>>(pbck1Data);

            listData = GetCorrectReceivedQuantity(listData);

            return listData;
        }

        private List<Pbck1MonitoringUsageDto> GetCorrectReceivedQuantity(List<Pbck1MonitoringUsageDto> listData)
        {
            var list = listData;

            foreach(var item in list)
            {
                var receivedAdditional = Convert.ToDecimal(0);

                var listPbckAdditional = _repository.GetByID(item.Pbck1Id).PBCK11;

                foreach (var data in listPbckAdditional)
                {
                    var additionalRec = _repository.GetByID(data.PBCK1_ID).CK5
                                            .Where(c => c.STATUS_ID != Enums.DocumentStatus.Cancelled).Sum(s => s.GRAND_TOTAL_EX);

                    receivedAdditional += additionalRec.Value;
                }

                item.ReceivedAdditional = receivedAdditional;
            }

            return list;
        }

        #endregion

        public Pbck1ReportDto GetPrintOutDataById(int id)
        {
            var rc = new Pbck1ReportDto();
            includeTables += ", PBCK12, PBCK11, PBCK1_PROD_CONVERTER, PBCK1_PROD_PLAN, PBCK1_PROD_PLAN.MONTH1, PBCK1_PROD_PLAN.UOM, PBCK1_PROD_CONVERTER.UOM, PBCK1_DECREE_DOC";
            var dbData = _repository.Get(c => c.PBCK1_ID == id, null, includeTables).FirstOrDefault();

            var dbDataSuratKonversi = _repository.Get(c => c.NPPBKC_ID == dbData.NPPBKC_ID, null, includeTables).FirstOrDefault();

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            rc.Detail.Pbck1Id = dbData.PBCK1_ID;
            rc.Detail.Pbck1Number = dbData.NUMBER;
            rc.Detail.Pbck1AdditionalText = dbData.PBCK1_TYPE == Enums.PBCK1Type.Additional ? "Tambahan" : "";
            if (dbData.PERIOD_FROM != null) rc.Detail.Year = dbData.PERIOD_FROM.Value.ToString("yyyy");

            //GET VENDOR BY NPPBKC_ID ON PBCK-1 FORM AND KPPBC_ID ON NPPBKC
            var nppbkcDetails = _nppbkcbll.GetDetailsById(dbData.NPPBKC_ID);
            if (nppbkcDetails != null)
            {
                var vendorData = _lfaBll.GetById(nppbkcDetails.KPPBC_ID);
                if (vendorData != null)
                {
                    rc.Detail.VendorAliasName = vendorData.NAME2;
                    //todo: change with field CITY FROM VENDOR MASTER
                    rc.Detail.VendorCityName = vendorData.ORT01;
                }
                rc.Detail.NppbkcAddress = "- " + string.Join(Environment.NewLine + "- ", nppbkcDetails.T001W.Select(d => d.ADDRESS).ToArray());
                var mainPlant = nppbkcDetails.T001W.FirstOrDefault(c => c.IS_MAIN_PLANT.HasValue && c.IS_MAIN_PLANT.Value);
                //get plant where nppbckid is nppbckid_import
                var plantImport = _plantBll.GetMainPlantByNppbkcId(dbData.NPPBKC_ID);
                if (mainPlant != null || plantImport != null)
                {
                    rc.Detail.PlantPhoneNumber = mainPlant == null ? plantImport.PHONE : mainPlant.PHONE;

                    //Get BrandRegistration Data
                    //var brandRegistrationDataByMainPlant = _brandRegistrationBll.GetByPlantId(mainPlant.WERKS);

                    if (dbData.PBCK1_PROD_CONVERTER != null && dbData.PBCK1_PROD_PLAN.Count > 0)
                    {
                        //var dataJoined = (from brand in brandRegistrationDataByMainPlant
                        //                  join prodConv in dbDataSuratKonversi.PBCK1_PROD_CONVERTER on brand.PROD_CODE equals prodConv.PROD_CODE
                        //                  select new Pbck1ReportBrandRegistrationDto()
                        //                  {
                        //                      Type = prodConv.PRODUCT_ALIAS,
                        //                      Brand = brand.BRAND_CE,
                        //                      Kadar = "-", //hardcoded, ref: FS PBCK-1 EMS Version document
                        //                      Convertion =
                        //                          prodConv.CONVERTER_OUTPUT.HasValue ? prodConv.CONVERTER_OUTPUT.Value.ToString("N2") : "-",
                        //                      ConvertionUom = prodConv.UOM.UOM_DESC,
                        //                      ConvertionUomId = prodConv.CONVERTER_UOM_ID
                        //                  }).DistinctBy(c => c.Brand).ToList();
                        var dataJoined = dbData.PBCK1_PROD_CONVERTER.ToList();
                        var convertedUomData =
                            dbData.PBCK1_PROD_CONVERTER.FirstOrDefault(c => !string.IsNullOrEmpty(c.CONVERTER_UOM_ID));
                        if (convertedUomData != null)
                        {
                            rc.Detail.ConvertedUomId = convertedUomData.CONVERTER_UOM_ID;
                        }

                        rc.BrandRegistrationList = new List<Pbck1ReportBrandRegistrationDto>();
                        foreach (var dataItem in dataJoined)
                        {
                            rc.BrandRegistrationList.Add(new Pbck1ReportBrandRegistrationDto() { Brand = dataItem.BRAND_CE, Convertion = dataItem.CONVERTER_OUTPUT.Value.ToString("N2"), ConvertionUomId = dataItem.CONVERTER_UOM_ID, Kadar = "-", Type = dataItem.PRODUCT_ALIAS, ConvertionUom = dataItem.UOM.UOM_DESC});
                        }
                    }
                }
                else
                {
                    rc.BrandRegistrationList = new List<Pbck1ReportBrandRegistrationDto>();
                }
                rc.Detail.NppbkcCity = nppbkcDetails.CITY;
            }

            var poaId = !string.IsNullOrEmpty(dbData.APPROVED_BY_POA) ? dbData.APPROVED_BY_POA : dbData.CREATED_BY;

            var poaDetails = _poaBll.GetDetailsById(poaId);
            if (poaDetails != null)
            {
                rc.Detail.PoaName = poaDetails.PRINTED_NAME;
                rc.Detail.PoaTitle = poaDetails.TITLE;
                rc.Detail.PoaAddress = poaDetails.POA_ADDRESS;
                if (!string.IsNullOrEmpty(poaDetails.MANAGER_ID))
                {
                    var managerData = _userBll.GetUserById(poaDetails.MANAGER_ID);
                    if (managerData != null)
                    {
                        //if external supplier port true
                        if (String.IsNullOrEmpty(dbData.SUPPLIER_PLANT_WERKS))
                        {
                            rc.Detail.ExciseManager = string.IsNullOrEmpty(dbData.SUPPLIER_COMPANY) ? "-" : dbData.SUPPLIER_COMPANY;
                        }
                        else
                        {
                            rc.Detail.ExciseManager = managerData.FIRST_NAME + " " + managerData.LAST_NAME;
                        }
                        
                    }
                }

            }

            rc.Detail.CompanyName = dbData.NPPBCK_BUTXT;
            rc.Detail.NppbkcId = dbData.NPPBKC_ID;
            rc.Detail.ExcisableGoodsDescription = dbData.EXC_TYP_DESC;

            //ambil dari prod converter
            if (dbData.PBCK1_PROD_CONVERTER != null)
            {
                rc.Detail.ProdConverterProductType = string.Join(", ",
                    dbData.PBCK1_PROD_CONVERTER.Select(d => d.PRODUCT_TYPE + " (" + d.PRODUCT_ALIAS + ")").Distinct().ToArray());

                var prodConverterGroup = dbData.PBCK1_PROD_CONVERTER.GroupBy(p => new
                {
                    p.PROD_CODE,
                    p.PRODUCT_TYPE,
                    p.PRODUCT_ALIAS,
                    p.CONVERTER_UOM_ID,
                    p.UOM.UOM_DESC
                }).Select(g => new
                {
                    g.Key.PROD_CODE,
                    g.Key.PRODUCT_TYPE,
                    g.Key.PRODUCT_ALIAS,
                    g.Key.CONVERTER_UOM_ID,
                    g.Key.UOM_DESC,
                    Total = g.Sum(p => p.CONVERTER_OUTPUT)
                });
                //rc.Detail.ProductConvertedOutputs = string.Join(Environment.NewLine,
                //    prodConverterGroup.Select(d => String.Format("{0:n}", d.Total.Value) + " " + d.UOM_DESC + " " + d.PRODUCT_TYPE + " (" + d.PRODUCT_ALIAS + ")").ToArray());
            }
            if (dbData.PERIOD_FROM.HasValue)
            {
                rc.Detail.PeriodFrom = DateReportString(dbData.PERIOD_FROM.Value);
            }
            if (dbData.PERIOD_TO.HasValue)
            {
                rc.Detail.PeriodTo = DateReportString(dbData.PERIOD_TO.Value);
            }
            // ReSharper disable once PossibleInvalidOperationException
            rc.Detail.RequestQty = String.Format("{0:n}", dbData.REQUEST_QTY.Value);
            rc.Detail.RequestQtyUom = dbData.REQUEST_QTY_UOM;
            rc.Detail.RequestQtyUomName = dbData.UOM.UOM_DESC;
            if (dbData.LATEST_SALDO != null) rc.Detail.LatestSaldo = String.Format("{0:n}", dbData.LATEST_SALDO.Value);
            rc.Detail.LatestSaldoUom = dbData.LATEST_SALDO_UOM;
            rc.Detail.SupplierPlantName = dbData.SUPPLIER_PLANT;
            rc.Detail.SupplierPlantId = dbData.SUPPLIER_PLANT_WERKS;
            rc.Detail.SupplierNppbkcId = dbData.SUPPLIER_NPPBKC_ID;
            rc.Detail.SupplierPlantAddress = dbData.SUPPLIER_ADDRESS;
            rc.Detail.SupplierPlantPhone = !string.IsNullOrEmpty(dbData.SUPPLIER_PHONE) ? dbData.SUPPLIER_PHONE : "-";
            rc.Detail.SupplierKppbcId = dbData.SUPPLIER_KPPBC_ID;
            rc.Detail.SupplierCompanyName = string.IsNullOrEmpty(dbData.SUPPLIER_COMPANY) ? "-" : dbData.SUPPLIER_COMPANY;
            
            if (!string.IsNullOrEmpty(rc.Detail.SupplierKppbcId))
            {
                var kppbcDetail = _kppbcbll.GetById(rc.Detail.SupplierKppbcId);
                if (kppbcDetail != null)
                {
                    //rc.Detail.SupplierKppbcMengetahui = kppbcDetail.MENGETAHUI_DETAIL;
                    if (!string.IsNullOrEmpty(kppbcDetail.MENGETAHUI_DETAIL))
                    {
                        var strToSplit = kppbcDetail.MENGETAHUI_DETAIL.Replace("ub<br />", "|");
                        List<string> stringList = strToSplit.Split('|').ToList();
                        rc.Detail.SupplierKppbcMengetahui = stringList[0].Replace("<br />", Environment.NewLine);
                        rc.Detail.SupplierKppbcMengetahui =
                            rc.Detail.SupplierKppbcMengetahui.Replace("Mengetahui", string.Empty)
                                .Replace("mengetahui", string.Empty)
                                .Replace("Kepala", string.Empty).Replace("kepala", string.Empty).Trim();
                    }

                    var tipeMadya = kppbcDetail.KPPBC_TYPE;
                    if (kppbcDetail.KPPBC_TYPE.ToLower().Contains("madya"))
                    {
                        tipeMadya = "Tipe " + tipeMadya;
                    }

                    rc.Detail.TipeMadya = tipeMadya;
                }
                else
                {
                    rc.Detail.SupplierKppbcMengetahui = dbData.SUPPLIER_KPPBC_NAME;
                }
            }
            else
            {
                rc.Detail.SupplierKppbcMengetahui = dbData.SUPPLIER_KPPBC_NAME;
            }
            
            string supplierPortName;
            if (string.IsNullOrEmpty(dbData.SUPPLIER_PORT_NAME))
                supplierPortName = "-";
            else
                supplierPortName = dbData.SUPPLIER_PORT_NAME.ToLower() == "none" ? "-" : dbData.SUPPLIER_PORT_NAME;

            rc.Detail.SupplierPortName = supplierPortName;
            //rc.Detail.PrintedDate = DateReportString(DateTime.Now);
            rc.Detail.PrintedDate = dbData.REPORTED_ON.HasValue
                ? DateReportString(dbData.REPORTED_ON.Value)
                : string.Empty;
            rc.Detail.ProdPlanPeriode = SetPeriod(dbData.PLAN_PROD_FROM.Value.Month, dbData.PLAN_PROD_FROM.Value.Year,
                dbData.PLAN_PROD_TO.Value.Month, dbData.PLAN_PROD_TO.Value.Year);
            rc.Detail.Lack1Periode = SetPeriod(dbData.LACK1_FROM_MONTH.Value, dbData.LACK1_FROM_YEAR.Value,
                dbData.LACK1_TO_MONTH.Value, dbData.LACK1_TO_YEAR.Value);

            //Set ProdPlan

            //proses prodplan
            var prodPlantList = dbData.PBCK1_PROD_PLAN;
            if (dbData.PLAN_PROD_FROM.HasValue && dbData.PLAN_PROD_TO.HasValue)
            {
                prodPlantList =
                    prodPlantList.Where(
                        c =>
                            dbData.PLAN_PROD_TO != null && (dbData.PLAN_PROD_FROM != null && (c.MONTH.HasValue && c.MONTH.Value >= dbData.PLAN_PROD_FROM.Value.Month &&
                                                                                              c.MONTH.Value <= dbData.PLAN_PROD_TO.Value.Year))).ToList();
            }

            rc.ProdPlanList = Mapper.Map<List<Pbck1ReportProdPlanDto>>(prodPlantList).ToList();
            rc = SetPbck1ProdPlanList(rc);

            //set realisasi P3BKC
            rc = SetPbck1RealizationList(rc, new Lack1GetPbck1RealizationListParamInput()
            {
                MonthFrom = dbData.LACK1_FROM_MONTH.Value,
                MonthTo = dbData.LACK1_TO_MONTH.Value,
                Year = dbData.LACK1_TO_YEAR.Value,
                NppbkcId = dbData.NPPBKC_ID,
                SupplierPlantId = dbData.SUPPLIER_PLANT_WERKS,
                ExcisableGoodsTypeId = dbData.EXC_GOOD_TYP
            });
            
            //set header footer data by CompanyCode and FormTypeId
            var headerFooterData = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput()
            {
                FormTypeId = Enums.FormType.PBCK1,
                CompanyCode = dbData.NPPBKC_BUKRS
            });

            rc.HeaderFooter = headerFooterData;

            return rc;
        }

        private Pbck1ReportDto SetPbck1ProdPlanList(Pbck1ReportDto reportData)
        {
            var prodPlanList = reportData.ProdPlanList;
            var monthList = _monthBll.GetAll();
            
            var monthNotInRealizationData = from x in monthList
                                            where !(prodPlanList.Select(d => d.MonthId).ToList().Contains(x.MONTH_ID))
                                            select x;

            prodPlanList.AddRange(monthNotInRealizationData.Select(month => new Pbck1ReportProdPlanDto()
            {
                MonthId = month.MONTH_ID,
                MonthName = month.MONTH_NAME_IND,
                ProdTypeCode = "-",
                ProdTypeName = "-",
                ProdAlias = "-",
                Amount = null,
                BkcRequired = null,
                BkcRequiredUomId = string.Empty,
                BkcRequiredUomName = string.Empty
            }));
            
            //set summary
            var groupedData = prodPlanList.Where(c => c.Amount.HasValue).GroupBy(p => new
            {
                p.ProdTypeCode,
                p.ProdTypeName,
                p.ProdAlias,
                p.Amount,
                p.BkcRequired
            }).Select(g => new Pbck1ReportSummaryProdPlanDto()
            {
                ProdTypeCode = g.Key.ProdTypeCode,
                ProdTypeName = g.Key.ProdTypeName,
                ProdAlias = g.Key.ProdAlias,
                TotalAmount = g.Sum(p => p.Amount.HasValue ? p.Amount.Value : 0),
                TotalBkc = g.Sum(p => p.BkcRequired.HasValue ? p.BkcRequired.Value : 0)
            });

            var totalAmount = groupedData.GroupBy(p => new
            {
                p.ProdAlias
            }).Select(g => new
            {
                ProdAlias = g.Key.ProdAlias,
                ProdName = _prodTypeBll.GetByAlias(g.Key.ProdAlias).PRODUCT_TYPE,
                TotalAmount = g.Sum(p => p.TotalAmount)
            });

            var uomAmount = "Kilogram";
            if (reportData.Detail.ConvertedUomId.ToLower() == "btg")
            {
                uomAmount = "Batang";
            }
            else if (reportData.Detail.ConvertedUomId.ToLower() == "l")
            {
                uomAmount = "Liter";
            }
            else if (reportData.Detail.ConvertedUomId.ToLower() == "g" || reportData.Detail.ConvertedUomId.ToLower() == "kg")
            {
                uomAmount = "Kg";
            }

            reportData.Detail.ProductConvertedOutputs = string.Join(Environment.NewLine,
                totalAmount.Select(d => String.Format("{0:n}", reportData.Detail.ConvertedUomId.ToLower() == "g" ? d.TotalAmount / 1000 : d.TotalAmount)
                        + " " + uomAmount + " " 
                        + d.ProdName + " (" + d.ProdAlias + ")").ToArray());

            reportData.SummaryProdPlantList = groupedData.ToList();
            reportData.ProdPlanList = prodPlanList.OrderBy(o => o.MonthId).ToList();
            return reportData;
        }

        private Pbck1ReportDto SetPbck1RealizationList(Pbck1ReportDto reportData, Lack1GetPbck1RealizationListParamInput input)
        {
            var rc = new List<Pbck1RealisasiP3BkcDto>();
            var summaryProdList = new List<Pbck1RealisasiProductionDetailDto>();
            var monthList = _monthBll.GetAll();
            var realizationData = _lack1Bll.GetPbck1RealizationList(input);

            if (realizationData == null || realizationData.Count <= 0) return reportData;
            
            foreach (var lack1 in realizationData)
            {
                var item = Mapper.Map<Pbck1RealisasiP3BkcDto>(lack1);
                var monthData = monthList.FirstOrDefault(c => c.MONTH_ID == lack1.PeriodMonth);
                if (monthData == null) continue;
                item.Bulan = monthData.MONTH_NAME_IND;
                item.BulanId = monthData.MONTH_ID;
                item.ProductionList = new List<Pbck1RealisasiProductionDetailDto>();
                //set ExcisableGoodsType by ProdCode
                foreach (var prod in lack1.Lack1ProductionDetail)
                {
                    var toInsert = Mapper.Map<Pbck1RealisasiProductionDetailDto>(prod);
                    var excisableGoodsType =
                        _brandRegistrationBll.GetGoodTypeByProdCodeInBrandRegistration(prod.PROD_CODE);
                    if (excisableGoodsType == null) continue;
                    toInsert.ExcisableGoodsTypeDesc = excisableGoodsType.EXT_TYP_DESC;
                    toInsert.ExcisableGoodsTypeId = excisableGoodsType.EXC_GOOD_TYP;
                    summaryProdList.Add(toInsert);
                    item.ProductionList.Add(toInsert);
                }
                rc.Add(item);
            }

            var maxData = rc.OrderBy(o => o.BulanId).LastOrDefault();

            decimal? latestSaldo = null;
            int? monthId = null;

            if (maxData != null)
            {
                //got the last data
                latestSaldo = maxData.SaldoAkhir;
                monthId = maxData.BulanId;
            }

            var monthNotInRealizationData = from x in monthList
                where !(realizationData.Select(d => d.PeriodMonth).ToList().Contains(x.MONTH_ID))
                select x;

            rc.AddRange(monthNotInRealizationData.Select(month => new Pbck1RealisasiP3BkcDto()
            {
                Bulan = month.MONTH_NAME_IND,
                BulanId = month.MONTH_ID,
                ProductionList = new List<Pbck1RealisasiProductionDetailDto>(),
                SaldoAkhir = null,
                SaldoAwal = monthId.HasValue && (monthId.Value + 1) == month.MONTH_ID ? latestSaldo : null,
                Penggunaan = null,
                Pemasukan = null,
                Lack1UomId = string.Empty,
                Lack1UomName = string.Empty
            }));

            rc = rc.OrderBy(o => o.BulanId).ToList();

            var selectFirstData = summaryProdList.FirstOrDefault(c => !string.IsNullOrEmpty(c.ExcisableGoodsTypeId));
            if (selectFirstData != null)
            {
                reportData.Detail.RealisasiBkcExcisableGoodsTypeDesc = selectFirstData.ExcisableGoodsTypeDesc;
                reportData.Detail.RealisasiBkcExcisableGoodsTypeId = selectFirstData.ExcisableGoodsTypeId;
            }

            var bkcUomSelected = summaryProdList.FirstOrDefault(c => !string.IsNullOrEmpty(c.UomId));
            if (bkcUomSelected != null)
            {
                reportData.Detail.RealisasiBkcUomId = bkcUomSelected.UomId;
            }

            var realisasiUomData = realizationData.FirstOrDefault(c => !string.IsNullOrEmpty(c.Lack1UomId));
            if (realisasiUomData != null)
            {
                reportData.Detail.RealisasiUomId = realisasiUomData.Lack1UomId;
                reportData.Detail.RealisasiUomDesc = realisasiUomData.Lack1UomName;
            }

            reportData.RealisasiP3Bkc = rc;

            var groupedData = summaryProdList.GroupBy(p => new
            {
                p.ProductCode,
                p.ProductAlias,
                p.ProductType,
                p.Amount
            }).Select(g => new Pbck1SummaryRealisasiProductionDetailDto()
            {
                ProductCode = g.Key.ProductCode,
                ProductType = g.Key.ProductType,
                ProductAlias = g.Key.ProductAlias,
                Total = g.Sum(p => p.Amount != null ? p.Amount.Value : 0)
            });
            reportData.SummaryRealisasiP3Bkc = groupedData.ToList();

            return reportData;
            
        }

        private string DateReportString(DateTime dt)
        {
            var monthPeriodFrom = _monthBll.GetMonth(dt.Month);
            return dt.ToString("dd") + " " + monthPeriodFrom.MONTH_NAME_IND +
                                   " " + dt.ToString("yyyy");
        }

        private string SetPeriod(int startMonth, int startYear, int endMonth, int endYear)
        {
            var month1 = GetMonthName(startMonth); // _monthBll.GetMonth(startMonth);
            var month2 = GetMonthName(endMonth); //_monthBll.GetMonth(endMonth);
            //return month1 + " " + startYear + " - " + month2 + " " + endYear;
            return month1 + " " + " - " + month2 + " " + endYear;
        }

        private string GetMonthName(int month)
        {
            return _monthBll.GetMonth(month).MONTH_NAME_IND;
        }


        private Pbck1MailNotification ProsesMailNotificationBody(Pbck1Dto pbck1Data, Enums.ActionType actionType, String comment)
        {
            var bodyMail = new StringBuilder();
            var rc = new Pbck1MailNotification();
            var rejected = _workflowHistoryBll.RejectedStatusByDocumentNumber(new GetByFormTypeAndFormIdInput() { FormId = pbck1Data.Pbck1Id, FormType = Enums.FormType.PBCK1});
            var poaList = _poaBll.GetPoaByNppbkcIdAndMainPlant(pbck1Data.NppbkcId);

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            //rc.Subject = "PBCK-1 " + pbck1Data.Pbck1Number + " is " + EnumHelper.GetDescription(pbck1Data.Status);
            rc.Subject = "PBCK-1 " + pbck1Data.Pbck1Number + " is " + EnumHelper.GetDescription(pbck1Data.Status);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + pbck1Data.NppbkcCompanyCode + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + pbck1Data.NppbkcId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + pbck1Data.Pbck1Number + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : PBCK-1</td></tr>");
            bodyMail.AppendLine();
            if (actionType == Enums.ActionType.Reject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/Pbck1/Edit/" + pbck1Data.Pbck1Id + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");


            switch (actionType)
            {
                case Enums.ActionType.Submit:
                    if (pbck1Data.Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (rejected != null)
                        {
                            if (pbck1Data.ApprovedByPoaId != null)
                            {
                                rc.To.Add(_userBll.GetUserById(pbck1Data.ApprovedByPoaId).EMAIL);
                            }
                            else {
                                rc.To.Add(_userBll.GetUserById(rejected.ACTION_BY).EMAIL);
                            }
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }
                        
                        rc.CC.Add(_userBll.GetUserById(pbck1Data.CreatedById).EMAIL);
                    }
                    else if (pbck1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var poaData = _poaBll.GetActivePoaById(pbck1Data.CreatedById);
                        rc.To.Add(GetManagerEmail(pbck1Data.CreatedById));
                        rc.CC.Add(_userBll.GetUserById(poaData.POA_ID).EMAIL);
                        
                        foreach (var poaDto in poaList)
                        {
                            if (poaData.POA_ID != poaDto.POA_ID)
                                rc.CC.Add(_userBll.GetUserById(poaDto.POA_ID).EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    if (pbck1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(pbck1Data.ApprovedByPoaId));

                        if (rejected != null)
                        {
                            rc.CC.Add(_userBll.GetUserById(pbck1Data.ApprovedByPoaId).EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.CC.Add(_userBll.GetUserById(poaDto.POA_ID).EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(pbck1Data.CreatedById).EMAIL);

                    }
                    else if (pbck1Data.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetActivePoaById(pbck1Data.CreatedById);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(_userBll.GetUserById(poaData.POA_ID).EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck1Data.CreatedById));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(pbck1Data.CreatedById);
                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(_userBll.GetUserById(pbck1Data.ApprovedByPoaId).EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck1Data.ApprovedByPoaId));
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(pbck1Data.CreatedById);
                    var poaData2 = _poaBll.GetActivePoaById(pbck1Data.CreatedById);

                    if (pbck1Data.ApprovedByPoaId != null || poaData2 != null)
                    {
                        if (poaData2 == null)
                        {
                            var poa = _poaBll.GetById(pbck1Data.ApprovedByPoaId);
                            rc.To.Add(userDetail.EMAIL);
                            rc.CC.Add(_userBll.GetUserById(poa.POA_ID).EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck1Data.ApprovedByPoaId));
                        }
                        else {
                            rc.To.Add(_userBll.GetUserById(poaData2.POA_ID).EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck1Data.CreatedById));
                        }
                    }
                    else {
                        rc.To.Add(userDetail.EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            rc.CC.Add(_userBll.GetUserById(poaDto.POA_ID).EMAIL);
                        }
                    }

                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovApprove:
                    var poaData3 = _poaBll.GetActivePoaById(pbck1Data.CreatedById);
                        if (poaData3 != null)
                        {
                            //creator is poa user
                            rc.CC.Add(GetManagerEmail(pbck1Data.CreatedById));
                            rc.To.Add(_userBll.GetUserById(poaData3.POA_ID).EMAIL);
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(pbck1Data.CreatedById);
                            rc.CC.Add(_userBll.GetUserById(pbck1Data.ApprovedByPoaId).EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck1Data.ApprovedByPoaId));
                            rc.To.Add(userData.EMAIL);
                        }
                        rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovPartialApprove:
                    var poaData4 = _poaBll.GetActivePoaById(pbck1Data.CreatedById);
                        if (poaData4 != null)
                        {
                            //creator is poa user
                            rc.CC.Add(GetManagerEmail(pbck1Data.CreatedById));
                            rc.To.Add(_userBll.GetUserById(poaData4.POA_ID).EMAIL);
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(pbck1Data.CreatedById);
                            rc.CC.Add(_userBll.GetUserById(pbck1Data.ApprovedByPoaId).EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck1Data.ApprovedByPoaId));
                            rc.To.Add(userData.EMAIL);
                        }
                        rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovReject:
                    var poaData5 = _poaBll.GetActivePoaById(pbck1Data.CreatedById);
                        if (poaData5 != null)
                        {
                            //creator is poa user
                            rc.CC.Add(GetManagerEmail(pbck1Data.CreatedById));
                            rc.To.Add(_userBll.GetUserById(poaData5.POA_ID).EMAIL);
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(pbck1Data.CreatedById);
                            rc.CC.Add(_userBll.GetUserById(pbck1Data.ApprovedByPoaId).EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck1Data.ApprovedByPoaId));
                            rc.To.Add(userData.EMAIL);
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

        private class Pbck1MailNotification
        {
            public Pbck1MailNotification()
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

        public Pbck1Dto GetByDocumentNumber(string documentNumber)
        {
            includeTables += ", PBCK12, PBCK11, PBCK1_PROD_CONVERTER, PBCK1_PROD_PLAN, PBCK1_PROD_PLAN.MONTH1, PBCK1_PROD_PLAN.UOM, PBCK1_PROD_CONVERTER.UOM, PBCK1_DECREE_DOC";
            var dbData = _repository.Get(c => c.NUMBER == documentNumber, null, includeTables).FirstOrDefault();
            var mapResult = Mapper.Map<Pbck1Dto>(dbData);
            if (dbData != null)
            {
                mapResult.Pbck1Parent = Mapper.Map<Pbck1Dto>(dbData.PBCK12);
                mapResult.Pbck1Childs = Mapper.Map<List<Pbck1Dto>>(dbData.PBCK11);
            }
            return mapResult;
        }

        public List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbkByCompanyCode(string companyCode)
        {
            includeTables = "";
            var dbData =
                _repository.Get(c => !string.IsNullOrEmpty(c.NPPBKC_BUKRS) && c.NPPBKC_BUKRS == companyCode, null,
                    includeTables);
            if (dbData == null)
                return null;

            var nppbkcList = Mapper.Map<List<ZAIDM_EX_NPPBKCCompositeDto>>(dbData.ToList());

            return nppbkcList.DistinctBy(c => c.NPPBKC_ID).ToList();

        }

        public void UpdateReportedOn(Pbck1UpdateReportedOn input)
        {
            PBCK1 dbData = _repository.Get(c => c.PBCK1_ID == input.Id, null, includeTables).FirstOrDefault();
            dbData.REPORTED_ON = input.ReportedOn;

            ChangeDocNumberByReportedOn(dbData);

            _uow.SaveChanges();
        }

        public List<Pbck1Dto> GetAllPbck1ByPbck1Ref(int pbckRef)
        {
            var dbData = _repository.Get(p => (p.PBCK1_ID == pbckRef || p.PBCK1_REF == pbckRef) && p.STATUS == Enums.DocumentStatus.Completed);

            return Mapper.Map<List<Pbck1Dto>>(dbData);
        }

        public List<Pbck1Dto> GetPbck1CompletedDocumentByPlant(string plant)
        {
            var dbData =
                _repository.Get(p => p.STATUS == Enums.DocumentStatus.Completed && p.SUPPLIER_PLANT_WERKS == plant);

            return Mapper.Map<List<Pbck1Dto>>(dbData);
        }



        public List<ZAIDM_EX_GOODTYPCompositeDto> GetGoodsTypeByNppbkcId(string nppbkcId)
        {
            includeTables = "";
            var dbData =
                _repository.Get(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && c.NPPBKC_ID == nppbkcId, null,
                    includeTables);
            if (dbData == null)
                return null;

            var nppbkcList = Mapper.Map<List<ZAIDM_EX_GOODTYPCompositeDto>>(dbData.ToList());

            return nppbkcList.DistinctBy(c => c.EXC_GOOD_TYP).ToList();
        }

        public List<T001WCompositeDto> GetSupplierPlantByParam(Pbck1GetSupplierPlantByParamInput input)
        {
            includeTables = "";
            var dbData =
                _repository.Get(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && c.NPPBKC_ID == input.NppbkcId && !string.IsNullOrEmpty(c.EXC_GOOD_TYP)
                    && c.EXC_GOOD_TYP == input.ExciseableGoodsTypeId, null,
                    includeTables);
            if (dbData == null)
                return null;

            var nppbkcList = Mapper.Map<List<T001WCompositeDto>>(dbData.ToList());

            return nppbkcList.DistinctBy(c => c.WERKS).ToList();
        }


        public List<Pbck1Dto> GetPbck1CompletedDocumentByPlantAndSubmissionDate(string plantId, string plantNppbkcId, DateTime? submissionDate, string destPlantNppbkcId, List<string> goodtypes)
        {

            var dbData =
                _repository.Get(p => p.STATUS == Enums.DocumentStatus.Completed && p.SUPPLIER_PLANT_WERKS == plantId && p.SUPPLIER_NPPBKC_ID == plantNppbkcId
                 && p.PERIOD_FROM <= submissionDate && p.PERIOD_TO >= submissionDate && p.NPPBKC_ID == destPlantNppbkcId && goodtypes.Contains(p.EXC_GOOD_TYP)).OrderByDescending(p => p.DECREE_DATE);

            return Mapper.Map<List<Pbck1Dto>>(dbData);

        }

        public List<Pbck1Dto> GetPbck1CompletedDocumentByExternalAndSubmissionDate(string exSupplierId, string exSupplierNppbkcId, DateTime? submissionDate, string destPlantNppbkcId, List<string> goodtypes)
        {

            var dbData =
                _repository.Get(p => p.STATUS == Enums.DocumentStatus.Completed && p.SUPPLIER_PLANT == exSupplierId && p.SUPPLIER_NPPBKC_ID == exSupplierNppbkcId
                 && p.PERIOD_FROM <= submissionDate && p.PERIOD_TO >= submissionDate && p.NPPBKC_ID == destPlantNppbkcId && goodtypes.Contains(p.EXC_GOOD_TYP)).OrderByDescending(p => p.DECREE_DATE);

            return Mapper.Map<List<Pbck1Dto>>(dbData);

        }

        public List<Pbck1Dto> GetByRef(int pbckId)
        {
            var dbData =
                _repository.Get(p => p.STATUS == Enums.DocumentStatus.Completed && p.PBCK1_REF == pbckId);

            return Mapper.Map<List<Pbck1Dto>>(dbData);
        }

        public string checkUniquePBCK1(Pbck1SaveInput input)
        {
            if (input.Pbck1.Pbck1Type == Enums.PBCK1Type.Additional)
                return null;

            var dbData = _repository.Get(
                p => ((input.Pbck1.Pbck1Id == null || p.PBCK1_ID != input.Pbck1.Pbck1Id) && p.STATUS != Enums.DocumentStatus.Cancelled && p.NPPBKC_ID == input.Pbck1.NppbkcId
                    && (p.PERIOD_FROM <= input.Pbck1.PeriodFrom && p.PERIOD_TO >= input.Pbck1.PeriodFrom
                    || p.PERIOD_FROM <= input.Pbck1.PeriodTo && p.PERIOD_TO >= input.Pbck1.PeriodTo || (p.PERIOD_FROM > input.Pbck1.PeriodFrom && p.PERIOD_TO < input.Pbck1.PeriodTo))
                    && ((input.Pbck1.SupplierPlantWerks != null && p.SUPPLIER_PLANT_WERKS == input.Pbck1.SupplierPlantWerks) || (input.Pbck1.SupplierPlantWerks == null && p.SUPPLIER_PLANT == input.Pbck1.SupplierPlant)) && p.EXC_GOOD_TYP == input.Pbck1.GoodType && p.PBCK1_TYPE == Enums.PBCK1Type.New)
            );

            var data = Mapper.Map<List<Pbck1Dto>>(dbData);

            if (data.Count > 0)
                return data.FirstOrDefault().Pbck1Number;

            return null;
        }

        public Pbck1Dto GetPBCK1Reference(Pbck1ReferenceSearchInput input)
        {
            var dbData = _repository.Get(
            p => p.PBCK1_TYPE == Enums.PBCK1Type.New && p.STATUS == Enums.DocumentStatus.Completed
                && p.NPPBKC_ID == input.NppbkcId
                && (p.PERIOD_FROM <= input.PeriodFrom && p.PERIOD_TO >= input.PeriodFrom
                || p.PERIOD_FROM <= input.PeriodTo && p.PERIOD_TO >= input.PeriodTo)
                && ((input.SupplierPlantWerks != "" && p.SUPPLIER_PLANT_WERKS == input.SupplierPlantWerks) || (input.SupplierPlantWerks == "" && p.SUPPLIER_PLANT == input.SupplierPlant)) && p.EXC_GOOD_TYP == input.GoodTypeId
            ).FirstOrDefault();
           
            
            var data = Mapper.Map<Pbck1Dto>(dbData);

            return data;
        }

        public List<CK5ExternalSupplierDto> GetExternalSupplierList(List<string> goodTypeList = null)
        {
            
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            queryFilter = queryFilter.And(c => string.IsNullOrEmpty(c.SUPPLIER_PLANT_WERKS));
            queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);

            if (goodTypeList != null && goodTypeList.Count > 0)
            {
                queryFilter = queryFilter.And(c => goodTypeList.Contains(c.EXC_GOOD_TYP));
            }
            var dbData =
                _repository.Get(queryFilter,null, "")
                    .GroupBy(l => new
                    {
                        l.SUPPLIER_COMPANY,
                        l.SUPPLIER_NPPBKC_ID
                    }).Select(cl => new PBCK1()
                    {
                        SUPPLIER_NPPBKC_ID = cl.First().SUPPLIER_NPPBKC_ID,
                        SUPPLIER_ADDRESS = cl.First().SUPPLIER_ADDRESS,
                        SUPPLIER_PLANT = cl.First().SUPPLIER_PLANT,
                        SUPPLIER_COMPANY = cl.First().SUPPLIER_COMPANY,
                        SUPPLIER_PORT_ID = cl.First().SUPPLIER_PORT_ID,
                        SUPPLIER_PORT_NAME = cl.First().SUPPLIER_PORT_NAME,
                        SUPPLIER_KPPBC_ID = cl.First().SUPPLIER_KPPBC_ID,
                        SUPPLIER_KPPBC_NAME = cl.First().SUPPLIER_KPPBC_NAME,
                        SUPPLIER_PHONE = cl.First().SUPPLIER_PHONE

                    }).ToList();

            var data = Mapper.Map<List<CK5ExternalSupplierDto>>(dbData);

            return data;
        }

        public List<Pbck1MonitoringMutasiDto> GetMonitoringMutasiByParam(Pbck1GetMonitoringMutasiByParamInput input)
        {
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();
           
            if (!string.IsNullOrEmpty(input.pbck1Number))
            {
                queryFilter = queryFilter.And(c => c.NUMBER == input.pbck1Number);
            }

            Func<IQueryable<PBCK1>, IOrderedQueryable<PBCK1>> orderBy = null;
            {
                if (!string.IsNullOrEmpty(input.SortOrderColumn))
                {
                    orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK1>(input.SortOrderColumn));
                }

                var dbData = _repository.Get(queryFilter, orderBy,"CK5");
                if (dbData == null)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                }
                var mapResult = Mapper.Map<List<Pbck1MonitoringMutasiDto>>(dbData.ToList());

                mapResult = GetCorrectReceivedAdditional(mapResult);

                return mapResult;
            }
        }

        private List<Pbck1MonitoringMutasiDto> GetCorrectReceivedAdditional(List<Pbck1MonitoringMutasiDto> listData)
        {
            var list = listData;

            foreach (var item in list)
            {
                var receivedAdditional = Convert.ToDecimal(0);

                var listPbckAdditional = _repository.GetByID(item.Pbck1Id).PBCK11;

                foreach (var data in listPbckAdditional)
                {
                    var additionalRec = _repository.GetByID(data.PBCK1_ID).CK5
                                            .Where(c => c.STATUS_ID != Enums.DocumentStatus.Cancelled).Sum(s => s.GRAND_TOTAL_EX);

                    receivedAdditional += additionalRec.Value;
                }

                item.ReceivedAdditional = receivedAdditional;
            }

            return list;
        }
        
        private void ChangeDocNumberByReportedOn(PBCK1 data)
        {
            var oldDocNumber = data.NUMBER;
            var splitNumber = data.NUMBER.Split('/');
            var newDocNumber = splitNumber[0] + "/" + splitNumber[1] + "/" + splitNumber[2] + "/" + 
                            MonthHelper.ConvertToRomansNumeral(data.REPORTED_ON.Value.Month) + "/" + data.REPORTED_ON.Value.Year;

            var listWorkflow = _workflowHistoryBll.GetByFormNumber(oldDocNumber);

            foreach(var item in listWorkflow)
            {
                item.FORM_NUMBER = newDocNumber;
                _workflowHistoryBll.Save(item);
            }

            data.NUMBER = newDocNumber;
        }
    }
}