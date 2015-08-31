using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
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
        private IGenericRepository<LACK1_PRODUCTION_DETAIL> _productionDetailRepository;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private IPOABLL _poaBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;

        //services
        private ICK4CItemService _ck4cItemService;
        private IBrandRegistrationService _brandRegistrationService;
        private ICK5Service _ck5Service;
        private IPBCK1Service _pbck1Service;
        private IT001KService _t001KService;
        
        private string includeTables = "UOM, UOM1, MONTH";

        public LACK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK1>();
            _productionDetailRepository = _uow.GetGenericRepository<LACK1_PRODUCTION_DETAIL>();

            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);

            _ck4cItemService = new CK4CItemService(_uow, _logger);
            _brandRegistrationService = new BrandRegistrationService(_uow, _logger);
            _ck5Service = new CK5Service(_uow, _logger);
            _pbck1Service = new PBCK1Service(_uow, _logger);
            _t001KService = new T001KService(_uow, _logger);
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
                "").ToList().Select(p => new
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

        public List<Lack1Dto> GetByPeriod(Lack1GetByPeriodParamInput input)
        {
            var getData =
                _repository.Get(
                    c => c.NPPBKC_ID == input.NppbkcId && (int) c.STATUS >= (int) Enums.DocumentStatus.Approved, null,
                    "").ToList();

            if (getData.Count == 0) return new List<Lack1Dto>();

            var mappedData = Mapper.Map<List<Lack1Dto>>(getData);

            var selected =
                mappedData.Where(c => c.Periode <= input.PeriodTo.AddDays(1) && c.Periode >= input.PeriodFrom)
                    .OrderByDescending(o => o.Periode)
                    .ToList();

            if (selected.Count == 0) return new List<Lack1Dto>();

            return selected;

        }

        internal List<LACK1_PRODUCTION_DETAIL> GetProductionDetailByPeriode(Lack1GetByPeriodParamInput input)
        {
            var getData =
                _productionDetailRepository.Get(
                    c =>
                        c.LACK1.NPPBKC_ID == input.NppbkcId &&
                        (int) c.LACK1.STATUS >= (int) Enums.DocumentStatus.Approved, null,
                    "LACK1, LACK1.UOM11, LACK1.MONTH");
            
            if(getData == null) return new List<LACK1_PRODUCTION_DETAIL>();
            
            //todo: select by periode in range period from and period to from input param

            return getData.ToList();
        }

        public Lack1GeneratedDto GenerateLack1DataByParam(Lack1GenerateDataParamInput input)
        {

            var rc = new Lack1GeneratedDto
            {
                CompanyCode = input.CompanyCode,
                CompanyName = input.CompanyName,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                ExcisableGoodsTypeDesc = input.ExcisableGoodsTypeDesc,
                BeginingBalance = 0 //set default
            };

            //set begining balance
            rc = SetBeginingBalanceBySelectionCritera(rc, input);

            //set Pbck-1 Data by selection criteria
            rc = SetPbck1DataBySelectionCriteria(rc, input);

            //Set Income List by selection Criteria
            //from CK5 data
            rc = SetIncomeListBySelectionCriteria(rc, input);


            rc.ProductionList = SetProductionDetailBySelectionCriteria(input);

            rc.PeriodMonthId = input.PeriodMonth;

            var monthData = _monthBll.GetMonth(rc.PeriodMonthId);
            if (monthData != null)
            {
                rc.PeriodMonthName = monthData.MONTH_NAME_IND;
            }

            rc.PeriodYear = input.PeriodYear;
            rc.Noted = input.Noted;

            rc.TotalUsage = 0; //todo: get from Inventory Movement

            return rc;
        }

        #region ----------------Private Method-------------------

        /// <summary>
        /// Set Production Detail from CK4C Item table 
        /// for Generate LACK-1 data by Selection Criteria
        /// </summary>
        private List<Lack1GeneratedProductionDataDto> SetProductionDetailBySelectionCriteria(
            Lack1GenerateDataParamInput input)
        {

            var ck4CItemInput = Mapper.Map<CK4CItemGetByParamInput>(input);
            var ck4CItemData = _ck4cItemService.GetByParam(ck4CItemInput);
            var faCodeList = ck4CItemData.Select(c => c.FA_CODE).Distinct().ToList();

            //get prod_code by fa_code list on selected CK4C_ITEM by selection criteria
            var brandDataSelected = _brandRegistrationService.GetByFaCodeList(faCodeList);

            //joined data
            var dataCk4CItemJoined = (from ck4CItem in ck4CItemData
                                      join brandData in brandDataSelected on ck4CItem.FA_CODE equals brandData.FA_CODE
                                      select new Lack1GeneratedProductionDataDto()
                                      {
                                          ProdCode = brandData.PROD_CODE,
                                          ProductType = brandData.ZAIDM_EX_PRODTYP.PRODUCT_TYPE,
                                          ProductAlias = brandData.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS,
                                          Amount = ck4CItem.PROD_QTY,
                                          UomId = ck4CItem.UOM != null ? ck4CItem.UOM.UOM_DESC : string.Empty
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
            var ck5Data = _ck5Service.GetForLack1ByParam(ck5Input);
            rc.IncomeList = Mapper.Map<List<Lack1GeneratedIncomeDataDto>>(ck5Data);

            if (rc.IncomeList.Count > 0)
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

            Expression<Func<LACK1, bool>> queryFilter =
                c => c.BUKRS == input.CompanyCode && c.LACK1_LEVEL == input.Lack1Level
                     && c.NPPBKC_ID == input.NppbkcId && c.NPPBKC_ID == input.NppbkcId
                     && (int)c.STATUS >= (int)Enums.DocumentStatus.Approved
                     && c.EX_GOODTYP == input.ExcisableGoodsType
                     && c.SUPPLIER_PLANT_WERKS == input.SupplierPlantId;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilter =
                    queryFilter.And(c => c.LACK1_PLANT.Any(p => p.PLANT_ID == input.ReceivedPlantId));
            }

            var getData = _repository.Get(queryFilter, null,
                "").ToList().Select(p => new
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

            var selected = getData.Where(c => c.PERIODE <= dtTo).OrderByDescending(o => o.PERIODE).FirstOrDefault();

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
                }
            }
            return rc;
        }

        #endregion

    }
}
