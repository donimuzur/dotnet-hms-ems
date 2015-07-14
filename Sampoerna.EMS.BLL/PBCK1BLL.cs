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
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

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
        }

        public Pbck1GetByParamOutput Pbck1GetByParam(Pbck1GetByParamInput input)
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
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
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

            Func<IQueryable<PBCK1>, IOrderedQueryable<PBCK1>> orderBy = null;
            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK1>(input.SortOrderColumn));
            }

            var rc = new Pbck1GetByParamOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                var exception = new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                rc.Data = null;
                rc.ErrorCode = exception.Code;
                rc.ErrorMessage = exception.Message;
            }

            var mapResult = Mapper.Map<List<Pbck1Dto>>(dbData.ToList());

            rc.Data = mapResult;

            return rc;

        }

        public Pbck1Dto GetById(long id)
        {
            includeTables += ", PBCK12, PBCK11, PBCK1_PROD_CONVERTER, PBCK1_PROD_PLAN, PBCK1_PROD_PLAN.MONTH1, PBCK1_PROD_CONVERTER.UOM";
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
            PBCK1 dbData = null;
            if (input.Pbck1.Pbck1Id > 0)
            {

                //update
                dbData = _repository.Get(c => c.PBCK1_ID == input.Pbck1.Pbck1Id, null, includeTables).FirstOrDefault();

                //set changes history
                var origin = Mapper.Map<Pbck1Dto>(dbData);
                SetChangesHistory(origin, input.Pbck1, input.UserId);

                Mapper.Map<Pbck1Dto, PBCK1>(input.Pbck1, dbData);

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
                input.Pbck1.Status = Core.Enums.DocumentStatus.Draft;
                input.Pbck1.CreatedDate = DateTime.Now;
                dbData = new PBCK1();
                Mapper.Map<Pbck1Dto, PBCK1>(input.Pbck1, dbData);

                _repository.Insert(dbData);

            }

            var output = new SavePbck1Output();

            _uow.SaveChanges();
            output.Success = true;
            if (dbData != null)
            {
                output.Id = dbData.PBCK1_ID;
                output.Pbck1Number = dbData.NUMBER;
            }

            //set workflow history
            AddWorkflowHistory(output.Id, output.Pbck1Number, input.WorkflowActionType, input.UserId);

            _uow.SaveChanges();

            return output;

        }

        public DeletePbck1Output Delete(long id)
        {
            var output = new DeletePbck1Output();
            try
            {
                var dbData = _repository.GetByID(id);

                if (dbData == null)
                {
                    _logger.Error(new BLLException(ExceptionCodes.BLLExceptions.DataNotFound));
                    output.ErrorCode = ExceptionCodes.BLLExceptions.DataNotFound.ToString();
                    output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.DataNotFound);
                }
                else
                {
                    _repository.Delete(dbData);
                    _uow.SaveChanges();
                    output.Success = true;
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;
        }

        private void SetChangesHistory(Pbck1Dto origin, Pbck1Dto data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("PBCK1_REF", origin.Pbck1Reference.Equals(data.Pbck1Reference));
            changesData.Add("PBCK1_TYPE", origin.Pbck1Type.Equals(data.Pbck1Type));
            changesData.Add("PERIOD_FROM", origin.PeriodFrom.Equals(data.PeriodFrom));
            changesData.Add("PERIOD_TO", origin.PeriodTo.HasValue ? origin.PeriodTo.Equals(data.PeriodTo) : false);
            changesData.Add("REPORTED_ON", origin.ReportedOn.HasValue ? origin.ReportedOn.Equals(data.ReportedOn) : false);
            changesData.Add("NPPBKC_ID", origin.NppbkcId.Equals(data.NppbkcId));
            changesData.Add("EXC_GOOD_TYP", !string.IsNullOrEmpty(origin.GoodType) ? origin.GoodType.Equals(data.GoodType) : false);
            changesData.Add("SUPPLIER_PLANT", !string.IsNullOrEmpty(origin.SupplierPlant) ? origin.SupplierPlant.Equals(data.SupplierPlant) : (!string.IsNullOrEmpty(data.SupplierPlant) ? false : true));
            changesData.Add("SUPPLIER_PORT_ID", origin.SupplierPortId.HasValue ? origin.SupplierPortId.Equals(data.SupplierPortId) : (data.SupplierPortId.HasValue ? false : true));
            changesData.Add("SUPPLIER_ADDRESS", !string.IsNullOrEmpty(origin.SupplierAddress) ? origin.SupplierAddress.Equals(data.SupplierAddress) : (!string.IsNullOrEmpty(data.SupplierAddress) ? false : true));
            changesData.Add("SUPPLIER_PHONE", !string.IsNullOrEmpty(origin.SupplierPhone) ? origin.SupplierPhone.Equals(data.SupplierPhone) : (!string.IsNullOrEmpty(data.SupplierPhone) ? false : true));
            changesData.Add("PLAN_PROD_FROM", origin.PlanProdFrom.HasValue ? origin.PlanProdFrom.Equals(data.PlanProdFrom) : false);
            changesData.Add("PLAN_PROD_TO", origin.PlanProdTo.HasValue ? origin.PlanProdTo.Equals(data.PlanProdTo) : false);
            changesData.Add("REQUEST_QTY", origin.RequestQty.HasValue ? origin.RequestQty.Equals(data.RequestQty) : false);
            changesData.Add("REQUEST_QTY_UOM", !string.IsNullOrEmpty(origin.RequestQtyUomId) ? origin.RequestQtyUomId.Equals(data.RequestQtyUomId) : false);
            changesData.Add("LACK1_FROM_MONTH", origin.Lack1FromMonthId.HasValue ? origin.Lack1FromMonthId.Equals(data.Lack1FromMonthId) : false);
            changesData.Add("LACK1_FROM_YEAR", origin.Lack1FormYear.HasValue ? origin.Lack1FormYear.Equals(data.Lack1FormYear) : false);
            changesData.Add("LACK1_TO_MONTH", origin.Lack1ToMonthId.HasValue ? origin.Lack1ToMonthId.Equals(data.Lack1ToMonthId) : false);
            changesData.Add("LACK1_TO_YEAR", origin.Lack1ToYear.HasValue ? origin.Lack1ToYear.Equals(data.Lack1ToYear) : false);
            changesData.Add("STATUS", origin.Status.Equals(data.Status));
            changesData.Add("STATUS_GOV", origin.StatusGov.Equals(data.StatusGov));
            changesData.Add("QTY_APPROVED", origin.QtyApproved.HasValue ? origin.QtyApproved.Equals(data.QtyApproved) : false);
            changesData.Add("DECREE_DATE", origin.DecreeDate.HasValue ? origin.DecreeDate.Equals(data.DecreeDate) : false);
            changesData.Add("LATEST_SALDO", origin.LatestSaldo.HasValue ? origin.LatestSaldo.Equals(data.LatestSaldo) : false);
            changesData.Add("LATEST_SALDO_UOM", !string.IsNullOrEmpty(origin.LatestSaldoUomId) ? origin.LatestSaldoUomId.Equals(data.LatestSaldoUomId) : false);

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.PBCK1,
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
                            changes.NEW_VALUE = data.QtyApproved.Value.ToString("N0");
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

        private void AddWorkflowHistory(long id, string formNumber, Core.Enums.ActionType actionType, string userId)
        {
            var dbData = _workflowHistoryBll.GetByActionAndFormNumber(new GetByActionAndFormNumberInput(){ FormNumber = formNumber, ActionType = actionType});

            if (dbData == null)
            {
                dbData = new WorkflowHistoryDto()
                {
                    ACTION = actionType,
                    FORM_ID = id,
                    FORM_TYPE_ID = Core.Enums.FormType.PBKC1,
                    FORM_NUMBER = formNumber
                };
            }
            dbData.ACTION_BY = userId;
            dbData.ACTION_DATE = DateTime.Now;
            _workflowHistoryBll.Save(dbData);
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

                string uomId;
                if (!ValidateUom(output.ConverterUom, out messages, out uomId))
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }
                else
                {
                    output.ConverterUomId = uomId;
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

                if (string.IsNullOrWhiteSpace(output.BkcRequired))
                {
                    output.IsValid = false;
                    messageList.Add("BKC Required is empty");
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

        private bool ValidateUom(string uom, out List<string> message, out string uomId)
        {
            var valResult = false;
            var messageList = new List<string>();
            uomId = string.Empty;
            if (!string.IsNullOrWhiteSpace(uom))
            {
                var uomData = _uomBll.GetById(uom);
                if (uomData != null)
                {
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

    }
}
