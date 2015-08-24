using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
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
        private IGenericRepository<LACK1> _repository;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private IPOABLL _poaBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;

        private string includeTables = "UOM, UOM1, MONTH";

        public LACK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK1>();
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
        }
        
        public List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input)
        {
            includeTables += ", LACK1_PLANT";
            Expression<Func<LACK1, bool>> queryFilter = c => c.LACK1_LEVEL == input.Lack1Level;

            queryFilter = queryFilter.And(ProcessQueryFilter(input));
            
            Func<IQueryable<LACK1>, IOrderedQueryable<LACK1>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK1>(input.SortOrderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<Lack1Dto>>(dbData.ToList());

            return mapResult;
        }

        public List<Lack1Dto> GetCompletedDocumentByParam(Lack1GetByParamInput input)
        {
            includeTables += ", LACK1_PLANT";
            Expression<Func<LACK1, bool>> queryFilter = c => (int) c.STATUS >= (int) Enums.DocumentStatus.Completed;

            queryFilter = queryFilter.And(ProcessQueryFilter(input));

            Func<IQueryable<LACK1>, IOrderedQueryable<LACK1>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK1>(input.SortOrderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<Lack1Dto>>(dbData.ToList());

            return mapResult;
        }

        public SaveLack1Output Save(Lack1SaveInput input)
        {
            LACK1 dbData;

            if (input.Lack1.Lack1Id > 0)
            {

                //update
                dbData = _repository.Get(c => c.LACK1_ID == input.Lack1.Lack1Id, null, includeTables).FirstOrDefault();

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                var origin = Mapper.Map<Lack1Dto>(dbData);
                SetChangesHistory(origin, input.Lack1, input.UserId);

                Mapper.Map<Lack1Dto, LACK1>(input.Lack1, dbData);
                dbData.LACK1_DOCUMENT = null;

                dbData.LACK1_DOCUMENT = Mapper.Map<List<LACK1_DOCUMENT>>(input.Lack1.DecreeDoc);

            }
            else
            {
                //Insert
                var generateNumberInput = new GenerateDocNumberInput()
                {
                    Year = Convert.ToInt32(input.Lack1.PeriodMonth),
                    Month = Convert.ToInt32(input.Lack1.PeriodYears),
                    NppbkcId = input.Lack1.NppbkcId
                };

                input.Lack1.Lack1Number = _docSeqNumBll.GenerateNumber(generateNumberInput);
                input.Lack1.Status = Enums.DocumentStatus.Draft;
                input.Lack1.CreateDate = DateTime.Now;
                dbData = new LACK1();
                Mapper.Map<Lack1Dto, LACK1>(input.Lack1, dbData);

                _repository.Insert(dbData);

            }

            var output = new SaveLack1Output();

            _uow.SaveChanges();

            output.Success = true;
            output.Id = dbData.LACK1_ID;
            output.Lack1Number = dbData.LACK1_NUMBER;

            //set workflow history
            var getUserRole = _poaBll.GetUserRole(input.UserId);

            var inputAddWorkflowHistory = new Lack1WorkflowDocumentInput()
            {
                DocumentId = output.Id,
                DocumentNumber = output.Lack1Number,
                ActionType = input.WorkflowActionType,
                UserId = input.UserId,
                UserRole = getUserRole
            };

            AddWorkflowHistory(inputAddWorkflowHistory);

            _uow.SaveChanges();

            return output;
        }

        public decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input)
        {
            var dtTo = new DateTime(input.YearTo, input.MonthTo, 1);

            var getData = _repository.Get(c => c.NPPBKC_ID == input.NppbkcId
                                               &&
                                               (int)c.STATUS >= (int)Enums.DocumentStatus.Approved, null,
                "LACK1_ITEM").ToList().Select(p => new
                {
                    p.LACK1_ID,
                    p.LACK1_NUMBER,
                    p.PERIOD_MONTH,
                    p.PERIOD_YEAR,
                    p.BEGINING_BALANCE,
                    p.TOTAL_INCOME,
                    p.USAGE,
                    p.TOTAL_PRODUCTION,
                    PERIODE = new DateTime(p.PERIOD_YEAR.Value, p.PERIOD_MONTH.Value, 1)
                }).ToList();

            if (getData.Count == 0) return 0;

            var selected = getData.Where(c => c.PERIODE <= dtTo).OrderByDescending(o => o.PERIODE).FirstOrDefault();

            if (selected == null) return 0;

            decimal rc = 0;

            rc = selected.BEGINING_BALANCE + selected.TOTAL_INCOME - selected.USAGE;

            return rc;
        }

        #region Private Methods

        private void SetChangesHistory(Lack1Dto origin, Lack1Dto data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("BUKRS", origin.Bukrs == data.Bukrs);

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
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }

        }

        private Expression<Func<LACK1, bool>> ProcessQueryFilter(Lack1GetByParamInput input)
        {

            Expression<Func<LACK1, bool>> queryFilter = PredicateHelper.True<LACK1>();

            //filter search by nppbkc id, both Level NPPBKC and Level Plant
            if (!string.IsNullOrEmpty(input.NppbKcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbKcId);
            }

            //filter search by plant id, only LACK-1 Level Plant
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter =
                    queryFilter.And(c => c.LACK1_PLANT.Any(p => p.PLANT_ID == input.PlantId));
            }

            //filter search by poa, both Lack-1 Level
            if (!string.IsNullOrEmpty(input.Poa))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Poa || c.APPROVED_BY_POA == input.Poa);
            }

            //filter search by creator
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }

            if (input.SubmissionDate.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.SUBMISSION_DATE.HasValue &&
                            c.SUBMISSION_DATE.Value == input.SubmissionDate.Value);
            }

            if (input.IsOpenDocumentOnly)
            {
                queryFilter = queryFilter.And(c => (int) c.STATUS <= (int) Enums.DocumentStatus.WaitingGovApproval);
            }

            return queryFilter;
        }

        #endregion

        #region workflow

        private void AddWorkflowHistory(Lack1WorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.FORM_TYPE_ID = Enums.FormType.LACK1;

            _workflowHistoryBll.Save(dbData);

        }

        #endregion

        private bool IsFoundPlant(IEnumerable<LACK1_PLANT> input, string plantId)
        {
            var rc = input.Where(c => c.PLANT_ID == plantId).ToList();
            return rc.Count > 0;
        }
    }
}
