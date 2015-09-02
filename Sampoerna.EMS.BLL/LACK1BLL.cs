using System;
using System.Collections.Generic;
using System.Linq;
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
        private ILACK1Service _lack1Service;

        public LACK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);

            _ck4cItemService = new CK4CItemService(_uow, _logger);
            _brandRegistrationService = new BrandRegistrationService(_uow, _logger);
            _ck5Service = new CK5Service(_uow, _logger);
            _pbck1Service = new PBCK1Service(_uow, _logger);
            _t001KService = new T001KService(_uow, _logger);
            _lack1Service = new LACK1Service(_uow, _logger);
        }

        public List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input)
        {
            return Mapper.Map<List<Lack1Dto>>(_lack1Service.GetAllByParam(input));
        }

        public List<Lack1Dto> GetCompletedDocumentByParam(Lack1GetByParamInput input)
        {
            var dbData = _lack1Service.GetCompletedDocumentByParam(input);
            var mapResult = Mapper.Map<List<Lack1Dto>>(dbData.ToList());
            return mapResult;
        }

        public Lack1CreateOutput Create(Lack1CreateParamInput input)
        {
            var generatedData = GenerateLack1Data(input);
            if (!generatedData.Success)
            {
                return new Lack1CreateOutput()
                {
                    Success = generatedData.Success,
                    ErrorCode = generatedData.ErrorCode,
                    ErrorMessage = generatedData.ErrorMessage,
                    Id = null,
                    Lack1Number = string.Empty
                };
            }

            var rc = new Lack1CreateOutput()
            {
                Success = false,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            var data = Mapper.Map<LACK1>(generatedData);

            //set default when create new LACK-1 Document
            data.APPROVED_BY_POA = null;
            data.APPROVED_DATE_POA = null;
            data.APPROVED_BY_MANAGER = null;
            data.APPROVED_DATE_MANAGER = null;
            data.DECREE_DATE = null;
            data.GOV_STATUS = null;
            data.STATUS = Enums.DocumentStatus.Draft;
            data.CREATED_DATE = DateTime.Now;

            //set from input, exclude on mapper
            data.CREATED_BY = input.UserId;
            data.LACK1_LEVEL = input.Lack1Level;
            data.SUBMISSION_DATE = input.SubmissionDate;
            data.WASTE_QTY = input.WasteAmount;
            data.WASTE_UOM = input.WasteAmountUom;
            data.RETURN_QTY = input.ReturnAmount;
            data.RETURN_UOM = input.ReturnAmountUom;

            //generate new Document Number get from Sequence Number BLL
            var generateNumberInput = new GenerateDocNumberInput()
            {
                Year = Convert.ToInt32(input.PeriodMonth),
                Month = Convert.ToInt32(input.PeriodYear),
                NppbkcId = input.NppbkcId
            };
            data.LACK1_NUMBER = _docSeqNumBll.GenerateNumber(generateNumberInput);

            _uow.SaveChanges();

            rc.Success = true;
            rc.ErrorCode = string.Empty;
            rc.Id = data.LACK1_ID;
            rc.Lack1Number = data.LACK1_NUMBER;

            return rc;
        }

        public decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input)
        {
            return _lack1Service.GetLatestSaldoPerPeriod(input);
        }
        
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
            var getData = _lack1Service.GetByPeriod(input);

            if (getData.Count == 0) return new List<Lack1Dto>();

            var mappedData = Mapper.Map<List<Lack1Dto>>(getData);

            var selected =
                mappedData.Where(c => c.Periode <= input.PeriodTo.AddDays(1) && c.Periode >= input.PeriodFrom)
                    .OrderByDescending(o => o.Periode)
                    .ToList();

            return selected.Count == 0 ? new List<Lack1Dto>() : selected;
        }

        internal List<LACK1_PRODUCTION_DETAIL> GetProductionDetailByPeriode(Lack1GetByPeriodParamInput input)
        {
            var getData = _lack1Service.GetProductionDetailByPeriode(input);

            if (getData == null) return new List<LACK1_PRODUCTION_DETAIL>();

            //todo: select by periode in range period from and period to from input param

            return getData.ToList();
        }

        public Lack1GeneratedOutput GenerateLack1DataByParam(Lack1GenerateDataParamInput input)
        {
            return GenerateLack1Data(input);
        }

        #region ----------------Private Method-------------------
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

        private Lack1GeneratedOutput GenerateLack1Data(Lack1GenerateDataParamInput input)
        {
            var oReturn = new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            //check if already exists with same selection criteria
            var lack1Check = _lack1Service.GetBySelectionCriteria(new Lack1GetBySelectionCriteriaParamInput()
            {
                CompanyCode = input.CompanyCode,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                ReceivingPlantId = input.ReceivedPlantId,
                SupplierPlantId = input.SupplierPlantId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear
            });

            if (lack1Check != null)
            {
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria),
                    Data = null
                };
            }

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
            if (rc.IncomeList.Count > 0)
            {
                rc.TotalIncome = rc.IncomeList.Sum(d => d.Amount);
            }

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

            //set summary
            rc = SetSummaryProductionlist(rc);
            rc.EndingBalance = rc.BeginingBalance - rc.TotalUsage + rc.TotalIncome;

            oReturn.Data = rc;

            return oReturn;
        }

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
            var selected = _lack1Service.GetLatestLack1ByParam(new Lack1GetLatestLack1ByParamInput()
            {
                CompanyCode = input.CompanyCode,
                Lack1Level = input.Lack1Level,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                SupplierPlantId = input.SupplierPlantId,
                ReceivedPlantId = input.ReceivedPlantId,
                PeriodTo = dtTo
            });

            rc.BeginingBalance = 0;
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
                    rc.Lack1UomId = latestDecreeDate.REQUEST_QTY_UOM;
                }
            }
            return rc;
        }

        /// <summary>
        /// set Summary Production List 
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        private Lack1GeneratedDto SetSummaryProductionlist(Lack1GeneratedDto rc)
        {
            if (rc.ProductionList.Count > 0)
            {
                var groupedData = rc.ProductionList.GroupBy(p => new
                {
                    p.ProdCode,
                    p.ProductType,
                    p.ProductAlias,
                    p.UomId,
                    p.UomDesc
                }).Select(g => new Lack1GeneratedProductionDataDto()
                {
                    ProdCode = g.Key.ProdCode,
                    ProductType = g.Key.ProductType,
                    ProductAlias = g.Key.ProductAlias,
                    UomId = g.Key.UomId,
                    UomDesc = g.Key.UomDesc,
                    Amount = g.Sum(p => p.Amount)
                });

                rc.SummaryProductionList = groupedData.ToList();
            }
            else
            {
                rc.SummaryProductionList = new List<Lack1GeneratedProductionDataDto>();
            }

            return rc;
        }

        #endregion

    }
}
