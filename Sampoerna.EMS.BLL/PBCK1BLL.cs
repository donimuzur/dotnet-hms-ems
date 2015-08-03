using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        private string includeTables = "UOM, UOM1, MONTH, MONTH1, USER, USER1";

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
        }

        public List<Pbck1Dto> GetAllByParam(Pbck1GetByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            return Mapper.Map<List<Pbck1Dto>>(GetPbck1Data(queryFilter, input.SortOrderColumn));
        }

        public List<Pbck1Dto> GetOpenDocumentByParam(Pbck1GetOpenDocumentByParamInput input)
        {

            var queryFilter = ProcessQueryFilter(input);

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
            return queryFilter;
        }

        private List<PBCK1> GetPbck1Data(Expression<Func<PBCK1, bool>> queryFilter, string orderColumn)
        {
            Func<IQueryable<PBCK1>, IOrderedQueryable<PBCK1>> orderBy = null;
            if (!string.IsNullOrEmpty(orderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK1>(orderColumn));
            }

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

            if (input.Pbck1.Pbck1Id > 0)
            {

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
                SetChangesHistory(origin, input.Pbck1, input.UserId);

                Mapper.Map<Pbck1Dto, PBCK1>(input.Pbck1, dbData);
                dbData.PBCK1_PROD_CONVERTER = null;
                dbData.PBCK1_PROD_PLAN = null;
                dbData.PBCK1_DECREE_DOC = null;

                dbData.PBCK1_PROD_CONVERTER = Mapper.Map<List<PBCK1_PROD_CONVERTER>>(input.Pbck1.Pbck1ProdConverter);
                dbData.PBCK1_PROD_PLAN = Mapper.Map<List<PBCK1_PROD_PLAN>>(input.Pbck1.Pbck1ProdPlan);
                dbData.PBCK1_DECREE_DOC = Mapper.Map<List<PBCK1_DECREE_DOC>>(input.Pbck1.Pbck1DecreeDoc);

            }
            else
            {
                //Insert
                var generateNumberInput = new GenerateDocNumberInput()
                {
                    Year = input.Pbck1.PeriodFrom.Year,
                    Month = input.Pbck1.PeriodFrom.Month,
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

            AddWorkflowHistory(inputAddWorkflowHistory);

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

        private void SetChangesHistory(Pbck1Dto origin, Pbck1Dto data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("PBCK1_REF", origin.Pbck1Reference == data.Pbck1Reference);
            changesData.Add("PBCK1_TYPE", origin.Pbck1Type == data.Pbck1Type);
            changesData.Add("PERIOD_FROM", origin.PeriodFrom.Equals(data.PeriodFrom));
            changesData.Add("PERIOD_TO", origin.PeriodTo == data.PeriodTo);
            changesData.Add("REPORTED_ON", origin.ReportedOn == data.ReportedOn);
            changesData.Add("NPPBKC_ID", origin.NppbkcId == data.NppbkcId);
            changesData.Add("EXC_GOOD_TYP", origin.GoodType == data.GoodType);
            changesData.Add("SUPPLIER_PLANT", origin.SupplierPlant == data.SupplierPlant);
            changesData.Add("SUPPLIER_PORT_ID", origin.SupplierPortId == data.SupplierPortId);
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
                            break;
                        case "PBCK1_TYPE":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.Pbck1Type);
                            changes.NEW_VALUE = EnumHelper.GetDescription(data.Pbck1Type);
                            break;
                        case "PERIOD_FROM":
                            changes.OLD_VALUE = origin.PeriodFrom.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = data.PeriodFrom.ToString("dd MMM yyyy");
                            break;
                        case "PERIOD_TO":
                            changes.OLD_VALUE = origin.PeriodTo.HasValue
                                ? origin.PeriodTo.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.NEW_VALUE = data.PeriodTo.HasValue
                                ? data.PeriodTo.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            break;
                        case "REPORTED_ON":
                            changes.OLD_VALUE = origin.ReportedOn.HasValue
                                ? origin.ReportedOn.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.NEW_VALUE = data.ReportedOn.HasValue
                                ? data.ReportedOn.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            break;
                        case "NPPBKC_ID":
                            changes.OLD_VALUE = origin.NppbkcId;
                            changes.NEW_VALUE = data.NppbkcId;
                            break;
                        case "GOODTYPE_ID":
                            changes.OLD_VALUE = origin.GoodTypeDesc;
                            changes.NEW_VALUE = data.GoodTypeDesc;
                            break;
                        case "SUPPLIER_PLANT":
                            changes.OLD_VALUE = origin.SupplierPlant;
                            changes.NEW_VALUE = data.SupplierPlant;
                            break;
                        case "SUPPLIER_PORT_ID":
                            changes.OLD_VALUE = origin.SupplierPortName;
                            changes.NEW_VALUE = data.SupplierPortName;
                            break;
                        case "SUPPLIER_ADDRESS":
                            changes.OLD_VALUE = origin.SupplierAddress;
                            changes.NEW_VALUE = data.SupplierAddress;
                            break;
                        case "SUPPLIER_PHONE":
                            changes.OLD_VALUE = origin.SupplierPhone;
                            changes.NEW_VALUE = data.SupplierPhone;
                            break;
                        case "PLAN_PROD_FROM":
                            changes.OLD_VALUE = origin.PlanProdFrom.HasValue ? origin.PlanProdFrom.Value.ToString("dd MMM yyyy") : "NULL";
                            changes.NEW_VALUE = data.PlanProdFrom.HasValue ? data.PlanProdFrom.Value.ToString("dd MMM yyyy") : "NULL";
                            break;
                        case "PLAN_PROD_TO":
                            changes.OLD_VALUE = origin.PlanProdTo.HasValue ? origin.PlanProdTo.Value.ToString("dd MMM yyyy") : "NULL";
                            changes.NEW_VALUE = data.PlanProdTo.HasValue ? data.PlanProdTo.Value.ToString("dd MMM yyyy") : "NULL";
                            break;
                        case "REQUEST_QTY":
                            changes.OLD_VALUE = origin.RequestQty.HasValue ? origin.RequestQty.Value.ToString("N0") : "NULL";
                            changes.NEW_VALUE = data.RequestQty.HasValue ? data.RequestQty.Value.ToString("N0") : "NULL";
                            break;
                        case "REQUEST_QTY_UOM":
                            changes.OLD_VALUE = !string.IsNullOrEmpty(origin.RequestQtyUomId) ? origin.RequestQtyUomName : "NULL";
                            changes.NEW_VALUE = data.RequestQtyUomName;
                            break;
                        case "LACK1_FROM_MONTH":
                            changes.OLD_VALUE = origin.Lack1FromMonthId.HasValue ? origin.Lack1FromMonthName : "NULL";
                            changes.NEW_VALUE = data.Lack1FromMonthName;
                            break;
                        case "LACK1_FROM_YEAR":
                            changes.OLD_VALUE = origin.Lack1FormYear.HasValue ? origin.Lack1FormYear.Value.ToString("N0") : "NULL";
                            changes.NEW_VALUE = data.Lack1FormYear.Value.ToString("N0");
                            break;
                        case "LACK1_TO_MONTH":
                            changes.OLD_VALUE = origin.Lack1ToMonthId.HasValue ? origin.Lack1ToMonthName : "NULL";
                            changes.NEW_VALUE = data.Lack1ToMonthName;
                            break;
                        case "LACK1_TO_YEAR":
                            changes.OLD_VALUE = origin.Lack1ToYear.HasValue ? origin.Lack1ToYear.Value.ToString("N0") : "NULL";
                            changes.NEW_VALUE = data.Lack1ToYear.Value.ToString("N0");
                            break;
                        case "STATUS":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.Status);
                            changes.NEW_VALUE = EnumHelper.GetDescription(data.Status);
                            break;
                        case "STATUS_GOV":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.StatusGov);
                            changes.NEW_VALUE = EnumHelper.GetDescription(data.StatusGov);
                            break;
                        case "QTY_APPROVED":
                            changes.OLD_VALUE = origin.QtyApproved.HasValue
                                ? origin.QtyApproved.Value.ToString("N0")
                                : "NULL";
                            changes.NEW_VALUE = data.QtyApproved.HasValue ? data.QtyApproved.Value.ToString("N0") : "NULL";
                            break;
                        case "DECREE_DATE":
                            changes.OLD_VALUE = origin.DecreeDate.HasValue
                                ? origin.DecreeDate.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            changes.NEW_VALUE = data.DecreeDate.HasValue
                                ? data.DecreeDate.Value.ToString("dd MMM yyyy")
                                : "NULL";
                            break;
                        case "LATEST_SALDO":
                            changes.OLD_VALUE = origin.LatestSaldo.HasValue
                                ? origin.LatestSaldo.Value.ToString("N0")
                                : "NULL";
                            changes.NEW_VALUE = data.LatestSaldo.HasValue
                                ? data.LatestSaldo.Value.ToString("N0")
                                : "NULL";
                            break;
                        case "LATEST_SALDO_UOM":
                            changes.OLD_VALUE = !string.IsNullOrEmpty(origin.LatestSaldoUomId)
                                ? origin.LatestSaldoUomName
                                : "NULL";
                            changes.NEW_VALUE = data.LatestSaldoUomName;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        public string GetPbckNumberById(long id)
        {
            var dbData = _repository.GetByID(id);
            return dbData == null ? string.Empty : dbData.NUMBER;
        }

        public List<Pbck1ProdConverterOutput> ValidatePbck1ProdConverterUpload(IEnumerable<Pbck1ProdConverterInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<Pbck1ProdConverterOutput>();
            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                var output = Mapper.Map<Pbck1ProdConverterOutput>(inputItem);
                output.IsValid = true;

                //Product Code Validation
                #region -------------- Product Code Validation --------------
                List<string> messages;
                ZAIDM_EX_PRODTYP prodTypeData = null;
                if (ValidateProductCode(output.ProductCode, out messages, out prodTypeData))
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
                if (!ValidateUom(output.ConverterUom, out messages, out uomName, out uomId))
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

        public List<Pbck1ProdPlanOutput> ValidatePbck1ProdPlanUpload(IEnumerable<Pbck1ProdPlanInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<Pbck1ProdPlanOutput>();
            foreach (var inputItem in inputs) //   <--- go back to here --------+
            {
                messageList.Clear();

                var output = Mapper.Map<Pbck1ProdPlanOutput>(inputItem);
                output.IsValid = true;

                #region ------------- Product Code Validation ----------
                List<string> messages;
                ZAIDM_EX_PRODTYP prodTypeData = null;
                if (ValidateProductCode(output.ProductCode, out messages, out prodTypeData))
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

                #region -------------- Month Validation -----------

                string monthName;

                if (ValidateMonth(output.Month, out messages, out monthName))
                {
                    output.MonthName = monthName;
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
                if (!ValidateUom(output.BkcRequiredUomId, out messages, out uomName, out uomId))
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
            return outputList;
        }

        private bool ValidateProductCode(string productCode, out List<string> message, out ZAIDM_EX_PRODTYP productData)
        {
            productData = null;
            var valResult = false;
            var messageList = new List<string>();
            #region ------------Product Code Validation-------------
            if (!string.IsNullOrWhiteSpace(productCode))
            {

                productData = _prodTypeBll.GetByCode(productCode);
                if (productData == null)
                {
                    messageList.Add("ProductCode not valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("ProductCode is empty");
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
                    messageList.Add("Month is not valid");
                }
                else
                {
                    //valid, get month name
                    var monthData = _monthBll.GetMonth(monthNumber);
                    if (monthData == null)
                    {
                        messageList.Add("Month is not valid");
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

        private bool ValidateUom(string uom, out List<string> message, out string uomName, out string uomId)
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
            }
            else
            {
                messageList.Add("UOM is empty");
            }

            message = messageList;

            return valResult;
        }

        public void Pbck1Workflow(Pbck1WorkflowDocumentInput input)
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
            dbData.APPROVED_BY_POA = input.UserId;
            dbData.APPROVED_DATE_POA = DateTime.Now;
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

        private void RejectDocument(Pbck1WorkflowDocumentInput input)
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
                                        DocumentNumber = dbData.NUMBER,
                                        NppbkcId = dbData.NPPBKC_ID
                                    });

            if (!isOperationAllow)
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

        private void GovApproveDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
            WorkflowStatusGovAddChanges(input, dbData.STATUS_GOV, Enums.DocumentStatusGov.FullApproved);

            dbData.STATUS = Enums.DocumentStatus.Completed;

            //todo: update remaining quota and necessary data
            dbData.PBCK1_DECREE_DOC = null;
            dbData.QTY_APPROVED = input.AdditionalDocumentData.QtyApproved;
            dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
            dbData.PBCK1_DECREE_DOC = Mapper.Map<List<PBCK1_DECREE_DOC>>(input.AdditionalDocumentData.Pbck1DecreeDoc);
            dbData.STATUS_GOV = Enums.DocumentStatusGov.FullApproved;

            dbData.APPROVED_BY_POA = input.UserId;
            dbData.APPROVED_DATE_POA = DateTime.Now;

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.NUMBER;

            AddWorkflowHistory(input);

        }

        private void GovPartialApproveDocument(Pbck1WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
            WorkflowStatusGovAddChanges(input, dbData.STATUS_GOV, Enums.DocumentStatusGov.PartialApproved);

            //input.ActionType = Enums.ActionType.Completed;
            input.DocumentNumber = dbData.NUMBER;

            //todo: update remaining quota and necessary data
            dbData.PBCK1_DECREE_DOC = null;
            dbData.STATUS = Enums.DocumentStatus.Completed;
            dbData.QTY_APPROVED = input.AdditionalDocumentData.QtyApproved;
            dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
            dbData.PBCK1_DECREE_DOC = Mapper.Map<List<PBCK1_DECREE_DOC>>(input.AdditionalDocumentData.Pbck1DecreeDoc);
            dbData.STATUS_GOV = Enums.DocumentStatusGov.PartialApproved;

            dbData.APPROVED_BY_POA = input.UserId;
            dbData.APPROVED_DATE_POA = DateTime.Now;

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
            //WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Draft);
            WorkflowStatusGovAddChanges(input, dbData.STATUS_GOV, Enums.DocumentStatusGov.Rejected);

            dbData.STATUS_GOV = Enums.DocumentStatusGov.Rejected;

            dbData.APPROVED_BY_POA = input.UserId;
            dbData.APPROVED_DATE_POA = DateTime.Now;

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
                FIELD_NAME = "STATUS",
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
                FIELD_NAME = "STATUS_GOV",
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
            var to = "irmansulaeman41@gmail.com";
            var subject = "this is subject for " + input.DocumentNumber;
            var body = "this is body message for " + input.DocumentNumber;
            var from = "a@gmail.com";

            _messageService.SendEmail(to, subject, body, true);
        }

        #endregion

        #region Summary Reports 
        
        public List<Pbck1SummaryReportDto> GetSummaryReportByParam(Pbck1GetSummaryReportByParamInput input)
        {
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);

            if (input.YearFrom.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_FROM.HasValue && c.PERIOD_FROM.Value.Year >= input.YearFrom.Value);
            if (input.YearTo.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_TO.HasValue && c.PERIOD_TO.Value.Year >= input.YearTo.Value);
            if (!string.IsNullOrEmpty(input.CompanyCode))
                queryFilter = queryFilter.And(c => c.NPPBKC_BUKRS == input.CompanyCode);
            if (!string.IsNullOrEmpty(input.NppbkcId))
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);

            var pbck1Data = GetPbck1Data(queryFilter, input.SortOrderColumn);

            if(pbck1Data == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //todo: ask the cleanest way
            var rc = Mapper.Map<List<Pbck1SummaryReportDto>>(pbck1Data);
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
                    queryFilter.And(c => c.PERIOD_TO.HasValue && c.PERIOD_TO.Value.Year >= input.YearTo.Value);

            if (!string.IsNullOrEmpty(input.CompanyCode))
                queryFilter = queryFilter.And(c => c.NPPBKC_BUKRS == input.CompanyCode);

            if (!string.IsNullOrEmpty(input.NppbkcId))
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);

            var pbck1Data = GetPbck1Data(queryFilter, input.SortOrderColumn);

            if (pbck1Data == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<List<Pbck1MonitoringUsageDto>>(pbck1Data);
        }

        #endregion

    }
}
