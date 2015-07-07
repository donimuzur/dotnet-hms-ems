﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
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

        private string includeTables = "ZAIDM_EX_GOODTYP, UOM, UOM1, ZAIDM_EX_NPPBKC, SUPPLIER_PORT, MONTH, MONTH1, USER, ZAIDM_EX_NPPBKC.T1001";

        public PBCK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1>();
            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
        }

        public List<Pbck1> GetPBCK1ByParam(Pbck1GetByParamInput input)
        {
            
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            if (input.NppbkcId.HasValue)
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID.Value == input.NppbkcId.Value);
            }

            if (input.Pbck1Type.HasValue)
            {
                queryFilter = queryFilter.And(c => c.PBCK1_TYPE == input.Pbck1Type.Value);
            }

            if (input.Poa.HasValue)
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY.HasValue && c.APPROVED_BY.Value == input.Poa.Value);
            }

            if (input.Creator.HasValue)
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY.HasValue && c.CREATED_BY.Value == input.Creator.Value);
            }

            if (input.GoodTypeId.HasValue)
            {
                queryFilter = queryFilter.And(c => c.GOODTYPE_ID.HasValue && c.GOODTYPE_ID.Value == input.GoodTypeId.Value);
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

            var rc = _repository.Get(queryFilter, orderBy, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<Pbck1>>(rc.ToList());

            return mapResult;

        }

        public Pbck1 GetById(long id)
        {
            includeTables += ", PBCK12, PBCK11, PBCK1_PROD_CONVERTER, PBCK1_PROD_PLAN";
            var dbData = _repository.Get(c => c.PBCK1_ID == id, null, includeTables).FirstOrDefault();
            var mapResult = Mapper.Map<Pbck1>(dbData);
            if (dbData != null)
            {
                mapResult.Pbck1Parent = Mapper.Map<Pbck1>(dbData.PBCK12);
                mapResult.Pbck1Childs = Mapper.Map<List<Pbck1>>(dbData.PBCK11);
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
                var origin = Mapper.Map<Pbck1>(dbData);
                SetChangesHistory(origin, input.Pbck1, input.UserId);

                Mapper.Map<Pbck1, PBCK1>(input.Pbck1, dbData);

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
                Mapper.Map<Pbck1, PBCK1>(input.Pbck1, dbData);

                _repository.Insert(dbData);

            }

            var output = new SavePbck1Output();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                if (dbData != null) output.Id = dbData.PBCK1_ID;

                //set workflow history
                AddWorkflowHistory(output.Id, input.WorkflowActionType, input.UserId);

                _uow.SaveChanges();

            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;

        }

        public DeletePBCK1Output Delete(long id)
        {
            var output = new DeletePBCK1Output();
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

        private void SetChangesHistory(Pbck1 origin, Pbck1 data, int userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("PBCK1_REF", origin.Pbck1Reference.Equals(data.Pbck1Reference));
            changesData.Add("PBCK1_TYPE", origin.Pbck1Type.Equals(data.Pbck1Type));
            changesData.Add("PERIOD_FROM", origin.PeriodFrom.Equals(data.PeriodFrom));
            changesData.Add("PERIOD_TO", origin.PeriodTo.Equals(data.PeriodTo));
            changesData.Add("REPORTED_ON", origin.ReportedOn.Equals(data.ReportedOn));
            changesData.Add("NPPBKC_ID", origin.NppbkcId.Equals(data.NppbkcId));
            changesData.Add("GOODTYPE_ID", origin.GoodTypeId.Equals(data.GoodTypeId));
            changesData.Add("SUPPLIER_PLANT", origin.SupplierPlant.Equals(data.SupplierPlant));
            changesData.Add("SUPPLIER_PORT_ID", origin.SupplierPortId.Equals(data.SupplierPortId));
            changesData.Add("SUPPLIER_ADDRESS", origin.SupplierAddress.Equals(data.SupplierAddress));
            changesData.Add("SUPPLIER_PHONE", origin.SupplierPhone.Equals(data.SupplierPhone));
            changesData.Add("PLAN_PROD_FROM", origin.PlanProdFrom.Equals(data.PlanProdFrom));
            changesData.Add("PLAN_PROD_TO", origin.PlanProdTo.Equals(data.PlanProdTo));
            changesData.Add("REQUEST_QTY", origin.RequestQty.Equals(data.RequestQty));
            changesData.Add("REQUEST_QTY_UOM", origin.RequestQtyUomId.Equals(data.RequestQtyUomId));
            changesData.Add("LACK1_FROM_MONTH", origin.Lack1FromMonthId.Equals(data.Lack1FromMonthId));
            changesData.Add("LACK1_FROM_YEAR", origin.Lack1FormYear.Equals(data.Lack1FormYear));
            changesData.Add("LACK1_TO_MONTH", origin.Lack1ToMonthId.Equals(data.Lack1ToMonthId));
            changesData.Add("LACK1_TO_YEAR", origin.Lack1ToYear.Equals(data.Lack1ToYear));
            changesData.Add("STATUS", origin.Status.Equals(data.Status));
            changesData.Add("STATUS_GOV", origin.StatusGov.Equals(data.StatusGov));
            changesData.Add("QTY_APPROVED", origin.QtyApproved.Equals(data.QtyApproved));
            changesData.Add("DECREE_DATE", origin.DecreeDate.Equals(data.DecreeDate));
            changesData.Add("LATEST_SALDO", origin.LatestSaldo.Equals(data.LatestSaldo));
            changesData.Add("LATEST_SALDO_UOM", origin.LatestSaldoUomId.Equals(data.LatestSaldoUomId));

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.PBCK1,
                        FORM_ID = data.Pbck1Id,
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
                            changes.OLD_VALUE = origin.NppbkcNo;
                            changes.NEW_VALUE = data.NppbkcNo;
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
                            changes.OLD_VALUE = origin.RequestQtyUomId.HasValue ? origin.RequestQtyUomName : "NULL";
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
                            changes.OLD_VALUE = origin.LatestSaldoUomId.HasValue
                                ? origin.LatestSaldoUomName
                                : "NULL";
                            changes.NEW_VALUE = data.LatestSaldoUomName;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        private void AddWorkflowHistory(long id, Core.Enums.ActionType actionType, int userId)
        {
            var dbData = _workflowHistoryBll.GetSpecificWorkflowHistory(Core.Enums.FormType.PBKC1, id, actionType);

            if (dbData == null)
            {
                dbData = new WORKFLOW_HISTORY()
                {
                    ACTION =  actionType, FORM_ID =  id, FORM_TYPE_ID = Core.Enums.FormType.PBKC1
                };
            }
            dbData.ACTION_BY = userId;
            dbData.ACTION_DATE = DateTime.Now;
            _workflowHistoryBll.Save(dbData);
        }

    }
}
