using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.LinqExtensions;
using Sampoerna.EMS.MessagingService;
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
        private IHeaderFooterBLL _headerFooterBll;
        private IWorkflowBLL _workflowBll;
        private IUserBLL _userBll;

        //services
        private ICK4CItemService _ck4cItemService;
        private ICK5Service _ck5Service;
        private IPBCK1Service _pbck1Service;
        private IT001KService _t001KService;
        private ILACK1Service _lack1Service;
        private IT001WService _t001WServices;
        private IExGroupTypeService _exGroupTypeService;
        private IInventoryMovementService _inventoryMovementService;
        private IMessageService _messageService;
        private ILack1IncomeDetailService _lack1IncomeDetailService;
        private ILack1Pbck1MappingService _lack1Pbck1MappingService;
        private ILack1PlantService _lack1PlantService;
        private ILack1ProductionDetailService _lack1ProductionDetailService;
        private ILack1TrackingService _lack1TrackingService;
        private IZaidmExProdTypeService _prodTypeService;
        private IZaidmExNppbkcService _nppbkcService;
        private IZaapShiftRptService _zaapShiftRptService;
        private IPbck1ProdConverterService _pbck1ProdConverterService;
        private IZaidmExMaterialService _materialService;
        private IGoodProdTypeService _goodProdTypeService;
        private IMaterialUomService _materialUomService;
        private IPoaDelegationServices _poaDelegationServices;
        private IMaterialBalanceService _materialBalanceService;

        public LACK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
            _headerFooterBll = new HeaderFooterBLL(_uow, _logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);

            _ck4cItemService = new CK4CItemService(_uow, _logger);
            _ck5Service = new CK5Service(_uow, _logger);
            _pbck1Service = new PBCK1Service(_uow, _logger);
            _t001KService = new T001KService(_uow, _logger);
            _lack1Service = new LACK1Service(_uow, _logger);
            _t001WServices = new T001WService(_uow, _logger);
            _exGroupTypeService = new ExGroupTypeService(_uow, _logger);
            _inventoryMovementService = new InventoryMovementService(_uow, _logger);
            _messageService = new MessageService(_logger);
            _lack1IncomeDetailService = new Lack1IncomeDetailService(_uow, _logger);
            _lack1Pbck1MappingService = new Lack1Pbck1MappingService(_uow, _logger);
            _lack1PlantService = new Lack1PlantService(_uow, _logger);
            _lack1ProductionDetailService = new Lack1ProductionDetailService(_uow, _logger);
            _lack1TrackingService = new Lack1TrackingService(_uow, _logger);
            _prodTypeService = new ZaidmExProdTypeService(_uow, _logger);
            _nppbkcService = new ZaidmExNppbkcService(_uow, _logger);
            _zaapShiftRptService = new ZaapShiftRptService(_uow, _logger);
            _pbck1ProdConverterService = new Pbck1ProdConverterService(_uow, _logger);
            _materialService = new ZaidmExMaterialService(_uow, _logger);
            _goodProdTypeService = new GoodProdTypeService(_uow, _logger);
            _materialUomService = new MaterialUomService(_uow, _logger);
            _poaDelegationServices = new PoaDelegationServices(_uow, _logger);
            _materialBalanceService = new MaterialBalanceService(_uow, _logger);
        }

        public List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input)
        {

            if (input.UserRole == Enums.UserRole.POA)
            {
                var nppbkc = _nppbkcService.GetNppbkcsByPoa(input.UserId);
                if (nppbkc != null && nppbkc.Count > 0)
                {
                    input.NppbkcList = nppbkc.Select(c => c.NPPBKC_ID).ToList();
                }
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poaBll.GetPOAIdByManagerId(input.UserId);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);
                input.DocumentNumberList = document;
            }
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
            input.IsCreateNew = true;
            //check if exists
            var isExists = IsExistLack1Data(input);
            if (!isExists.Success) return new Lack1CreateOutput()
            {
                Success = isExists.Success,
                ErrorCode = isExists.ErrorCode,
                ErrorMessage = isExists.ErrorMessage,
                Id = null,
                Lack1Number = string.Empty
            };
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

            var data = Mapper.Map<LACK1>(generatedData.Data);

            //set default when create new LACK-1 Document
            data.APPROVED_BY_POA = null;
            data.APPROVED_DATE_POA = null;
            data.APPROVED_BY_MANAGER = null;
            data.APPROVED_DATE_MANAGER = null;
            data.DECREE_DATE = null;
            data.GOV_STATUS = null;
            data.STATUS = Enums.DocumentStatus.Draft;
            data.CREATED_DATE = DateTime.Now;
            data.IS_TIS_TO_TIS = input.IsTisToTis;

            //set from input, exclude on mapper
            data.CREATED_BY = input.UserId;
            data.LACK1_LEVEL = input.Lack1Level;
            data.SUBMISSION_DATE = input.SubmissionDate;
            data.WASTE_QTY = input.WasteAmount;
            data.WASTE_UOM = input.WasteAmountUom;
            data.RETURN_QTY = input.ReturnAmount;
            data.RETURN_UOM = input.ReturnAmountUom;

            //set LACK1_TRACKING
            var allTrackingList = new List<Lack1GeneratedTrackingDto>();
            if (generatedData.Data.InventoryProductionTisToFa.InvetoryMovementData != null)
            {
                //tis_to_fa
                var toAddRange = generatedData.Data.InventoryProductionTisToFa.InvetoryMovementData.InvMovementAllList;
                toAddRange.AddRange(generatedData.Data.InventoryProductionTisToFa.InvetoryMovementData.InvMovementReceivingList);

                for (var i = 0; i < toAddRange.Count; i++)
                {
                    toAddRange[i].IsTisToTisData = false;
                }

                allTrackingList = toAddRange;
            }
            if (input.IsTisToTis && !generatedData.IsEtilAlcohol)
            {
                //Tis To Tis
                if (generatedData.Data.InventoryProductionTisToTis.InvetoryMovementData != null)
                {
                    var toAddRange = generatedData.Data.InventoryProductionTisToTis.InvetoryMovementData.InvMovementAllList;
                    toAddRange.AddRange(generatedData.Data.InventoryProductionTisToTis.InvetoryMovementData.InvMovementReceivingList);

                    for (var i = 0; i < toAddRange.Count; i++)
                    {
                        toAddRange[i].IsTisToTisData = true;
                    }

                    allTrackingList.AddRange(toAddRange);
                }
            }

            data.LACK1_TRACKING = Mapper.Map<List<LACK1_TRACKING>>(allTrackingList.Distinct().ToList());

            if (!string.IsNullOrEmpty(generatedData.Data.ExcisableGoodsTypeDesc) && (generatedData.Data.ExcisableGoodsTypeDesc.ToLower().Contains("alkohol") ||
                                              generatedData.Data.ExcisableGoodsTypeDesc.ToLower().Contains("alcohol")))
            {
                //etil alcohol 100%
                data.LACK1_TRACKING_ALCOHOL = null;
                data.LACK1_TRACKING_ALCOHOL =
                    Mapper.Map<List<LACK1_TRACKING_ALCOHOL>>(generatedData.Data.AlcoholTrackingList.Distinct().ToList());
            }

            data.LACK1_PLANT = null;

            //set LACK1_PLANT table
            if (input.Lack1Level == Enums.Lack1Level.Nppbkc)
            {
                var plantListFromMaster = _t001WServices.GetByNppbkcId(input.NppbkcId);
                data.LACK1_PLANT = Mapper.Map<List<LACK1_PLANT>>(plantListFromMaster);
            }
            else
            {
                var plantFromMaster = _t001WServices.GetById(input.ReceivedPlantId);
                data.LACK1_PLANT = new List<LACK1_PLANT>() { Mapper.Map<LACK1_PLANT>(plantFromMaster) };
            }

            //set LACK1_PRODUCTION_DETAIL
            data.LACK1_PRODUCTION_DETAIL = null;
            var productionDetail = new List<Lack1GeneratedProductionDataDto>();

            //tis to fa
            if (generatedData.Data.InventoryProductionTisToFa.ProductionData != null)
            {
                var toAddRange = generatedData.Data.InventoryProductionTisToFa.ProductionData.ProductionList;
                for (var i = 0; i < toAddRange.Count; i++)
                {
                    toAddRange[i].IsTisToTisData = false;
                }
                productionDetail.AddRange(toAddRange);
            }

            if (input.IsTisToTis && !generatedData.IsEtilAlcohol)
            {
                //tis to tis
                if (generatedData.Data.InventoryProductionTisToTis.ProductionData != null)
                {
                    var toAddRange = generatedData.Data.InventoryProductionTisToTis.ProductionData.ProductionList;
                    for (var i = 0; i < toAddRange.Count; i++)
                    {
                        toAddRange[i].IsTisToTisData = true;
                    }
                    productionDetail.AddRange(toAddRange);
                }
            }

            data.LACK1_PRODUCTION_DETAIL =
                Mapper.Map<List<LACK1_PRODUCTION_DETAIL>>(productionDetail.Distinct().ToList());

            //generate new Document Number get from Sequence Number BLL
            var generateNumberInput = new GenerateDocNumberInput()
            {
                Month = Convert.ToInt32(input.PeriodMonth),
                Year = Convert.ToInt32(input.PeriodYear),
                NppbkcId = input.NppbkcId
            };

            data.LACK1_NUMBER = _docSeqNumBll.GenerateNumber(generateNumberInput);

            _lack1Service.Insert(data);

            //add workflow history for create document
            var getUserRole = _poaBll.GetUserRole(input.UserId);
            AddWorkflowHistory(new Lack1WorkflowDocumentInput()
            {
                DocumentId = null,
                DocumentNumber = data.LACK1_NUMBER,
                ActionType = Enums.ActionType.Created,
                UserId = input.UserId,
                UserRole = getUserRole
            });

            _uow.SaveChanges();

            rc.Success = true;
            rc.ErrorCode = string.Empty;
            rc.Id = data.LACK1_ID;
            rc.Lack1Number = data.LACK1_NUMBER;

            return rc;
        }

        public SaveLack1Output SaveEdit(Lack1SaveEditInput input)
        {
            bool isModified = false;
            var rc = new SaveLack1Output()
            {
                Success = false
            };

            if (input == null)
            {
                throw new Exception("Invalid data entry");
            }

            //origin
            var dbData = _lack1Service.GetDetailsById(input.Detail.Lack1Id);

            //check if need to regenerate
            var isNeedToRegenerate = dbData.STATUS == Enums.DocumentStatus.Draft || dbData.STATUS == Enums.DocumentStatus.Rejected;
            var generateInput = Mapper.Map<Lack1GenerateDataParamInput>(input);
            //if (!isNeedToRegenerate)
            //{
            //    isNeedToRegenerate = IsNeedToRegenerate(generateInput, dbData);
            //}

            if (isNeedToRegenerate)
            {
                //do regenerate data
                generateInput.IsCreateNew = false;
                var generatedData = GenerateLack1Data(generateInput);
                if (!generatedData.Success)
                {
                    return new SaveLack1Output()
                    {
                        Success = false,
                        ErrorCode = generatedData.ErrorCode,
                        ErrorMessage = generatedData.ErrorMessage
                    };
                }

                var origin = Mapper.Map<Lack1DetailsDto>(dbData);
                var destination = Mapper.Map<Lack1DetailsDto>(generatedData.Data);

                destination.Lack1Id = dbData.LACK1_ID;
                destination.Lack1Number = dbData.LACK1_NUMBER;
                destination.Lack1Level = dbData.LACK1_LEVEL;
                destination.SubmissionDate = input.Detail.SubmissionDate;
                destination.SupplierCompanyName = origin.SupplierCompanyName;
                destination.SupplierCompanyCode = origin.SupplierCompanyCode;
                destination.WasteQty = input.Detail.WasteQty;
                destination.WasteUom = input.Detail.WasteUom;
                destination.WasteUomDesc = input.Detail.WasteUomDesc;
                destination.ReturnQty = input.Detail.ReturnQty;
                destination.ReturnUom = input.Detail.ReturnUom;
                destination.ReturnUomDesc = input.Detail.ReturnUomDesc;
                destination.Status = input.Detail.Status;
                destination.GovStatus = input.Detail.GovStatus;
                destination.DecreeDate = input.Detail.DecreeDate;
                destination.DocumentNoted = generatedData.Data.DocumentNoted;
                destination.Noted = input.Detail.Noted;
                isModified = SetChangesHistory(origin, destination, input.UserId);
                destination.IsTisToTis = input.IsTisToTis;

                //delete first
                _lack1TrackingService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1IncomeDetailService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1Pbck1MappingService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1PlantService.DeleteByLack1Id(dbData.LACK1_ID);
                _lack1ProductionDetailService.DeleteByLack1Id(dbData.LACK1_ID);

                //regenerate
                Mapper.Map<Lack1GeneratedDto, LACK1>(generatedData.Data, dbData);

                //set to null
                dbData.LACK1_INCOME_DETAIL = null;
                dbData.LACK1_PBCK1_MAPPING = null;
                dbData.LACK1_PLANT = null;
                dbData.LACK1_PRODUCTION_DETAIL = null;
                dbData.LACK1_TRACKING = null;

                //set from input
                dbData.LACK1_INCOME_DETAIL = Mapper.Map<List<LACK1_INCOME_DETAIL>>(generatedData.Data.AllIncomeList);
                dbData.LACK1_PBCK1_MAPPING = Mapper.Map<List<LACK1_PBCK1_MAPPING>>(generatedData.Data.Pbck1List);

                //set LACK1_PRODUCTION_DETAIL
                var productionDetail = new List<Lack1GeneratedProductionDataDto>();

                //tis to fa
                if (generatedData.Data.InventoryProductionTisToFa.ProductionData != null)
                {
                    var toAddRange = generatedData.Data.InventoryProductionTisToFa.ProductionData.ProductionList;
                    for (var i = 0; i < toAddRange.Count; i++)
                    {
                        toAddRange[i].IsTisToTisData = false;
                    }
                    productionDetail.AddRange(toAddRange);
                }

                if (input.IsTisToTis && !generatedData.IsEtilAlcohol)
                {
                    //tis to tis
                    if (generatedData.Data.InventoryProductionTisToTis.ProductionData != null)
                    {
                        var toAddRange = generatedData.Data.InventoryProductionTisToTis.ProductionData.ProductionList;
                        for (var i = 0; i < toAddRange.Count; i++)
                        {
                            toAddRange[i].IsTisToTisData = true;
                        }
                        productionDetail.AddRange(toAddRange);
                    }
                }

                dbData.LACK1_PRODUCTION_DETAIL =
                    Mapper.Map<List<LACK1_PRODUCTION_DETAIL>>(productionDetail.Distinct().ToList());

                //set LACK1_TRACKING
                var allTrackingList = new List<Lack1GeneratedTrackingDto>();
                if (generatedData.Data.InventoryProductionTisToFa.InvetoryMovementData != null)
                {
                    //tis_to_fa
                    var toAddRange = generatedData.Data.InventoryProductionTisToFa.InvetoryMovementData.InvMovementAllList;
                    toAddRange.AddRange(generatedData.Data.InventoryProductionTisToFa.InvetoryMovementData.InvMovementReceivingList);

                    for (var i = 0; i < toAddRange.Count; i++)
                    {
                        toAddRange[i].IsTisToTisData = false;
                    }

                    allTrackingList = toAddRange;
                }
                if (input.IsTisToTis && !generatedData.IsEtilAlcohol)
                {
                    //Tis To Tis
                    if (generatedData.Data.InventoryProductionTisToTis.InvetoryMovementData != null)
                    {
                        var toAddRange = generatedData.Data.InventoryProductionTisToTis.InvetoryMovementData.InvMovementAllList;
                        toAddRange.AddRange(generatedData.Data.InventoryProductionTisToTis.InvetoryMovementData.InvMovementReceivingList);

                        for (var i = 0; i < toAddRange.Count; i++)
                        {
                            toAddRange[i].IsTisToTisData = true;
                        }

                        allTrackingList.AddRange(toAddRange);
                    }
                }

                dbData.LACK1_TRACKING = Mapper.Map<List<LACK1_TRACKING>>(allTrackingList.Distinct().ToList());

                if (!string.IsNullOrEmpty(generatedData.Data.ExcisableGoodsTypeDesc) && (generatedData.Data.ExcisableGoodsTypeDesc.ToLower().Contains("alkohol") ||
                                              generatedData.Data.ExcisableGoodsTypeDesc.ToLower().Contains("alcohol")))
                {
                    //etil alcohol 100%
                    dbData.LACK1_TRACKING_ALCOHOL = null;
                    dbData.LACK1_TRACKING_ALCOHOL =
                        Mapper.Map<List<LACK1_TRACKING_ALCOHOL>>(generatedData.Data.AlcoholTrackingList.Distinct().ToList());
                }

                //set LACK1_PLANT table
                if (input.Detail.Lack1Level == Enums.Lack1Level.Nppbkc)
                {
                    var plantListFromMaster = _t001WServices.GetByNppbkcId(input.Detail.NppbkcId);
                    dbData.LACK1_PLANT = Mapper.Map<List<LACK1_PLANT>>(plantListFromMaster);
                }
                else
                {
                    var plantFromMaster = _t001WServices.GetById(input.Detail.LevelPlantId);
                    dbData.LACK1_PLANT = new List<LACK1_PLANT>() { Mapper.Map<LACK1_PLANT>(plantFromMaster) };
                }
                dbData.NOTED = generatedData.Data.Noted;
                dbData.DOCUMENT_NOTED = generatedData.Data.DocumentNoted;
            }
            else
            {
                var origin = Mapper.Map<Lack1DetailsDto>(dbData);

                isModified = SetChangesHistory(origin, input.Detail, input.UserId);
            }

            dbData.SUBMISSION_DATE = input.Detail.SubmissionDate;
            dbData.LACK1_LEVEL = input.Detail.Lack1Level;
            dbData.WASTE_QTY = input.Detail.WasteQty;
            dbData.WASTE_UOM = input.Detail.WasteUom;
            dbData.RETURN_QTY = input.Detail.ReturnQty;
            dbData.RETURN_UOM = input.Detail.ReturnUom;
            dbData.IS_TIS_TO_TIS = input.IsTisToTis;

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData.STATUS == Enums.DocumentStatus.Rejected)
            {
                //add history for changes status from rejected to draft
                WorkflowStatusAddChanges(new Lack1WorkflowDocumentInput() { DocumentId = dbData.LACK1_ID, UserId = input.UserId }, dbData.STATUS, Enums.DocumentStatus.Draft);
                dbData.STATUS = Enums.DocumentStatus.Draft;
            }

            if (dbData.GOV_STATUS != null && dbData.GOV_STATUS.Value == Enums.DocumentStatusGovType2.Rejected)
            {
                //add history for changes status from rejected to draft
                WorkflowStatusGovAddChanges(new Lack1WorkflowDocumentInput() { DocumentId = dbData.LACK1_ID, UserId = input.UserId }, dbData.GOV_STATUS, null);
                dbData.GOV_STATUS = null;
            }

            _uow.SaveChanges();

            rc.Success = true;
            rc.Id = dbData.LACK1_ID;
            rc.Lack1Number = dbData.LACK1_NUMBER;
            rc.IsModifiedHistory = isModified;

            //set workflow history
            var getUserRole = _poaBll.GetUserRole(input.UserId);

            var inputAddWorkflowHistory = new Lack1WorkflowDocumentInput()
            {
                DocumentId = rc.Id,
                DocumentNumber = rc.Lack1Number,
                ActionType = input.WorkflowActionType,
                UserId = input.UserId,
                UserRole = getUserRole
            };

            //delegate user
            inputAddWorkflowHistory.Comment = _poaDelegationServices.CommentDelegatedUserSaveOrSubmit(dbData.CREATED_BY,
                input.UserId, DateTime.Now);

            AddWorkflowHistory(inputAddWorkflowHistory);

            _uow.SaveChanges();

            return rc;

        }

        public Lack1DetailsDto GetDetailsById(int id)
        {
            var dbData = _lack1Service.GetDetailsById(id);
            var rc = Mapper.Map<Lack1DetailsDto>(dbData);
            if (rc.ExGoodsTypeDesc.ToLower().Contains("alcohol") || rc.ExGoodsTypeDesc.ToLower().Contains("alkohol"))
            {
                rc.IsEtilAlcohol = true;
            }
            else
            {
                rc.IsEtilAlcohol = false;
            }
            rc.FusionSummaryProductionByProdTypeList = GetProductionDetailSummaryByProdType(rc.Lack1ProductionDetail);

            //process separate between tis to tis and tis to fa from production detail
            if (rc.Lack1ProductionDetail != null && rc.Lack1ProductionDetail.Count > 0)
            {
                var tisToTisData =
                    rc.Lack1ProductionDetail.Where(c => c.IS_TISTOTIS_DATA.HasValue && c.IS_TISTOTIS_DATA.Value)
                        .ToList();

                var tisToFaData =
                    rc.Lack1ProductionDetail.Where(c => !c.IS_TISTOTIS_DATA.HasValue || !c.IS_TISTOTIS_DATA.Value)
                        .ToList();

                rc.InventoryProductionTisToFa = new Lack1InventoryAndProductionDto()
                {
                    ProductionData = new Lack1ProductionDto()
                    {
                        ProductionList = tisToFaData,
                        ProductionSummaryByProdTypeList = GetProductionDetailSummaryByProdType(tisToFaData)
                    }
                };
                rc.InventoryProductionTisToTis = new Lack1InventoryAndProductionDto()
                {
                    ProductionData = new Lack1ProductionDto()
                    {
                        ProductionSummaryByProdTypeList = GetProductionDetailSummaryByProdType(tisToTisData),
                        ProductionList = tisToTisData
                    }
                };
            }

            if (rc.AllLack1IncomeDetail == null || rc.AllLack1IncomeDetail.Count <= 0) return rc;

            //process for incomedetail remark
            rc.Ck5RemarkData = new Lack1RemarkDto()
            {
                Ck5ReturnData = rc.AllLack1IncomeDetail.Where(c => c.CK5_TYPE == Enums.CK5Type.Return && c.FLAG_FOR_LACK1).ToList(),
                /*story : http://192.168.62.216/TargetProcess/entity/1637 
                 * Ck5 Manual Trial don't include in remark column, 
                 * see previous function about getting data from ck5 that only include ck5 manual trial if REDUCE_TRIAL value is TRUE
                 */
                Ck5WasteData = rc.AllLack1IncomeDetail.Where(c => c.CK5_TYPE == Enums.CK5Type.Waste).ToList()
            };

            //set Lack1IncomeDetail
            rc.Lack1IncomeDetail =
                rc.AllLack1IncomeDetail.Where(
                    c =>
                        !((c.CK5_TYPE == Enums.CK5Type.Return && c.FLAG_FOR_LACK1) || c.CK5_TYPE == Enums.CK5Type.Waste)).ToList();

            return rc;
        }

        public decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input)
        {
            return _lack1Service.GetLatestSaldoPerPeriod(input);
        }

        #region workflow

        public void Lack1Workflow(Lack1WorkflowDocumentInput input)
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
                    //isNeedSendNotif = false;
                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocument(input);
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.BackToGovApprovalAfterCompleted:
                    //update gov status to NULL
                    BackToGovApprovalAfterCompleted(input);
                    isNeedSendNotif = false;
                    break;
                default:
                    throw new BLLException(ExceptionCodes.BLLExceptions.InvalidWorkflowActionType);
            }

            //todo sent mail
            if (isNeedSendNotif)
                SendEmailWorkflow(input);
            _uow.SaveChanges();
        }

        private void BackToGovApprovalAfterCompleted(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId == null) return;
            var dbData = _lack1Service.GetById(input.DocumentId.Value);
            if (dbData.GOV_STATUS.HasValue && dbData.GOV_STATUS.Value == Enums.DocumentStatusGovType2.Approved)
            {
                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, null);
                dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                dbData.GOV_STATUS = null;

                //delegate
                if (dbData.CREATED_BY != input.UserId)
                {
                    var workflowHistoryDto =
                        _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                    input.Comment = _poaDelegationServices.CommentDelegatedByHistory(workflowHistoryDto.COMMENT,
                        workflowHistoryDto.ACTION_BY, input.UserId, input.UserRole, dbData.CREATED_BY, DateTime.Now);
                }
                //end delegate

            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }

            AddWorkflowHistory(input);
        }

        private void AddWorkflowHistory(Lack1WorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.FORM_TYPE_ID = Enums.FormType.LACK1;

            if (!input.IsModified && input.ActionType == Enums.ActionType.Submit)
                _workflowHistoryBll.UpdateHistoryModifiedForSubmit(dbData);
            else
                _workflowHistoryBll.Save(dbData);

        }

        private void SubmitDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

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
                        //dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                        /* CR-2 : 2015-12-22 */
                        dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;
                        break;
                    default:
                        throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
                }

                input.DocumentNumber = dbData.LACK1_NUMBER;

                //delegate
                input.Comment = _poaDelegationServices.CommentDelegatedUserSaveOrSubmit(dbData.CREATED_BY, input.UserId,
                    DateTime.Now);
            }

           

            AddWorkflowHistory(input);

        }

        private void ApproveDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //todo: gk boleh loncat approval nya, creator->poa->manager atau poa(creator)->manager
                //dbData.APPROVED_BY_POA = input.UserId;
                //dbData.APPROVED_DATE_POA = DateTime.Now;

                if (input.UserRole == Enums.UserRole.POA)
                {
                    if (dbData.STATUS == Enums.DocumentStatus.WaitingForApproval)
                    {
                        //Add Changes
                        //WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingForApprovalManager);
                        //dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                        /* CR-2 : 2015-12-22 Remove manager approve */
                        WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);
                        dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                        dbData.APPROVED_BY_POA = input.UserId;
                        dbData.APPROVED_DATE_POA = DateTime.Now;
                    }
                    else
                    {
                        throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
                    }
                }
                /* CR-2 : 2015-12-22 Remove manager approve */
                //else
                //{
                //    //manager
                //    if (dbData.STATUS == Enums.DocumentStatus.WaitingForApprovalManager)
                //    {
                //        //Add Changes
                //        WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.WaitingGovApproval);
                //        dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                //        dbData.APPROVED_BY_MANAGER = input.UserId;
                //        dbData.APPROVED_DATE_MANAGER = DateTime.Now;
                //    }
                //    else
                //    {
                //        throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
                //    }
                //}

                input.DocumentNumber = dbData.LACK1_NUMBER;

                //delegate
                input.Comment = CommentDelegateUser(dbData, input);
                //end delegate
            }

            AddWorkflowHistory(input);

        }

        private string CommentDelegateUser(LACK1 dbData, Lack1WorkflowDocumentInput input)
        {
            string comment = "";

            var inputHistory = new GetByFormTypeAndFormIdInput();
            inputHistory.FormId = dbData.LACK1_ID;
            inputHistory.FormType = Enums.FormType.LACK1;

            var rejectedPoa = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(inputHistory);
            if (rejectedPoa != null)
            {
                comment = _poaDelegationServices.CommentDelegatedByHistory(rejectedPoa.COMMENT,
                    rejectedPoa.ACTION_BY, input.UserId, input.UserRole, dbData.CREATED_BY, DateTime.Now);
            }
            else
            {
                //var isPoaCreatedUser = _poaBll.GetActivePoaById(dbData.CREATED_BY);
                //List<string> listPoa;
                //if (isPoaCreatedUser != null) //if creator = poa
                //{
                //    listPoa = _poaBll.GetPoaActiveByNppbkcId(dbData.NPPBKC_ID).Select(c => c.POA_ID).ToList();
                //}
                //else
                //{
                //    listPoa = _poaBll.GetPoaActiveByPlantId(dbData.PLANT_ID).Select(c => c.POA_ID).ToList();
                //}

                var listPoa = _poaBll.GetPoaActiveByPlantId(dbData.NPPBKC_ID).Select(c => c.POA_ID).ToList();
                comment = _poaDelegationServices.CommentDelegatedUserApproval(listPoa, input.UserId, DateTime.Now);

            }

            return comment;
        }

        private void RejectDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                //    dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                //    dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                /* CR-2 : 2015-12-22 Remove manager approve */
                if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                    dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);

                //change back to draft
                dbData.STATUS = Enums.DocumentStatus.Rejected;

                //todo ask
                dbData.APPROVED_BY_POA = null;
                dbData.APPROVED_DATE_POA = null;

                input.DocumentNumber = dbData.LACK1_NUMBER;

                //delegate
                string commentReject = CommentDelegateUser(dbData, input);

                if (!string.IsNullOrEmpty(commentReject))
                    input.Comment += " [" + commentReject + "]";
                //end delegate

            }

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGovType2.Approved);
                WorkflowDecreeDateAddChanges(input.DocumentId, input.UserId, dbData.DECREE_DATE,
                    input.AdditionalDocumentData.DecreeDate);

                dbData.LACK1_DOCUMENT = null;
                dbData.STATUS = Enums.DocumentStatus.Completed;
                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;
                dbData.LACK1_DOCUMENT = Mapper.Map<List<LACK1_DOCUMENT>>(input.AdditionalDocumentData.Lack1Document);
                dbData.GOV_STATUS = Enums.DocumentStatusGovType2.Approved;

                input.DocumentNumber = dbData.LACK1_NUMBER;

                //delegate
                if (dbData.CREATED_BY != input.UserId)
                {
                    var workflowHistoryDto =
                        _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                    input.Comment = _poaDelegationServices.CommentDelegatedByHistory(workflowHistoryDto.COMMENT,
                        workflowHistoryDto.ACTION_BY, input.UserId, input.UserRole, dbData.CREATED_BY, DateTime.Now);
                }
                //end delegate

            }

            AddWorkflowHistory(input);

        }

        private void GovRejectedDocument(Lack1WorkflowDocumentInput input)
        {
            if (input.DocumentId != null)
            {
                var dbData = _lack1Service.GetById(input.DocumentId.Value);

                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

                //Add Changes
                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGovType2.Rejected);
                WorkflowDecreeDateAddChanges(input.DocumentId, input.UserId, dbData.DECREE_DATE,
                    input.AdditionalDocumentData.DecreeDate);

                dbData.STATUS = Enums.DocumentStatus.Rejected;
                dbData.GOV_STATUS = Enums.DocumentStatusGovType2.Rejected;
                dbData.LACK1_DOCUMENT = Mapper.Map<List<LACK1_DOCUMENT>>(input.AdditionalDocumentData.Lack1Document);

                //set to null
                dbData.APPROVED_BY_POA = null;
                dbData.APPROVED_BY_MANAGER = null;
                dbData.APPROVED_DATE_MANAGER = null;
                dbData.APPROVED_DATE_POA = null;

                dbData.DECREE_DATE = input.AdditionalDocumentData.DecreeDate;

                input.DocumentNumber = dbData.LACK1_NUMBER;

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
            }

            AddWorkflowHistory(input);

        }

        private void WorkflowDecreeDateAddChanges(int? documentId, string userId, DateTime? origin, DateTime? updated)
        {
            if (origin == updated) return;
            //set changes log
            if (documentId == null) return;
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK2,
                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                FORM_ID = documentId.Value.ToString(),
                FIELD_NAME = "Decree Date",
                NEW_VALUE = origin.HasValue ? origin.Value.ToString("dd-MM-yyyy") : "NULL",
                OLD_VALUE = updated.HasValue ? updated.Value.ToString("dd-MM-yyyy") : "NULL",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowStatusAddChanges(Lack1WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK1,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = EnumHelper.GetDescription(oldStatus),
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };
            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowStatusGovAddChanges(Lack1WorkflowDocumentInput input, Enums.DocumentStatusGovType2? oldStatus, Enums.DocumentStatusGovType2? newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.LACK1,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "GOV_STATUS",
                NEW_VALUE = newStatus.HasValue ? EnumHelper.GetDescription(newStatus) : "NULL",
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void SendEmailWorkflow(Lack1WorkflowDocumentInput input)
        {
            //todo: body message from email template
            //todo: to = ?
            //todo: subject = from email template
            //var to = "irmansulaeman41@gmail.com";
            //var subject = "this is subject for " + input.DocumentNumber;
            //var body = "this is body message for " + input.DocumentNumber;
            //var from = "a@gmail.com";

            if (input.DocumentId == null) return;

            var lack1Data = Mapper.Map<Lack1DetailsDto>(_lack1Service.GetDetailsById(input.DocumentId.Value));

            if ((input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
                && lack1Data.Status != Enums.DocumentStatus.Completed)
                return;

            var mailProcess = ProsesMailNotificationBody(lack1Data, input);
            List<string> listTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(listTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(listTo, mailProcess.Subject, mailProcess.Body, true);
        }

        private MailNotification ProsesMailNotificationBody(Lack1DetailsDto lack1Data, Lack1WorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();

            var rejected = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(new GetByFormTypeAndFormIdInput()
            {
                FormId = lack1Data.Lack1Id,
                FormType = Enums.FormType.LACK1
            });

            var poaList = _poaBll.GetPoaActiveByNppbkcId(lack1Data.NppbkcId);

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            rc.Subject = "LACK-1 " + lack1Data.Lack1Number + " is " + EnumHelper.GetDescription(lack1Data.Status);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + lack1Data.Bukrs + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + lack1Data.NppbkcId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + lack1Data.Lack1Number + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : LACK-1</td></tr>");
            bodyMail.AppendLine();
            if (input.ActionType == Enums.ActionType.Reject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/Lack1/Details/" + lack1Data.Lack1Id + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (lack1Data.Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (rejected != null)
                        {
                            rc.To.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(lack1Data.CreateBy).EMAIL);
                    }
                        /* CR-2 : 2015-12-22 Remove manager approve*/
                    else if (lack1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        //var poaData = _poaBll.GetActivePoaById(lack1Data.CreateBy);
                        //rc.To.Add(GetManagerEmail(lack1Data.CreateBy));
                        var userData = _userBll.GetUserById(lack1Data.CreateBy);
                        rc.To.Add(userData.EMAIL);
                    }
                    /*Old code before CR-2*/
                    //else if (lack1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    //{
                    //    var poaData = _poaBll.GetActivePoaById(lack1Data.CreateBy);
                    //    rc.To.Add(GetManagerEmail(lack1Data.CreateBy));
                    //    rc.CC.Add(poaData.POA_EMAIL);

                    //    foreach (var poaDto in poaList)
                    //    {
                    //        if (poaData.POA_ID != poaDto.POA_ID)
                    //            rc.To.Add(poaDto.POA_EMAIL);
                    //    }
                    //}
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    if (lack1Data.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetActivePoaById(lack1Data.CreateBy);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                            //first code when manager exists
                            //rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(lack1Data.CreateBy);

                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                            //first code when manager exists
                            //rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                        }
                    }
                    //first code when manager exists
                    //else if (lackData.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    //{
                    //    rc.To.Add(GetManagerEmail(lackData.ApprovedBy));

                    //    if (rejected != null)
                    //    {
                    //        rc.CC.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                    //    }
                    //    else
                    //    {
                    //        foreach (var poaDto in poaList)
                    //        {
                    //            rc.CC.Add(poaDto.POA_EMAIL);
                    //        }
                    //    }

                    //    rc.CC.Add(_userBll.GetUserById(lackData.CreatedBy).EMAIL);

                    //}
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(lack1Data.CreateBy);
                    var poaData2 = _poaBll.GetActivePoaById(lack1Data.CreateBy);

                    if (lack1Data.ApprovedByPoa != null || poaData2 != null)
                    {
                        if (poaData2 == null)
                        {
                            var poa = _poaBll.GetById(lack1Data.ApprovedByPoa);
                            rc.To.Add(userDetail.EMAIL);
                            rc.CC.Add(poa.POA_EMAIL);
                            //first code when manager exists
                            //rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                        }
                        else
                        {
                            rc.To.Add(poaData2.POA_EMAIL);
                            //first code when manager exists
                            //rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                        }
                    }
                    else
                    {
                        rc.To.Add(userDetail.EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            rc.CC.Add(poaDto.POA_EMAIL);
                        }
                    }

                    rc.IsCCExist = true;
                    break;

                case Enums.ActionType.GovApprove:
                   var poaData3 = _poaBll.GetActivePoaById(lack1Data.CreateBy);
                    if (poaData3 != null)
                    {
                        //creator is poa user
                        rc.To.Add(poaData3.POA_EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(lack1Data.CreateBy);
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovPartialApprove:
                    var poaData4 = _poaBll.GetActivePoaById(lack1Data.CreateBy);
                    if (poaData4 != null)
                    {
                        //creator is poa user
                        rc.To.Add(poaData4.POA_EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lackData.CreatedBy));
                    }
                    else
                    {
                        //creator is excise executive
                        //var userData = _userBll.GetUserById(lackData.CreatedBy);
                        //rc.To.Add(_poaBll.GetById(lackData.ApprovedBy).POA_EMAIL);
                        //rc.To.Add(GetManagerEmail(lackData.ApprovedBy));
                        //rc.CC.Add(userData.EMAIL);
                        var userData = _userBll.GetUserById(lack1Data.CreateBy);
                        rc.To.Add(userData.EMAIL);
                        rc.CC.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                        //first code when manager exists
                        //rc.CC.Add(GetManagerEmail(lackData.ApprovedBy));
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovReject:
                    var poaData5 = _poaBll.GetActivePoaById(lack1Data.CreateBy);
                    if (poaData5 != null)
                    {
                        //first code when manager exists
                        //creator is poa user
                        //rc.To.Add(GetManagerEmail(lackData.CreatedBy));
                        //rc.CC.Add(poaData5.POA_EMAIL);
                        rc.To.Add(poaData5.POA_EMAIL);
                    }
                    else
                    {
                        //creator is excise executive
                        var userData = _userBll.GetUserById(lack1Data.CreateBy);

                        rc.To.Add(_poaBll.GetById(lack1Data.ApprovedByPoa).POA_EMAIL);
                        //first code when manager exists
                        //rc.To.Add(GetManagerEmail(lackData.ApprovedBy));
                        rc.CC.Add(userData.EMAIL);
                    }
                    rc.IsCCExist = true;
                    break;

            }
            //delegatemail
            var inputDelegate = new GetEmailDelegateUserInput();
            inputDelegate.FormType = Enums.FormType.LACK1;
            inputDelegate.FormId = lack1Data.Lack1Id;
            inputDelegate.FormNumber = lack1Data.Lack1Number;
            inputDelegate.ActionType = input.ActionType;

            inputDelegate.CurrentUser = input.UserId;
            inputDelegate.CreatedUser = lack1Data.CreateBy;
            inputDelegate.Date = DateTime.Now;

            inputDelegate.WorkflowHistoryDto = rejected;
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

        /* CR-2 : 2015-12-22 Remove manager approve */
        //private string GetManagerEmail(string poaId)
        //{
        //    var managerId = _poaBll.GetManagerIdByPoaId(poaId);
        //    var managerDetail = _userBll.GetUserById(managerId);
        //    return managerDetail.EMAIL;
        //}

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

        public Lack1GeneratedOutput GenerateLack1DataByParam(Lack1GenerateDataParamInput input)
        {
            return GenerateLack1Data(input);
        }

        public Lack1PrintOutDto GetPrintOutData(int id)
        {
            var dbData = _lack1Service.GetDetailsById(id);
            var dtToReturn = Mapper.Map<Lack1PrintOutDto>(dbData);
            dtToReturn.FusionSummaryProductionByProdTypeList =
                GetProductionDetailSummaryByProdType(dtToReturn.Lack1ProductionDetail);

            //process separate between tis to tis and tis to fa from production detail
            if (dtToReturn.Lack1ProductionDetail != null && dtToReturn.Lack1ProductionDetail.Count > 0)
            {
                var tisToTisData =
                    dtToReturn.Lack1ProductionDetail.Where(c => c.IS_TISTOTIS_DATA.HasValue && c.IS_TISTOTIS_DATA.Value)
                        .ToList();

                var tisToFaData =
                    dtToReturn.Lack1ProductionDetail.Where(c => !c.IS_TISTOTIS_DATA.HasValue || !c.IS_TISTOTIS_DATA.Value)
                        .ToList();

                dtToReturn.InventoryProductionTisToFa = new Lack1InventoryAndProductionDto()
                {
                    ProductionData = new Lack1ProductionDto()
                    {
                        ProductionList = tisToFaData,
                        ProductionSummaryByProdTypeList = GetProductionDetailSummaryByProdType(tisToFaData)
                    }
                };
                dtToReturn.InventoryProductionTisToTis = new Lack1InventoryAndProductionDto()
                {
                    ProductionData = new Lack1ProductionDto()
                    {
                        ProductionSummaryByProdTypeList = GetProductionDetailSummaryByProdType(tisToTisData),
                        ProductionList = tisToTisData
                    }
                };
            }

            if (dtToReturn.Lack1Pbck1Mapping.Count > 0)
            {
                var monthList = _monthBll.GetAll();
                for (int i = 0; i < dtToReturn.Lack1Pbck1Mapping.Count; i++)
                {
                    if (dtToReturn.Lack1Pbck1Mapping[i].DECREE_DATE.HasValue)
                    {
                        dtToReturn.Lack1Pbck1Mapping[i].DisplayDecreeDate = SetDisplayPbck1DecreeDate(monthList,
                            dtToReturn.Lack1Pbck1Mapping[i].DECREE_DATE.Value);
                    }
                    else
                    {
                        dtToReturn.Lack1Pbck1Mapping[i].DisplayDecreeDate = "";
                    }
                }
            }


            //set header footer data by CompanyCode and FormTypeId
            var headerFooterData = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput()
            {
                FormTypeId = Enums.FormType.LACK1,
                CompanyCode = dbData.BUKRS
            });

            dtToReturn.HeaderFooter = headerFooterData;

            if (dtToReturn.SubmissionDate.HasValue)
            {
                int monthId = dtToReturn.SubmissionDate.Value.Month;
                int year = dtToReturn.SubmissionDate.Value.Year;
                var monthData = _monthBll.GetMonth(monthId);
                if (monthData != null)
                {
                    dtToReturn.SubmissionDateDisplayString = dtToReturn.SubmissionDate.Value.Day + " " +
                                                             monthData.MONTH_NAME_IND + " " + year;
                }
            }

            var userCreator = _userBll.GetUserById(dtToReturn.CreateBy);
            if (userCreator != null)
            {
                dtToReturn.ExcisableExecutiveCreator = userCreator.FIRST_NAME + " " + userCreator.LAST_NAME;
            }

            //set NppbkcCity
            if (dtToReturn.Lack1Level == Enums.Lack1Level.Nppbkc)
            {
                //get main plant
                var mainPlant = _t001WServices.GetMainPlantByNppbkcId(dtToReturn.NppbkcId);
                if (mainPlant != null)
                {
                    dtToReturn.NppbkcCity = mainPlant.ORT01;
                }
            }
            else
            {
                //get by plant id
                var plant = _t001WServices.GetById(dtToReturn.LevelPlantId);
                if (plant != null)
                {
                    dtToReturn.NppbkcCity = plant.ORT01;
                }
            }

            if (dtToReturn.AllLack1IncomeDetail == null || dtToReturn.AllLack1IncomeDetail.Count <= 0)
                return dtToReturn;
            dtToReturn.Ck5RemarkData = new Lack1RemarkDto()
            {
                Ck5ReturnData = dtToReturn.AllLack1IncomeDetail.Where(c => c.CK5_TYPE == Enums.CK5Type.Return && c.FLAG_FOR_LACK1).ToList(),
                /*story : http://192.168.62.216/TargetProcess/entity/1637 
                 * Ck5 Manual Trial don't include in remark column, 
                 * see previous function about getting data from ck5 that only include ck5 manual trial if REDUCE_TRIAL value is TRUE
                 */
                Ck5WasteData = dtToReturn.AllLack1IncomeDetail.Where(c => c.CK5_TYPE == Enums.CK5Type.Waste).ToList()
            };
            //set Lack1IncomeDetail
            dtToReturn.Lack1IncomeDetail =
                dtToReturn.AllLack1IncomeDetail.Where(
                    c =>
                        !((c.CK5_TYPE == Enums.CK5Type.Return && c.FLAG_FOR_LACK1) || c.CK5_TYPE == Enums.CK5Type.Waste)).ToList();

            if (string.IsNullOrEmpty(dtToReturn.ExGoodsTypeDesc)) return dtToReturn;

            if (dtToReturn.ExGoodsTypeDesc.ToLower().Contains("alcohol") ||
                dtToReturn.ExGoodsTypeDesc.ToLower().Contains("alkohol"))
            {
                dtToReturn.IsEtilAlcohol = true;
            }

            return dtToReturn;
        }

        public List<Lack1DetailsDto> GetPbck1RealizationList(Lack1GetPbck1RealizationListParamInput input)
        {
            return Mapper.Map<List<Lack1DetailsDto>>(_lack1Service.GetPbck1RealizationList(input));
        }

        #region ----------------Private Method-------------------

        [Obsolete("Old logic, now only from STATUS for regenerate condition")]
        private bool IsNeedToRegenerate(Lack1GenerateDataParamInput input, LACK1 lack1Data)
        {
            if (input.CompanyCode == lack1Data.BUKRS && input.PeriodMonth == lack1Data.PERIOD_MONTH
                && input.PeriodYear == lack1Data.PERIOD_YEAR && input.NppbkcId == lack1Data.NPPBKC_ID &&
                input.ExcisableGoodsType == lack1Data.EX_GOODTYP
                && input.SupplierPlantId == lack1Data.SUPPLIER_PLANT_WERKS)
            {
                //no need to regenerate
                if (input.Lack1Level == Enums.Lack1Level.Plant)
                {
                    var plant = lack1Data.LACK1_PLANT.FirstOrDefault();
                    if (plant != null && plant.PLANT_ID == input.ReceivedPlantId)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        private bool SetChangesHistory(Lack1DetailsDto origin, Lack1DetailsDto data, string userId)
        {
            var rc = false;
            var changesData = new Dictionary<string, bool>
            {
                { "BUKRS", origin.Bukrs == data.Bukrs },
                { "BUTXT", origin.Butxt == data.Butxt },
                { "PERIOD_MONTH", origin.PeriodMonth == data.PeriodMonth },
                { "PERIOD_YEAR", origin.PeriodYears == data.PeriodYears },
                { "SUBMISSION_DATE", origin.SubmissionDate == data.SubmissionDate },
                { "SUPPLIER_PLANT", origin.SupplierPlant == data.SupplierPlant },
                { "SUPPLIER_PLANT_WERKS", origin.SupplierPlantId == data.SupplierPlantId },
                { "SUPPLIER_PLANT_ADDRESS", origin.SupplierPlantAddress == data.SupplierPlantAddress },
                { "EX_GOODTYP", origin.ExGoodsType == data.ExGoodsType },
                { "EX_TYP_DESC", origin.ExGoodsTypeDesc == data.ExGoodsTypeDesc },
                { "WASTE_QTY", origin.WasteQty == data.WasteQty },
                { "WASTE_UOM", origin.WasteUom == data.WasteUom },
                { "RETURN_QTY", origin.ReturnQty == data.ReturnQty },
                { "RETURN_UOM", origin.ReturnUom == data.ReturnUom },
                { "NPPBKC_ID", origin.NppbkcId == data.NppbkcId },
                { "BEGINING_BALANCE", origin.BeginingBalance == data.BeginingBalance },
                { "TOTAL_INCOME", origin.TotalIncome == data.TotalIncome },
                { "USAGE", origin.Usage == data.Usage },
                { "USAGE_TIS_TO_TIS", origin.UsageTisToTis == data.UsageTisToTis },
                { "NOTED", origin.Noted == data.Noted },
                { "DOCUMENT_NOTED", origin.DocumentNoted == data.DocumentNoted },
                { "SUPPLIER_COMPANY_NAME", origin.SupplierCompanyName == data.SupplierCompanyName },
                { "SUPPLIER_COMPANY_CODE", origin.SupplierCompanyCode == data.SupplierCompanyCode }
            };

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
                        case "BUTXT":
                            changes.OLD_VALUE = origin.Butxt;
                            changes.NEW_VALUE = data.Butxt;
                            break;
                        case "PERIOD_MONTH":
                            changes.OLD_VALUE = origin.PeriodMonth.ToString();
                            changes.NEW_VALUE = data.PeriodMonth.ToString();
                            break;
                        case "PERIOD_YEAR":
                            changes.OLD_VALUE = origin.PeriodYears.ToString();
                            changes.NEW_VALUE = data.PeriodYears.ToString();
                            break;
                        case "SUBMISSION_DATE":
                            changes.OLD_VALUE = origin.SubmissionDate.ToString();
                            changes.NEW_VALUE = data.SubmissionDate.ToString();
                            break;
                        case "SUPPLIER_PLANT":
                            changes.OLD_VALUE = origin.SupplierPlant;
                            changes.NEW_VALUE = data.SupplierPlant;
                            break;
                        case "SUPPLIER_PLANT_WERKS":
                            changes.OLD_VALUE = origin.SupplierPlantId;
                            changes.NEW_VALUE = data.SupplierPlantId;
                            break;
                        case "SUPPLIER_PLANT_ADDRESS":
                            changes.OLD_VALUE = origin.SupplierPlantAddress;
                            changes.NEW_VALUE = data.SupplierPlantAddress;
                            break;
                        case "EX_GOODTYP":
                            changes.OLD_VALUE = origin.ExGoodsType;
                            changes.NEW_VALUE = data.ExGoodsType;
                            break;
                        case "EX_TYP_DESC":
                            changes.OLD_VALUE = origin.ExGoodsTypeDesc;
                            changes.NEW_VALUE = data.ExGoodsTypeDesc;
                            break;
                        case "WASTE_QTY":
                            changes.OLD_VALUE = origin.WasteQty.ToString();
                            changes.NEW_VALUE = data.WasteQty.ToString();
                            break;
                        case "WASTE_UOM":
                            changes.OLD_VALUE = origin.WasteUom;
                            changes.NEW_VALUE = data.WasteUom;
                            break;
                        case "RETURN_QTY":
                            changes.OLD_VALUE = origin.ReturnQty.ToString("N2");
                            changes.NEW_VALUE = data.ReturnQty.ToString("N2");
                            break;
                        case "RETURN_UOM":
                            changes.OLD_VALUE = origin.ReturnUom;
                            changes.NEW_VALUE = data.ReturnUom;
                            break;
                        case "NPPBKC_ID":
                            changes.OLD_VALUE = origin.NppbkcId;
                            changes.NEW_VALUE = data.NppbkcId;
                            break;
                        case "BEGINING_BALANCE":
                            changes.OLD_VALUE = origin.BeginingBalance.ToString("N2");
                            changes.NEW_VALUE = data.BeginingBalance.ToString("N2");
                            break;
                        case "TOTAL_INCOME":
                            changes.OLD_VALUE = origin.TotalIncome.ToString("N2");
                            changes.NEW_VALUE = data.TotalIncome.ToString("N2");
                            break;
                        case "USAGE":
                            changes.OLD_VALUE = origin.Usage.ToString("N2");
                            changes.NEW_VALUE = data.Usage.ToString("N2");
                            break;
                        case "USAGE_TIS_TO_TIS":
                            changes.OLD_VALUE = origin.UsageTisToTis.HasValue ? origin.UsageTisToTis.Value.ToString("N2") : "NULL";
                            changes.NEW_VALUE = data.UsageTisToTis.HasValue
                                ? data.UsageTisToTis.Value.ToString("N2")
                                : "NULL";
                            break;
                        case "NOTED":
                            changes.OLD_VALUE = origin.Noted;
                            changes.NEW_VALUE = data.Noted;
                            break;
                        case "DOCUMENT_NOTED":
                            changes.OLD_VALUE = origin.DocumentNoted;
                            changes.NEW_VALUE = data.DocumentNoted;
                            break;
                        case "SUPPLIER_COMPANY_NAME":
                            changes.OLD_VALUE = origin.SupplierCompanyName;
                            changes.NEW_VALUE = data.SupplierCompanyName;
                            break;
                        case "SUPPLIER_COMPANY_CODE":
                            changes.OLD_VALUE = origin.SupplierCompanyCode;
                            changes.NEW_VALUE = data.SupplierCompanyCode;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                    rc = true;
                }
            }
            return rc;
        }

        private string SetDisplayPbck1DecreeDate(IEnumerable<MONTH> months, DateTime dt)
        {
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            string monthName = "<<undefine month>>";

            var monthData = months.FirstOrDefault(c => c.MONTH_ID == month);
            if (monthData != null)
            {
                monthName = monthData.MONTH_NAME_IND;
            }
            return day + " " + monthName + " " + year;
        }

        private Lack1GeneratedOutput ValidationOnGenerateLack1Data(ref Lack1GenerateDataParamInput input)
        {

            #region Validation

            //Check Excisable Group Type if exists
            var checkExcisableGroupType = _exGroupTypeService.GetGroupTypeDetailByGoodsType(input.ExcisableGoodsType);
            if (checkExcisableGroupType == null)
            {
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound),
                    Data = null
                };
            }

            if (checkExcisableGroupType.EX_GROUP_TYPE_ID == null)
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.ExcisabeGroupTypeNotFound),
                    Data = null
                };

            input.ExGroupTypeId = checkExcisableGroupType.EX_GROUP_TYPE_ID.Value;
            input.ExcisableGoodsTypeDesc = checkExcisableGroupType.ZAIDM_EX_GOODTYP.EXT_TYP_DESC;

            #endregion

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };
        }

        private Lack1GeneratedOutput IsExistLack1Data(Lack1GenerateDataParamInput input)
        {
            //check if already exists with same selection criteria
            var lack1Check = _lack1Service.GetBySelectionCriteria(new Lack1GetBySelectionCriteriaParamInput()
            {
                CompanyCode = input.CompanyCode,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                ReceivingPlantId = input.ReceivedPlantId,
                SupplierPlantId = input.SupplierPlantId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                Lack1Level = input.Lack1Level,
                IsTisToTis = input.IsTisToTis
            });

            if (input.IsCreateNew)
            {
                if (lack1Check != null)
                {
                    return new Lack1GeneratedOutput()
                    {
                        Success = false,
                        ErrorCode = ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria.ToString(),
                        ErrorMessage =
                            EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria),
                        Data = null
                    };
                }
            }
            else
            {
                if (lack1Check != null && lack1Check.LACK1_ID != input.Lack1Id)
                {
                    return new Lack1GeneratedOutput()
                    {
                        Success = false,
                        ErrorCode = ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria.ToString(),
                        ErrorMessage =
                            EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack1DuplicateSelectionCriteria),
                        Data = null
                    };
                }
            }

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };
        }

        private Lack1GeneratedOutput GenerateLack1Data(Lack1GenerateDataParamInput input)
        {
            var isWithTisToTisReport = input.IsTisToTis;

            var outValidation = ValidationOnGenerateLack1Data(ref input);

            if (!outValidation.Success) return outValidation;

            var oReturn = new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                IsWithTisToTisReport = isWithTisToTisReport
            };

            var rc = new Lack1GeneratedDto
            {
                CompanyCode = input.CompanyCode,
                CompanyName = input.CompanyName,
                NppbkcId = input.NppbkcId,
                ExcisableGoodsType = input.ExcisableGoodsType,
                ExcisableGoodsTypeDesc = input.ExcisableGoodsTypeDesc,
                SupplierPlantId = input.SupplierPlantId,
                FusionSummaryProductionList = new List<Lack1GeneratedSummaryProductionDataDto>(),
                BeginingBalance = 0 //set default
            };

            //set begining balance
            rc = SetBeginingBalanceBySelectionCritera(rc, input);

            //set Pbck-1 Data by selection criteria
            rc = SetPbck1DataBySelectionCriteria(rc, input);

            var supplierPlantInfo = _t001WServices.GetById(input.SupplierPlantId);
            if (supplierPlantInfo == null || supplierPlantInfo.NPPBKC_ID != input.NppbkcId)
            {
                //validation here
                if (rc.Pbck1List.Count == 0)
                {
                    return new Lack1GeneratedOutput()
                    {
                        Success = false,
                        ErrorCode = ExceptionCodes.BLLExceptions.Lack1MissingPbck1Selected.ToString(),
                        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack1MissingPbck1Selected),
                        Data = null
                    };
                }
            }

            //Set Income List by selection Criteria
            //from CK5 data
            rc = SetIncomeListBySelectionCriteria(rc, input);

            if (rc.AllIncomeList.Count == 0)
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.MissingIncomeListItem.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingIncomeListItem),
                    Data = null
                };

            var bkcUomId = rc.AllIncomeList.Select(d => d.PackageUomId).First(c => !string.IsNullOrEmpty(c));
            if (string.IsNullOrEmpty(bkcUomId))
            {
                //bkc uom is null or empty
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Ck5PackageUomNullOrEmpty.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Ck5PackageUomNullOrEmpty),
                    Data = null
                };
            }

            var plantIdList = new List<string>();
            if (input.Lack1Level == Enums.Lack1Level.Nppbkc)
            {
                //get plant list by nppbkcid
                var plantList = _t001WServices.GetByNppbkcId(input.NppbkcId);
                if (plantList.Count > 0)
                {
                    plantIdList = plantList.Select(c => c.WERKS).ToList();
                }
            }
            else
            {
                plantIdList = new List<string>() { input.ReceivedPlantId };
            }

            var outProcess = !string.IsNullOrEmpty(input.ExcisableGoodsTypeDesc) && (input.ExcisableGoodsTypeDesc.ToLower().Contains("alkohol") ||
                                              input.ExcisableGoodsTypeDesc.ToLower().Contains("alcohol"))
                ? ProcessGenerateLack1DomesticAlcohol(rc, input, plantIdList, bkcUomId)
                : ProcessGenerateLack1NormalExcisableGoods(rc, input, plantIdList, bkcUomId);

            if (!outProcess.Success) return outProcess;
            rc = outProcess.Data;

            rc.PeriodMonthId = input.PeriodMonth;

            var monthData = _monthBll.GetMonth(rc.PeriodMonthId);
            if (monthData != null)
            {
                rc.PeriodMonthName = monthData.MONTH_NAME_IND;
            }

            rc.PeriodYear = input.PeriodYear;
            //format for noted
            //LOGS POINT 19 : replace with new logic for remark
            //var noteTemp = new List<string>();
            ////format for noted
            //if (!string.IsNullOrEmpty(input.WasteAmountUom))
            //{
            //    var uomWasteAmountDescription = _uomBll.GetById(input.WasteAmountUom);
            //    input.WasteAmountUom = uomWasteAmountDescription.UOM_ID;
            //    noteTemp.Add(GeneratedNoteFormat("Jumlah Waste", input.WasteAmount, uomWasteAmountDescription.UOM_DESC));
            //}

            //if (!string.IsNullOrEmpty(input.ReturnAmountUom))
            //{
            //    var uomReturnDescription = _uomBll.GetById(input.ReturnAmountUom);
            //    input.ReturnAmountUom = uomReturnDescription.UOM_ID;
            //    noteTemp.Add(GeneratedNoteFormat("Jumlah Pengembalian", input.ReturnAmount, uomReturnDescription.UOM_DESC));
            //}

            //rc.DocumentNoted = string.Join(Environment.NewLine, noteTemp).Replace(Environment.NewLine, "<br />");
            rc.Noted = input.Noted;

            //recalculate total usage from income list ck5 type manual and reduce trial true
            var ck5ReduceTrial =
                rc.IncomeList.Where(c => c.Ck5Type == Enums.CK5Type.Manual && c.IsCk5ReduceTrial).ToList();
            if (ck5ReduceTrial.Count > 0)
            {
                rc.TotalUsage = rc.TotalUsage + ck5ReduceTrial.Sum(d => d.Amount);
            }

            rc.EndingBalance = rc.BeginingBalance + rc.TotalIncome - (rc.TotalUsage + (rc.TotalUsageTisToTis.HasValue ? rc.TotalUsageTisToTis.Value : 0)) - (input.ReturnAmount.HasValue ? input.ReturnAmount.Value : 0);

            oReturn.Data = rc;
            oReturn.IsEtilAlcohol = outProcess.IsEtilAlcohol;

            return oReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="input"></param>
        /// <param name="plantIdList"></param>
        /// <param name="bkcUomId"></param>
        /// <returns></returns>
        private Lack1GeneratedOutput ProcessGenerateLack1DomesticAlcohol(Lack1GeneratedDto rc, Lack1GenerateDataParamInput input, List<string> plantIdList, string bkcUomId)
        {
            //add logic here for LACK-1 Etil Alcohol
            //set InventoryMovement
            InvMovementGetForLack1UsageMovementByParamOutput invMovementOutput;
            rc.InventoryProductionTisToFa = new Lack1GeneratedInventoryAndProductionDto();
            var outGenerateLack1InventoryMovement = SetGenerateLack1InventoryMovementForEtilAlcohol(rc, input, plantIdList, out invMovementOutput, bkcUomId);
            if (!outGenerateLack1InventoryMovement.Success) return outGenerateLack1InventoryMovement;

            rc = outGenerateLack1InventoryMovement.Data;

            //Get Prev Inventory Movement
            var prevInventoryMovementByParamInput = new InvMovementGetUsageByParamInput()
            {
                PlantIdList = plantIdList,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                NppbkcId = input.NppbkcId,
                IsTisToTis = input.IsTisToTis,
                IsEtilAlcohol = true
            };

            if (input.PeriodMonth == 1)
            {
                //Year - 1, Month = 12
                prevInventoryMovementByParamInput.PeriodMonth = 12;
                prevInventoryMovementByParamInput.PeriodYear = prevInventoryMovementByParamInput.PeriodYear - 1;
            }
            else
            {
                //Same Year, Month - 1
                prevInventoryMovementByParamInput.PeriodMonth = input.PeriodMonth - 1;
            }

            var stoReceiverNumberList = rc.IncomeList.Select(d => d.DnNumber).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

            var prevInventoryMovementByParam = GetInventoryMovementByParam(prevInventoryMovementByParamInput,
                stoReceiverNumberList, bkcUomId);

            //set production List
            var productionTraceList = new List<Lack1GeneratedProductionDomesticAlcoholDto>();

            var allTrackingList = new List<Lack1GeneratedInvMovementProductionStepTracingItem>();

            //(1)start from 101 level 0, to get 261 level 0
            //(2)get 101 (level 1) base on ORDR and PLANT_ID on lvl 0, 261 and check  material_id on zaidm_ex_material, if exists that's the finish goods and save it as production result
            //(3)if not exists, get 261 lvl 1 base on BATCH and PLANT_ID on lvl 1, 101
            //(4)get 101 (level 2) base on ORDR and PLANT_ID on lvl 1, 261 and check  material_id on zaidm_ex_material, if exists that's the finish goods and save it as production result
            //continue as point (3), stop if finish goods found

            //grouped first by MAT_DOC, MVT, MATERIAL_ID, PLANT_ID, BATCH, ORDR
            var invMovementReceivingListGrouped =
                InvMovementGroupedForProductionStepTracingItem(
                    rc.InventoryProductionTisToFa.InvetoryMovementData.InvMovementReceivingList);

            foreach (var item in invMovementReceivingListGrouped)
            {
                //set for level 0
                item.TrackLevel = 0;
                item.ParentOrdr = item.Ordr;
                item.ProductionQty = item.Qty;
                item.IsFinalGoodsType = false;

                //get tracing data
                var itemToInsert = new Lack1GeneratedProductionDomesticAlcoholDto()
                {
                    InvMovementUsage = item,
                    InvMovementProductionStepTracing = new List<Lack1GeneratedInvMovementProductionStepTracingItem>()
                };

                var traceItems = GetUsageEtilAlcoholProdTrace(item.ParentOrdr, 0, item.Batch, item.PlantId,
                    input.PeriodMonth, input.PeriodYear, bkcUomId).ToList();

                if (traceItems.Count > 0)
                {
                    item.ParentOrdr = traceItems.First().ParentOrdr;
                    itemToInsert.InvMovementProductionStepTracing.Add(item);
                    itemToInsert.InvMovementProductionStepTracing.AddRange(traceItems);
                }
                else
                {
                    item.ProductionQty = 0;
                    item.IsFinalGoodsType = true;
                    itemToInsert.InvMovementProductionStepTracing.Add(item);
                }

                productionTraceList.Add(itemToInsert);
                allTrackingList.AddRange(itemToInsert.InvMovementProductionStepTracing);
            }

            //process the production list got from previous process
            var finalGoodsList = allTrackingList.Where(c => c.IsFinalGoodsType).ToList();

            var productionList = new List<Lack1GeneratedProductionDataDto>();

            foreach (var item in finalGoodsList)
            {
                var itemToInsert = new Lack1GeneratedProductionDataDto()
                {
                    FaCode = item.MaterialId,
                    Ordr = item.Ordr,
                    ProdCode = "", //from ?
                    ProductType = "",//from ?
                    ProductAlias = "",//from?
                    Amount = item.ProductionQty,
                    UomId = item.UomId,
                    UomDesc = item.UomDesc
                };

                //Get product type info
                var prodType = _goodProdTypeService.GetProdCodeByGoodTypeId(item.ExGoodsTypeId);
                if (prodType != null)
                {
                    itemToInsert.ProdCode = prodType.PROD_CODE;
                    itemToInsert.ProductAlias = prodType.PRODUCT_ALIAS;
                    itemToInsert.ProductType = prodType.PRODUCT_TYPE;
                }
                else
                {
                    return new Lack1GeneratedOutput()
                    {
                        Data = null,
                        ErrorCode = ExceptionCodes.BLLExceptions.GoodsProdTypeMappingNotFound.ToString(),
                        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.GoodsProdTypeMappingNotFound),
                        Success = false
                    };
                }

                var rec = invMovementOutput.UsageProportionalList.FirstOrDefault(c =>
                    c.Order == item.ParentOrdr);
                if (rec != null)
                {
                    //calculate proporsional
                    itemToInsert.Amount =
                        Math.Round(
                            ((rec.Qty / rec.TotalQtyPerMaterialId) * itemToInsert.Amount), 5);
                }
                else
                {
                    //check in prev data inventory_movement
                    rec =
                        prevInventoryMovementByParam.UsageProportionalList.FirstOrDefault(
                            c => c.Order == item.ParentOrdr);

                    if (rec != null)
                    {
                        //calculate proporsional from prev inventory movement
                        itemToInsert.Amount =
                            Math.Round(
                                ((rec.Qty / rec.TotalQtyPerMaterialId) * itemToInsert.Amount), 5);
                    }
                }

                productionList.Add(itemToInsert);

            }

            rc.AlcoholTrackingList = allTrackingList;
            //set to tis to fa
            rc.InventoryProductionTisToFa.ProductionData = new Lack1GeneratedProductionDto
            {
                ProductionList = productionList,
                ProductionSummaryByProdTypeList = GetProductionGroupedByProdTypeList(productionList),
                SummaryProductionList = GetSummaryGroupedProductionList(productionList)
            };

            rc.FusionSummaryProductionList = rc.InventoryProductionTisToFa.ProductionData.SummaryProductionList;

            return new Lack1GeneratedOutput()
            {
                Data = rc,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Success = true,
                IsEtilAlcohol = true
            };
        }

        private IEnumerable<Lack1GeneratedInvMovementProductionStepTracingItem> GetUsageEtilAlcoholProdTrace(string parentOrdr, int trackLevel, string batch, string plantId, int periodMonth, int periodYear, string bkcUomId)
        {
            var traceItems = new List<Lack1GeneratedInvMovementProductionStepTracingItem>();

            var usageList =
                _inventoryMovementService.GetUsageByBatchAndPlantIdInPeriod(
                    new GetUsageByBatchAndPlantIdInPeriodParamInput()
                    {
                        Batch = batch,
                        PlantId = plantId,
                        PeriodYear = periodYear,
                        PeriodMonth = periodMonth
                    });

            var usageListWithConvertion = InvMovementConvertionProcess(usageList, bkcUomId);

            var groupedUsageList = InvMovementGroupedForProductionStepTracingItem(usageListWithConvertion).ToList();

            parentOrdr = trackLevel == 0 && groupedUsageList.Count > 0 ? groupedUsageList.First().Ordr : parentOrdr;

            foreach (var item in groupedUsageList)
            {
                item.TrackLevel = trackLevel;
                item.IsFinalGoodsType = false;
                item.ParentOrdr = parentOrdr;
                item.ProductionQty = item.Qty;

                var receivingList = GetReceivingEtilAlcoholProdTrace(parentOrdr, (trackLevel + 1), item.Ordr, plantId,
                    periodMonth, periodYear, bkcUomId).ToList();

                if (receivingList.Count <= 0)
                {
                    //tidak ada next 101 nya, jadi harusnya hasil produksi nya adalah 101 sebelumnya
                    //bagaimana caranya ?
                    //sebenarnya cuma nge-set isFinalGoods nya aja sih ya ? atau dari max level nya ? bisa bisa :-)
                    item.IsFinalGoodsType = true;
                    item.ProductionQty = 0;
                    traceItems.Add(item);
                }
                else
                {
                    traceItems.Add(item);
                    traceItems.AddRange(receivingList);
                }

            }

            return traceItems;

        }

        private IEnumerable<Lack1GeneratedInvMovementProductionStepTracingItem> GetReceivingEtilAlcoholProdTrace(string parentOrdr, int trackLevel, string ordr, string plantId, int periodMonth, int periodYear, string bkcUomId)
        {
            var traceItems = new List<Lack1GeneratedInvMovementProductionStepTracingItem>();
            var receivingList =
                _inventoryMovementService.GetReceivingByOrderAndPlantIdInPeriod(
                    new GetReceivingByOrderAndPlantIdInPeriodParamInput()
                    {
                        Ordr = ordr,
                        PlantId = plantId,
                        PeriodYear = periodYear,
                        PeriodMonth = periodMonth
                    });

            var receivingListWithConvertion = InvMovementConvertionProcess(receivingList, bkcUomId);

            var groupedReceivingList = InvMovementGroupedForProductionStepTracingItem(receivingListWithConvertion);

            foreach (var item in groupedReceivingList)
            {
                //check to material
                var chkMaterial = _materialService.GetByMaterialAndPlantId(
                    item.MaterialId, item.PlantId);
                item.TrackLevel = trackLevel;
                item.ParentOrdr = parentOrdr;
                item.ProductionQty = item.Qty;

                if (chkMaterial != null)
                {
                    //exists in zaidm_ex_material = final goods
                    item.IsFinalGoodsType = true;
                    item.ExGoodsTypeId = chkMaterial.EXC_GOOD_TYP;
                    item.UomId = chkMaterial.BASE_UOM_ID;
                    item.UomDesc = chkMaterial.UOM != null ? chkMaterial.UOM.UOM_DESC : string.Empty;
                    traceItems.Add(item);
                }
                else
                {
                    //not exists in zaidm_ex_material = continue get 261
                    item.IsFinalGoodsType = false;
                    var usageList = GetUsageEtilAlcoholProdTrace(parentOrdr, trackLevel, item.Batch, plantId, periodMonth, periodYear, bkcUomId).ToList();
                    if (usageList.Count <= 0)
                    {
                        //set prodution qty to zero cause of no more usage at next level
                        //and set this item as final goods
                        item.IsFinalGoodsType = true;
                        item.ProductionQty = 0;
                        traceItems.Add(item);
                    }
                    else
                    {
                        traceItems.Add(item);
                        traceItems.AddRange(usageList);
                    }

                }
            }

            return traceItems;

        }

        private IEnumerable<Lack1GeneratedInvMovementProductionStepTracingItem>
            InvMovementGroupedForProductionStepTracingItem(IEnumerable<InvMovementItemWithConvertion> invMovements)
        {
            return invMovements.GroupBy(p => new
            {
                p.MAT_DOC,
                p.MVT,
                p.MATERIAL_ID,
                p.PLANT_ID,
                p.BATCH,
                p.ORDR
            }).Select(g => new Lack1GeneratedInvMovementProductionStepTracingItem()
            {
                Mvt = g.Key.MVT,
                MaterialId = g.Key.MATERIAL_ID,
                PlantId = g.Key.PLANT_ID,
                Batch = g.Key.BATCH,
                Ordr = g.Key.ORDR,
                Bun = g.First().BUN,
                MatDoc = g.Key.MAT_DOC,
                PurchDoc = g.First().PURCH_DOC,
                PostingDate = g.First().POSTING_DATE,
                Qty = g.Sum(p => p.QTY.HasValue ? p.QTY.Value : 0),
                ConvertedUomDesc = g.First().ConvertedUomDesc,
                ConvertedUomId = g.First().ConvertedUomId,
                ConvertedQty = g.Sum(p => p.ConvertedQty)
            }).ToList();
        }

        private IEnumerable<Lack1GeneratedInvMovementProductionStepTracingItem> InvMovementGroupedForProductionStepTracingItem(
            IEnumerable<Lack1GeneratedTrackingDto> invMovements)
        {
            return invMovements.GroupBy(p => new
            {
                p.MAT_DOC,
                p.MVT,
                p.MATERIAL_ID,
                p.PLANT_ID,
                p.BATCH,
                p.ORDR
            }).Select(g => new Lack1GeneratedInvMovementProductionStepTracingItem()
            {
                Mvt = g.Key.MVT,
                MaterialId = g.Key.MATERIAL_ID,
                MatDoc = g.Key.MAT_DOC,
                PlantId = g.Key.PLANT_ID,
                Batch = g.Key.BATCH,
                Ordr = g.Key.ORDR,
                Bun = g.First().BUN,
                PurchDoc = g.First().PURCH_DOC,
                PostingDate = g.First().POSTING_DATE,
                Qty = g.Sum(p => p.QTY.HasValue ? p.QTY.Value : 0),
                ConvertedUomDesc = g.First().ConvertedUomDesc,
                ConvertedQty = g.Sum(p => p.ConvertedQty),
                ConvertedUomId = g.First().ConvertedUomId
            }).ToList();
        }

        private Lack1GeneratedOutput ProcessGenerateLack1NormalExcisableGoods(Lack1GeneratedDto rc,
            Lack1GenerateDataParamInput input, List<string> plantIdList, string bkcUomId)
        {
            //instantiate
            rc.InventoryProductionTisToFa = new Lack1GeneratedInventoryAndProductionDto();
            rc.InventoryProductionTisToTis = new Lack1GeneratedInventoryAndProductionDto();

            //Get InventoryMovement for Tis To Fa
            InvMovementGetForLack1UsageMovementByParamOutput invMovementTisToFaOutput;
            var outGenerateLack1InventoryMovementTisToFa = SetGenerateLack1InventoryMovement(rc, input, plantIdList, false, out invMovementTisToFaOutput, bkcUomId);
            if (!outGenerateLack1InventoryMovementTisToFa.Success) return outGenerateLack1InventoryMovementTisToFa;

            //normal report, normal logic
            if (invMovementTisToFaOutput.IncludeInCk5List.Count == 0)
            {
                //no usage receiving
                var prodDataOutTisToFa = SetProductionListWithoutUsageReceiving(rc, input, plantIdList, bkcUomId);
                if (!prodDataOutTisToFa.Success) return prodDataOutTisToFa;

                rc = prodDataOutTisToFa.Data;
            }
            else
            {
                var prodDataOutTisToFa = SetProductionList(rc, input, plantIdList, invMovementTisToFaOutput, bkcUomId);
                if (!prodDataOutTisToFa.Success) return prodDataOutTisToFa;

                rc = prodDataOutTisToFa.Data;
            }

            rc.FusionSummaryProductionList.AddRange(rc.InventoryProductionTisToFa.ProductionData.SummaryProductionList);

            if (input.IsTisToTis)
            {

                //Get InventoryMovement for Tis To Tis
                InvMovementGetForLack1UsageMovementByParamOutput invMovementTisToTisOutput;
                var outGenerateLack1InventoryMovementTisToTis = SetGenerateLack1InventoryMovement(rc, input, plantIdList, true, out invMovementTisToTisOutput, bkcUomId);
                if (!outGenerateLack1InventoryMovementTisToTis.Success) return outGenerateLack1InventoryMovementTisToTis;

                //set Production tis to tis
                //tis to tis, get from PBCK-1 PROD CONVERTER
                var prodDataOut = SetProductionListForTisToTis(rc, input);
                if (!prodDataOut.Success) return prodDataOut;

                rc = prodDataOut.Data;

                if (rc.InventoryProductionTisToTis.ProductionData != null)
                    rc.FusionSummaryProductionList.AddRange(rc.InventoryProductionTisToTis.ProductionData.SummaryProductionList);
            }

            //process FusionSummaryProductionList
            if (rc.FusionSummaryProductionList != null && rc.FusionSummaryProductionList.Count > 0)
            {
                rc.FusionSummaryProductionList = GetFusionSummaryGroupedProductionList(rc.FusionSummaryProductionList);
            }

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Data = rc,
                IsEtilAlcohol = false
            };

        }

        private List<Lack1GeneratedSummaryProductionDataDto> GetFusionSummaryGroupedProductionList(List<Lack1GeneratedSummaryProductionDataDto> list)
        {
            if (list.Count <= 0) return new List<Lack1GeneratedSummaryProductionDataDto>();
            var groupedData = list.GroupBy(p => new
            {
                p.UomId,
                p.UomDesc
            }).Select(g => new Lack1GeneratedSummaryProductionDataDto()
            {
                UomId = g.Key.UomId,
                UomDesc = g.Key.UomDesc,
                Amount = g.Sum(p => p.Amount)
            });

            return groupedData.ToList();
        }

        //private string GeneratedNoteFormat(string prefix, decimal? nominal, string uomId)
        //{
        //    if (nominal.HasValue)
        //        return prefix + " : " + nominal.Value.ToString("N2") + " " + uomId;
        //    return "";
        //}

        /// <summary>
        /// for normal LACK-1 Production Data
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="input"></param>
        /// <param name="plantIdList"></param>
        /// <param name="invMovementOutput"></param>
        /// <param name="bkcUomId"></param>
        /// <returns></returns>
        private Lack1GeneratedOutput SetProductionList(Lack1GeneratedDto rc, Lack1GenerateDataParamInput input, List<string> plantIdList,
            InvMovementGetForLack1UsageMovementByParamOutput invMovementOutput, string bkcUomId)
        {
            //get Ck4CItem
            var ck4CItemInput = Mapper.Map<CK4CItemGetByParamInput>(input);
            ck4CItemInput.IsHigherFromApproved = false;
            ck4CItemInput.IsCompletedOnly = true;
            var ck4CItemData = _ck4cItemService.GetByParam(ck4CItemInput);

            //by pass : http://192.168.62.216/TargetProcess/entity/1465
            //if (ck4CItemData.Count == 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.MissingProductionList.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingProductionList),
            //        Data = null
            //    };
            //}

            var zaapShiftReportInput = new ZaapShiftRptGetForLack1ByParamInput()
            {
                CompanyCode = input.CompanyCode,
                Werks = plantIdList,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                FaCodeList = ck4CItemData.Select(d => d.FA_CODE).Distinct().ToList()
            };

            //get zaap_shift_rpt
            var zaapShiftRpt = _zaapShiftRptService.GetForLack1ByParam(zaapShiftReportInput);

            //bypass http://192.168.62.216/TargetProcess/entity/1465
            //if (zaapShiftRpt.Count == 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.MissingProductionList.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingProductionList),
            //        Data = null
            //    };
            //}

            var prodTypeData = _prodTypeService.GetAll();

            //join data ck4cItem and ZaapShiftRpt
            var joinedData = (from zaap in zaapShiftRpt
                              join ck4CItem in ck4CItemData on new { zaap.WERKS, zaap.FA_CODE } equals
                                   new { ck4CItem.WERKS, ck4CItem.FA_CODE }
                              join prod in prodTypeData on new { ck4CItem.PROD_CODE } equals new { prod.PROD_CODE }
                              select new
                              {
                                  zaap.FA_CODE,
                                  zaap.WERKS,
                                  zaap.COMPANY_CODE,
                                  zaap.UOM,
                                  zaap.PRODUCTION_DATE,
                                  zaap.BATCH,
                                  zaap.QTY,
                                  zaap.ORDR,
                                  ck4CItem.PROD_CODE,
                                  prod.PRODUCT_ALIAS,
                                  prod.PRODUCT_TYPE
                              }).Distinct().ToList();

            //bypass http://192.168.62.216/TargetProcess/entity/1465
            //if (joinedData.Count == 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.MissingProductionList.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingProductionList),
            //        Data = null
            //    };
            //}

            var productionList = new List<Lack1GeneratedProductionDataDto>();
            var uomData = _uomBll.GetAll();

            var joinedWithUomData = (from j in joinedData
                                     join u in uomData on j.UOM equals u.UOM_ID
                                     select new
                                     {
                                         j.FA_CODE,
                                         j.WERKS,
                                         j.COMPANY_CODE,
                                         j.UOM,
                                         j.PRODUCTION_DATE,
                                         j.BATCH,
                                         j.QTY,
                                         j.ORDR,
                                         j.PROD_CODE,
                                         j.PRODUCT_ALIAS,
                                         j.PRODUCT_TYPE,
                                         u.UOM_DESC
                                         
                                     }).Distinct().ToList();

            //Get Prev Inventory Movement
            var prevInventoryMovementByParamInput = new InvMovementGetUsageByParamInput()
            {
                PlantIdList = plantIdList,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                NppbkcId = input.NppbkcId,
                IsTisToTis = input.IsTisToTis
            };

            if (input.PeriodMonth == 1)
            {
                //Year - 1, Month = 12
                prevInventoryMovementByParamInput.PeriodMonth = 12;
                prevInventoryMovementByParamInput.PeriodYear = prevInventoryMovementByParamInput.PeriodYear - 1;
            }
            else
            {
                //Same Year, Month - 1
                prevInventoryMovementByParamInput.PeriodMonth = input.PeriodMonth - 1;
            }

            var stoReceiverNumberList = rc.IncomeList.Select(d => d.Ck5Type == Enums.CK5Type.Intercompany ? d.StoReceiverNumber : d.StoSenderNumber).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

            var prevInventoryMovementByParam = GetInventoryMovementByParam(prevInventoryMovementByParamInput,
                stoReceiverNumberList, bkcUomId);

            //calculation proccess
            foreach (var item in joinedWithUomData)
            {
                var itemToInsert = new Lack1GeneratedProductionDataDto()
                {
                    FaCode = item.FA_CODE,
                    Ordr = item.ORDR,
                    ProdCode = item.PROD_CODE,
                    ProductType = item.PRODUCT_TYPE,
                    ProductAlias = item.PRODUCT_ALIAS,
                    Amount = item.QTY.HasValue ? item.QTY.Value : 0,
                    UomId = item.UOM,
                    UomDesc = item.UOM_DESC
                };

                var rec = invMovementOutput.UsageProportionalList.FirstOrDefault(c =>
                    c.Order == item.ORDR);

                if (rec != null)
                {
                    //calculate proporsional
                    itemToInsert.Amount =
                        Math.Round(
                            ((rec.Qty / rec.TotalQtyPerMaterialId) * itemToInsert.Amount), 3);
                }
                else
                {
                    if (prevInventoryMovementByParam.UsageProportionalList.Count > 0)
                    {
                        var chk =
                            prevInventoryMovementByParam.UsageProportionalList.FirstOrDefault(
                                c => c.Order == item.ORDR);
                        if (chk != null)
                        {
                            //produksi lintas bulan, di proporsional kan jika ketemu ordr nya
                            itemToInsert.Amount =
                        Math.Round(
                            ((chk.Qty / chk.TotalQtyPerMaterialId) * itemToInsert.Amount), 3);
                        }
                    }
                }

                productionList.Add(itemToInsert);
            }

            //set to Normal Data
            rc.InventoryProductionTisToFa.ProductionData = new Lack1GeneratedProductionDto
            {
                ProductionList = productionList,
                ProductionSummaryByProdTypeList = GetProductionGroupedByProdTypeList(productionList),
                SummaryProductionList = GetSummaryGroupedProductionList(productionList)
            };

            //calculate summary by UOM ID

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Data = rc
            };
        }

        /// <summary>
        /// for Tis To Fa Data
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="input"></param>
        /// <param name="plantIdList"></param>
        /// <param name="bkcUomId"></param>
        /// <returns></returns>
        private Lack1GeneratedOutput SetProductionListWithoutUsageReceiving(Lack1GeneratedDto rc, Lack1GenerateDataParamInput input, List<string> plantIdList, string bkcUomId)
        {
            //get Ck4CItem
            var ck4CItemInput = Mapper.Map<CK4CItemGetByParamInput>(input);
            ck4CItemInput.IsHigherFromApproved = false;
            ck4CItemInput.IsCompletedOnly = true;
            var ck4CItemData = _ck4cItemService.GetByParam(ck4CItemInput);

            //by pass : http://192.168.62.216/TargetProcess/entity/1465
            //if (ck4CItemData.Count == 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.MissingProductionList.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingProductionList),
            //        Data = null
            //    };
            //}

            var zaapShiftReportInput = new ZaapShiftRptGetForLack1ByParamInput()
            {
                CompanyCode = input.CompanyCode,
                Werks = plantIdList,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                FaCodeList = ck4CItemData.Select(d => d.FA_CODE).Distinct().ToList()
            };

            //get zaap_shift_rpt
            var zaapShiftRpt = _zaapShiftRptService.GetForLack1ByParam(zaapShiftReportInput);

            //by pass : http://192.168.62.216/TargetProcess/entity/1465
            //if (zaapShiftRpt.Count == 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.MissingProductionList.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingProductionList),
            //        Data = null
            //    };
            //}

            var prodTypeData = _prodTypeService.GetAll();

            //join data ck4cItem and ZaapShiftRpt
            var joinedData = (from zaap in zaapShiftRpt
                              join ck4CItem in ck4CItemData on new { zaap.WERKS, zaap.FA_CODE } equals
                                   new { ck4CItem.WERKS, ck4CItem.FA_CODE }
                              join prod in prodTypeData on new { ck4CItem.PROD_CODE } equals new { prod.PROD_CODE }
                              select new
                              {
                                  zaap.FA_CODE,
                                  zaap.WERKS,
                                  zaap.COMPANY_CODE,
                                  zaap.UOM,
                                  zaap.PRODUCTION_DATE,
                                  zaap.BATCH,
                                  zaap.QTY,
                                  zaap.ORDR,
                                  ck4CItem.PROD_CODE,
                                  prod.PRODUCT_ALIAS,
                                  prod.PRODUCT_TYPE
                              }).Distinct().ToList();

            //by pass : http://192.168.62.216/TargetProcess/entity/1465
            //if (joinedData.Count == 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.MissingProductionList.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingProductionList),
            //        Data = null
            //    };
            //}

            var productionList = new List<Lack1GeneratedProductionDataDto>();
            var uomData = _uomBll.GetAll();

            var joinedWithUomData = (from j in joinedData
                                     join u in uomData on j.UOM equals u.UOM_ID
                                     select new
                                     {
                                         j.FA_CODE,
                                         j.WERKS,
                                         j.COMPANY_CODE,
                                         j.UOM,
                                         j.PRODUCTION_DATE,
                                         j.BATCH,
                                         j.QTY,
                                         j.ORDR,
                                         j.PROD_CODE,
                                         j.PRODUCT_ALIAS,
                                         j.PRODUCT_TYPE,
                                         u.UOM_DESC
                                     }).Distinct().ToList();

            //Get Prev Inventory Movement
            var prevInventoryMovementByParamInput = new InvMovementGetUsageByParamInput()
            {
                PlantIdList = plantIdList,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                NppbkcId = input.NppbkcId,
                IsTisToTis = input.IsTisToTis
            };

            if (input.PeriodMonth == 1)
            {
                //Year - 1, Month = 12
                prevInventoryMovementByParamInput.PeriodMonth = 12;
                prevInventoryMovementByParamInput.PeriodYear = prevInventoryMovementByParamInput.PeriodYear - 1;
            }
            else
            {
                //Same Year, Month - 1
                prevInventoryMovementByParamInput.PeriodMonth = input.PeriodMonth - 1;
            }

            var stoReceiverNumberList = rc.IncomeList.Select(d => d.Ck5Type == Enums.CK5Type.Intercompany ? d.StoReceiverNumber : d.StoSenderNumber).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

            var prevInventoryMovementByParam = GetInventoryMovementByParam(prevInventoryMovementByParamInput,
                stoReceiverNumberList, bkcUomId);

            if (prevInventoryMovementByParam.UsageProportionalList.Count > 0)
            {
                //need to proportional or not base on ORDR
                //calculation proccess
                foreach (var item in joinedWithUomData)
                {
                    var itemToInsert = new Lack1GeneratedProductionDataDto()
                    {
                        FaCode = item.FA_CODE,
                        Ordr = item.ORDR,
                        ProdCode = item.PROD_CODE,
                        ProductType = item.PRODUCT_TYPE,
                        ProductAlias = item.PRODUCT_ALIAS,
                        Amount = item.QTY.HasValue ? item.QTY.Value : 0,
                        UomId = item.UOM,
                        UomDesc = item.UOM_DESC
                    };

                    var chk =
                             prevInventoryMovementByParam.UsageProportionalList.FirstOrDefault(
                                 c => c.Order == item.ORDR);
                    if (chk != null)
                    {
                        //produksi lintas bulan, di proporsional kan jika ketemu ordr nya
                        //old logic, hard coded about UOM
                        //var totalUsageInCk5PrevPeriod = (-1) * prevInventoryMovementByParam.IncludeInCk5List.Sum(d => d.QTY.HasValue ? (!string.IsNullOrEmpty(d.BUN) && d.BUN.ToLower() == "kg" ? d.QTY.Value * 1000 : d.QTY.Value) : 0);
                        //var totalUsageExcludeCk5PrevPeriod = (-1) * prevInventoryMovementByParam.ExcludeFromCk5List.Sum(d => d.QTY.HasValue ? (!string.IsNullOrEmpty(d.BUN) && d.BUN.ToLower() == "kg" ? d.QTY.Value * 1000 : d.QTY.Value) : 0);
                        //    var totalUsageInCk5PrevPeriod = (-1) * prevInventoryMovementByParam.IncludeInCk5List.Sum(d => d.ConvertedQty);
                        //    var totalUsageExcludeCk5PrevPeriod = (-1) * prevInventoryMovementByParam.ExcludeFromCk5List.Sum(d => d.ConvertedQty);
                        //    var totalUsagePrevPeriod = totalUsageInCk5PrevPeriod + totalUsageExcludeCk5PrevPeriod;

                        //    itemToInsert.Amount =
                        //Math.Round(
                        //    ((totalUsageInCk5PrevPeriod / totalUsagePrevPeriod) * itemToInsert.Amount), 3);

                        //calculate proporsional
                        //produksi lintas bulan, di proporsional kan jika ketemu ordr nya
                        itemToInsert.Amount =
                    Math.Round(
                        ((chk.Qty / chk.TotalQtyPerMaterialId) * itemToInsert.Amount), 3);
                    }

                    productionList.Add(itemToInsert);
                }
            }
            else
            {
                //100% production result
                productionList.AddRange(joinedWithUomData.Select(item => new Lack1GeneratedProductionDataDto()
                {
                    FaCode = item.FA_CODE,
                    Ordr = item.ORDR,
                    ProdCode = item.PROD_CODE,
                    ProductType = item.PRODUCT_TYPE,
                    ProductAlias = item.PRODUCT_ALIAS,
                    Amount = item.QTY.HasValue ? item.QTY.Value : 0,
                    UomId = item.UOM,
                    UomDesc = item.UOM_DESC
                }));
            }


            //set to Tis To Fa data
            rc.InventoryProductionTisToFa.ProductionData = new Lack1GeneratedProductionDto
            {
                ProductionList = productionList,
                ProductionSummaryByProdTypeList = GetProductionGroupedByProdTypeList(productionList),
                SummaryProductionList = GetSummaryGroupedProductionList(productionList)//calculate summary by UOM ID
            };

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Data = rc
            };
        }

        /// <summary>
        /// For Tis To Tis Data
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private Lack1GeneratedOutput SetProductionListForTisToTis(Lack1GeneratedDto rc, Lack1GenerateDataParamInput input)
        {
            var t001WSupplierInfo = _t001WServices.GetById(input.SupplierPlantId);
            if (t001WSupplierInfo != null) input.SupplierPlantNppbkcId = t001WSupplierInfo.NPPBKC_ID;
            var pbck1ProdConverter =
                _pbck1ProdConverterService.GetProductionLack1TisToTis(new Pbck1GetProductionLack1TisToTisParamInput()
                {
                    NppbkcId = input.NppbkcId,
                    ExcisableGoodsTypeId = input.ExcisableGoodsType,
                    SupplierPlantId = input.SupplierPlantId,
                    SupplierPlantNppbkcId = input.SupplierPlantNppbkcId,
                    PeriodMonth = input.PeriodMonth,
                    PeriodYear = input.PeriodYear
                });

            //just for testing, bypass this validation
            if (pbck1ProdConverter == null || pbck1ProdConverter.Count == 0)
            {
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.Lack1MissingPbckProdConverter.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.Lack1MissingPbckProdConverter),
                    Data = rc
                };
            }

            var uomData = _uomBll.GetAll();

            if (uomData.Count <= 0)
            {
                return new Lack1GeneratedOutput()
                {
                    Success = false,
                    ErrorCode = ExceptionCodes.BLLExceptions.MissingUomData.ToString(),
                    ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingUomData),
                    Data = rc
                };
            }

            //old code, remove harcoded uom on tis to tis production list
            //var gramUomData = uomData.Count > 0
            //    ? uomData.FirstOrDefault(c => c.UOM_ID.ToLower() == "g" || c.UOM_DESC.ToLower() == "gram")
            //    : null;

            //if (gramUomData == null)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.MissingUomData.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MissingUomData),
            //        Data = rc
            //    };
            //}

            //old code, remove harcoded uom on tis to tis production list
            //var joinedWithUomData = (from j in pbck1ProdConverter
            //                         join u in uomData on j.CONVERTER_UOM_ID equals u.UOM_ID
            //                         select new
            //                         {
            //                             j.PROD_CODE,
            //                             j.PRODUCT_TYPE,
            //                             j.PRODUCT_ALIAS,
            //                             j.CONVERTER_OUTPUT,
            //                             j.CONVERTER_UOM_ID,
            //                             u.UOM_DESC,
            //                             Convertion = (decimal)(j.CONVERTER_UOM_ID.ToLower() == "kg" ? 1000 : 1)
            //                         }).Distinct().ToList();

            var joinedWithUomData = (from j in pbck1ProdConverter
                                     select new
                                     {
                                         j.PBCK1_PROD_COV_ID,
                                         j.PROD_CODE,
                                         j.PRODUCT_TYPE,
                                         j.PRODUCT_ALIAS,
                                         j.CONVERTER_OUTPUT
                                     }).Distinct().ToList();
            //2015-12-29
            var firstDataInventoryMovementTisToTis =
                rc.InventoryProductionTisToTis.InvetoryMovementData.InvMovementReceivingCk5List.FirstOrDefault(c => !string.IsNullOrEmpty(c.ConvertedUomId));
            var uomIdFirstDataInvMovementTisToTis = "";
            var uomDescFirstDataInvMovementTisToTis = "";

            if (firstDataInventoryMovementTisToTis != null)
            {
                uomIdFirstDataInvMovementTisToTis = firstDataInventoryMovementTisToTis.ConvertedUomId;
                uomDescFirstDataInvMovementTisToTis = firstDataInventoryMovementTisToTis.ConvertedUomDesc;
            }

            //old code, remove harcoded uom on tis to tis production list
            //var productionList = joinedWithUomData.Select(item => new Lack1GeneratedProductionDataDto()
            //{
            //    FaCode = null,
            //    Ordr = null,
            //    ProdCode = item.PROD_CODE,
            //    ProductType = item.PRODUCT_TYPE,
            //    ProductAlias = item.PRODUCT_ALIAS,
            //    Amount = item.CONVERTER_OUTPUT.HasValue ? ((rc.TotalUsageTisToTis.HasValue ? rc.TotalUsageTisToTis.Value : 0) * item.CONVERTER_OUTPUT.Value * item.Convertion) : 0,
            //    UomId = item.CONVERTER_UOM_ID.ToLower() == "kg" ? gramUomData.UOM_ID : item.CONVERTER_UOM_ID,
            //    UomDesc = item.CONVERTER_UOM_ID.ToLower() == "kg" ? gramUomData.UOM_DESC : item.UOM_DESC
            //}).ToList();

            var productionList = joinedWithUomData.Select(item => new Lack1GeneratedProductionDataDto()
            {
                FaCode = null,
                Ordr = null,
                ProdCode = item.PROD_CODE,
                ProductType = item.PRODUCT_TYPE,
                ProductAlias = item.PRODUCT_ALIAS,
                Amount = item.CONVERTER_OUTPUT.HasValue ? ((rc.TotalUsageTisToTis.HasValue ? rc.TotalUsageTisToTis.Value : 0) * item.CONVERTER_OUTPUT.Value) : 0,
                UomId = uomIdFirstDataInvMovementTisToTis,
                UomDesc = uomDescFirstDataInvMovementTisToTis
            }).ToList();

            rc.InventoryProductionTisToTis.ProductionData = new Lack1GeneratedProductionDto
            {
                ProductionList = productionList,
                ProductionSummaryByProdTypeList = GetProductionGroupedByProdTypeList(productionList),
                SummaryProductionList = GetSummaryGroupedProductionList(productionList)//calculate summary by UOM ID
            };

            return new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Data = rc
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Lack1GeneratedProductionSummaryByProdTypeDataDto> GetProductionGroupedByProdTypeList(
            List<Lack1GeneratedProductionDataDto> list)
        {
            if (list.Count <= 0) return new List<Lack1GeneratedProductionSummaryByProdTypeDataDto>();
            var groupedData = list.GroupBy(p => new
            {
                p.ProdCode,
                p.ProductAlias,
                p.ProductType,
                p.UomId,
                p.UomDesc
            }).Select(g => new Lack1GeneratedProductionSummaryByProdTypeDataDto()
            {
                ProdCode = g.Key.ProdCode,
                ProductAlias = g.Key.ProductAlias,
                ProductType = g.Key.ProductType,
                UomId = g.Key.UomId,
                UomDesc = g.Key.UomDesc,
                TotalAmount = g.Sum(p => p.Amount)
            });

            return groupedData.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Lack1GeneratedSummaryProductionDataDto> GetSummaryGroupedProductionList(List<Lack1GeneratedProductionDataDto> list)
        {
            if (list.Count <= 0) return new List<Lack1GeneratedSummaryProductionDataDto>();
            var groupedData = list.GroupBy(p => new
            {
                p.UomId,
                p.UomDesc
            }).Select(g => new Lack1GeneratedSummaryProductionDataDto()
            {
                UomId = g.Key.UomId,
                UomDesc = g.Key.UomDesc,
                Amount = g.Sum(p => p.Amount)
            });

            return groupedData.ToList();
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
            var nppbckData = _nppbkcService.GetById(input.NppbkcId);
            ck5Input.IsExcludeSameNppbkcId = !(nppbckData.FLAG_FOR_LACK1.HasValue && nppbckData.FLAG_FOR_LACK1.Value);

            //pbck-1 decree id list
            ck5Input.Pbck1DecreeIdList = rc.Pbck1List.Select(d => d.Pbck1Id).ToList();

            var ck5Data = _ck5Service.GetForLack1ByParam(ck5Input);
            rc.AllIncomeList = Mapper.Map<List<Lack1GeneratedIncomeDataDto>>(ck5Data);
            if (ck5Data.Count <= 0) return rc;

            rc.Ck5RemarkData = new Lack1GeneratedRemarkDto()
            {
                Ck5ReturnData = rc.AllIncomeList.Where(c => c.Ck5Type == Enums.CK5Type.Return && c.FlagForLack1).ToList(),
                /*story : http://192.168.62.216/TargetProcess/entity/1637 
                 * Ck5 Manual Trial don't include in remark column, 
                 * see previous function about getting data from ck5 that only include ck5 manual trial if REDUCE_TRIAL value is TRUE
                 */
                //Ck5TrialData = rc.AllIncomeList.Where(c => c.Ck5Type == Enums.CK5Type.Manual).ToList(),
                Ck5WasteData = rc.AllIncomeList.Where(c => c.Ck5Type == Enums.CK5Type.Waste).ToList()
            };

            rc.IncomeList = rc.AllIncomeList.Where(c =>
                !((c.Ck5Type == Enums.CK5Type.Return && c.FlagForLack1) || c.Ck5Type == Enums.CK5Type.Waste)).ToList();

            rc.TotalIncome = rc.IncomeList.Sum(d => d.Amount);

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
            //var dtTo = new DateTime(input.PeriodYear, input.PeriodMonth, 1);
            //var selected = _lack1Service.GetLatestLack1ByParam(new Lack1GetLatestLack1ByParamInput()
            //{
            //    CompanyCode = input.CompanyCode,
            //    Lack1Level = input.Lack1Level,
            //    NppbkcId = input.NppbkcId,
            //    ExcisableGoodsType = input.ExcisableGoodsType,
            //    SupplierPlantId = input.SupplierPlantId,
            //    ReceivedPlantId = input.ReceivedPlantId,
            //    PeriodTo = dtTo,
            //    ExcludeLack1Id = input.Lack1Id
            //});

            var listMaterial = _materialService.GetByPlantIdAndExGoodType(input.SupplierPlantId, input.ExcisableGoodsType);
            var listSticker = listMaterial.Select(x => x.STICKER_CODE).ToList();

            var listMaterialBalance = _materialBalanceService.GetByPlantAndMaterialList(input.SupplierPlantId, listSticker);

            rc.BeginingBalance = 0;
            if (listMaterialBalance.Count > 0)
            {
                //rc.BeginingBalance = selected.BEGINING_BALANCE + selected.TOTAL_INCOME - selected.USAGE;
                rc.BeginingBalance = listMaterialBalance.Sum(x => x.OPEN_BALANCE.Value);
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
                    rc.SupplierPlantName = latestDecreeDate.SUPPLIER_PLANT;
                    rc.Lack1UomId = latestDecreeDate.REQUEST_QTY_UOM;
                }
                rc.Pbck1List = Mapper.Map<List<Lack1GeneratedPbck1DataDto>>(pbck1Data);

            }
            else
            {
                rc.Pbck1List = new List<Lack1GeneratedPbck1DataDto>();
            }
            return rc;
        }

        private List<Lack1ProductionSummaryByProdTypeDto> GetProductionDetailSummaryByProdType(
            List<Lack1ProductionDetailDto> list)
        {
            if (list.Count == 0) return new List<Lack1ProductionSummaryByProdTypeDto>();
            var groupedData = list.GroupBy(p => new
            {
                p.PROD_CODE,
                p.PRODUCT_ALIAS,
                p.PRODUCT_TYPE,
                p.UOM_ID,
                p.UOM_DESC
            }).Select(g => new Lack1ProductionSummaryByProdTypeDto()
            {
                ProdCode = g.Key.PROD_CODE,
                ProductAlias = g.Key.PRODUCT_ALIAS,
                ProductType = g.Key.PRODUCT_TYPE,
                UomId = g.Key.UOM_ID,
                UomDesc = g.Key.UOM_DESC,
                TotalAmount = g.Sum(p => p.AMOUNT)
            });

            return groupedData.ToList();
        }

        private Lack1GeneratedOutput SetGenerateLack1InventoryMovementForEtilAlcohol(Lack1GeneratedDto rc,
            Lack1GenerateDataParamInput input, List<string> plantIdList, out InvMovementGetForLack1UsageMovementByParamOutput invMovementOutput, string bkcUomId)
        {
            //invMovementOutput = new InvMovementGetForLack1UsageMovementByParamOutput();
            var oRet = new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Data = rc
            };

            var stoReceiverNumberList = rc.IncomeList.Select(d => d.DnNumber).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

            var getInventoryMovementByParamOutput = GetInventoryMovementByParam(new InvMovementGetUsageByParamInput()
            {
                NppbkcId = input.NppbkcId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                PlantIdList = plantIdList,
                IsTisToTis = false,
                IsEtilAlcohol = true
            }, stoReceiverNumberList, bkcUomId);

            //bypass this error handling base on user story => http://192.168.62.216/TargetProcess/entity/1465
            //if (getInventoryMovementByParamOutput.AllUsageList.Count <= 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.TotalUsageLessThanEqualTpZero.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.TotalUsageLessThanEqualTpZero),
            //        Data = null
            //    };
            //}
            decimal totalUsage;
            if (getInventoryMovementByParamOutput.IncludeInCk5List.Count == 0)
            {
                totalUsage = 0;
            }
            else
            {
                totalUsage = (-1) * getInventoryMovementByParamOutput.IncludeInCk5List.Sum(d => d.ConvertedQty);
            }

            //nebeng in tis to fa field
            //set to tis to fa
            rc.InventoryProductionTisToFa.InvetoryMovementData = new Lack1GeneratedInventoryMovementDto
            {
                InvMovementReceivingCk5List =
                    Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.IncludeInCk5List),
                InvMovementReceivingList =
                    Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.ReceivingList),
                InvMovementAllList =
                    Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.AllUsageList)
            };
            rc.TotalUsage = totalUsage;

            invMovementOutput = getInventoryMovementByParamOutput;

            return oRet;
        }

        private List<InvMovementItemWithConvertion> InvMovementConvertionProcess(List<INVENTORY_MOVEMENT> invMovements, string bkcUomId)
        {
            var materialIdList = invMovements.Select(d => d.MATERIAL_ID).Distinct().ToList();
            var plantIdList = invMovements.Select(d => d.PLANT_ID).Distinct().ToList();
            var materialUomList = _materialUomService.GetByMaterialListAndPlantIdListSpecificBkcUom(materialIdList, plantIdList, bkcUomId);

            var uomData = _uomBll.GetAll().Distinct().ToList();

            //join material_uom and uom
            var joinedMaterialUomData = from x in materialUomList
                                        join y in uomData on x.MEINH equals y.UOM_ID into gj
                                        from subY in gj.DefaultIfEmpty()
                                        select new
                                        {
                                            x.STICKER_CODE,
                                            x.WERKS,
                                            x.ZAIDM_EX_MATERIAL.BASE_UOM_ID,
                                            x.MEINH,
                                            x.UMREN,
                                            ConvertedUomDesc = subY != null ? subY.UOM_DESC : string.Empty
                                        };

            //left join
            var dataToReturn = from x in invMovements
                               join m in joinedMaterialUomData on new { x.MATERIAL_ID, x.PLANT_ID, x.BUN }
                    equals new { MATERIAL_ID = m.STICKER_CODE, PLANT_ID = m.WERKS, BUN = m.BASE_UOM_ID } into gj
                               from subM in gj.DefaultIfEmpty()
                               select new InvMovementItemWithConvertion()
                               {
                                   INVENTORY_MOVEMENT_ID = x.INVENTORY_MOVEMENT_ID,
                                   MVT = x.MVT,
                                   MATERIAL_ID = x.MATERIAL_ID,
                                   PLANT_ID = x.PLANT_ID,
                                   QTY = x.QTY,
                                   SLOC = x.SLOC,
                                   VENDOR = x.VENDOR,
                                   BUN = x.BUN,
                                   PURCH_DOC = x.PURCH_DOC,
                                   POSTING_DATE = x.POSTING_DATE,
                                   ENTRY_DATE = x.ENTRY_DATE,
                                   TIME = x.TIME,
                                   CREATED_USER = x.CREATED_USER,
                                   MAT_DOC = x.MAT_DOC,
                                   BATCH = x.BATCH,
                                   ORDR = x.ORDR,
                                   ConvertedUomId = subM != null ? subM.MEINH : string.Empty,
                                   ConvertedUomDesc = subM != null ? subM.ConvertedUomDesc : string.Empty,
                                   ConvertedQty = subM != null ? (x.QTY.HasValue && subM.UMREN.HasValue ? (x.QTY.Value / subM.UMREN.Value) : 0) : 0
                               };

            return dataToReturn.ToList();

        }

        private Lack1GeneratedOutput SetGenerateLack1InventoryMovement(Lack1GeneratedDto rc,
            Lack1GenerateDataParamInput input, List<string> plantIdList, bool isForTisToTis, out InvMovementGetForLack1UsageMovementByParamOutput invMovementOutput,
            string bkcUomId)
        {
            //invMovementOutput = new InvMovementGetForLack1UsageMovementByParamOutput();
            var oRet = new Lack1GeneratedOutput()
            {
                Success = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Data = rc
            };

            var stoReceiverNumberList = rc.IncomeList.Where(c => c.Ck5Type != Enums.CK5Type.Manual).Select(d => d.Ck5Type == Enums.CK5Type.Intercompany ? d.StoReceiverNumber : d.StoSenderNumber).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

            var getInventoryMovementByParamOutput = GetInventoryMovementByParam(new InvMovementGetUsageByParamInput()
            {
                NppbkcId = input.NppbkcId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                PlantIdList = plantIdList,
                IsTisToTis = isForTisToTis,
                IsEtilAlcohol = false
            }, stoReceiverNumberList, bkcUomId);

            //bypass this handling base on user story => http://192.168.62.216/TargetProcess/entity/1465
            //if (getInventoryMovementByParamOutput.AllUsageList.Count <= 0)
            //{
            //    return new Lack1GeneratedOutput()
            //    {
            //        Success = false,
            //        ErrorCode = ExceptionCodes.BLLExceptions.TotalUsageLessThanEqualTpZero.ToString(),
            //        ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.TotalUsageLessThanEqualTpZero),
            //        Data = null
            //    };
            //}
            decimal totalUsage;
            if (getInventoryMovementByParamOutput.IncludeInCk5List.Count == 0)
            {
                totalUsage = 0;
            }
            else
            {
                totalUsage = (-1) * getInventoryMovementByParamOutput.IncludeInCk5List.Sum(d => d.ConvertedQty);

                /*Old Code, remove hardcoded convertion */
                //var totalUsageIncludeCk5 = (-1) * getInventoryMovementByParamOutput.IncludeInCk5List.Sum(d => d.QTY.HasValue ? (!string.IsNullOrEmpty(d.BUN) && d.BUN.ToLower() == "kg" ? d.QTY.Value * 1000 : d.QTY.Value) : 0);
                //totalUsage = totalUsageIncludeCk5;
            }
            if (isForTisToTis)
            {
                //set to tis to tis
                rc.InventoryProductionTisToTis.InvetoryMovementData = new Lack1GeneratedInventoryMovementDto
                {
                    InvMovementReceivingCk5List =
                        Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.IncludeInCk5List),
                    InvMovementReceivingList =
                        Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.ReceivingList),
                    InvMovementAllList =
                        Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.AllUsageList)
                };
                rc.TotalUsageTisToTis = totalUsage;
            }
            else
            {
                //set to tis to fa
                rc.InventoryProductionTisToFa.InvetoryMovementData = new Lack1GeneratedInventoryMovementDto
                {
                    InvMovementReceivingCk5List =
                        Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.IncludeInCk5List),
                    InvMovementReceivingList =
                        Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.ReceivingList),
                    InvMovementAllList =
                        Mapper.Map<List<Lack1GeneratedTrackingDto>>(getInventoryMovementByParamOutput.AllUsageList)
                };
                rc.TotalUsage = totalUsage;
            }

            invMovementOutput = getInventoryMovementByParamOutput;

            return oRet;
        }

        private InvMovementGetForLack1UsageMovementByParamOutput GetInventoryMovementByParam(
            InvMovementGetUsageByParamInput input, List<string> stoReceiverNumberList, string bkcUomId)
        {

            var usageParamInput = new InvMovementGetUsageByParamInput()
            {
                NppbkcId = input.NppbkcId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                PlantIdList = input.PlantIdList,
                IsTisToTis = input.IsTisToTis,
                IsEtilAlcohol = input.IsEtilAlcohol
            };

            var receivingParamInput = new InvMovementGetReceivingByParamInput()
            {
                NppbkcId = input.NppbkcId,
                PeriodMonth = input.PeriodMonth,
                PeriodYear = input.PeriodYear,
                PlantIdList = input.PlantIdList,
                IsTisToTis = input.IsTisToTis,
                IsEtilAlcohol = input.IsEtilAlcohol
            };

            var prevReceivingParamInput = new InvMovementGetReceivingByParamInput()
            {
                NppbkcId = input.NppbkcId,
                PlantIdList = input.PlantIdList,
                IsTisToTis = input.IsTisToTis,
                IsEtilAlcohol = input.IsEtilAlcohol
            };

            if (input.PeriodMonth == 1)
            {
                prevReceivingParamInput.PeriodMonth = 12;
                prevReceivingParamInput.PeriodYear = input.PeriodYear - 1;
            }
            else
            {
                prevReceivingParamInput.PeriodMonth = input.PeriodMonth - 1;
                prevReceivingParamInput.PeriodYear = input.PeriodYear;
            }

            var movementUsageAll = _inventoryMovementService.GetUsageByParam(usageParamInput);
            var movementUsaheAllWithConvertion = InvMovementConvertionProcess(movementUsageAll, bkcUomId);
            var receiving = _inventoryMovementService.GetReceivingByParam(receivingParamInput);
            //get prev receiving for CASE 2 : prev Receiving, Current Receiving, Current Usage
            var prevReceiving = _inventoryMovementService.GetReceivingByParam(prevReceivingParamInput);
            var receivingAll = receiving.Where(c => stoReceiverNumberList.Contains(c.PURCH_DOC)).ToList();
            receivingAll.AddRange(prevReceiving);

            var receivingAllWithConvertion = InvMovementConvertionProcess(receivingAll, bkcUomId);

            //there is records on receiving Data
            //normal case
            var receivingList = (from rec in receivingAllWithConvertion
                                 join a in movementUsaheAllWithConvertion.DistinctBy(d => new { d.MAT_DOC, d.MVT, d.MATERIAL_ID, d.PLANT_ID, d.BATCH, d.ORDR }) on new { rec.BATCH, rec.MATERIAL_ID } equals new { a.BATCH, a.MATERIAL_ID }
                                 select rec).DistinctBy(d => d.INVENTORY_MOVEMENT_ID).ToList();

            var usageReceivingList = (from rec in receivingAllWithConvertion.DistinctBy(d => new { d.MAT_DOC, d.MVT, d.MATERIAL_ID, d.PLANT_ID, d.BATCH, d.ORDR })
                                      join a in movementUsaheAllWithConvertion on new { rec.BATCH, rec.MATERIAL_ID } equals new { a.BATCH, a.MATERIAL_ID }
                                      select a).DistinctBy(d => d.INVENTORY_MOVEMENT_ID).ToList();

            //get exclude in receiving data
            var movementExclueInCk5List = (movementUsaheAllWithConvertion.Where(
                all => !usageReceivingList.Select(d => d.INVENTORY_MOVEMENT_ID)
                    .ToList()
                    .Contains(all.INVENTORY_MOVEMENT_ID))).DistinctBy(d => d.INVENTORY_MOVEMENT_ID).ToList();

            var usageProportionalList = CalculateInvMovementUsageProportional(usageReceivingList, movementUsageAll);

            var rc = new InvMovementGetForLack1UsageMovementByParamOutput
            {
                IncludeInCk5List = usageReceivingList,
                ReceivingList = receivingList,
                AllUsageList = movementUsaheAllWithConvertion,
                ExcludeFromCk5List = movementExclueInCk5List,
                UsageProportionalList = usageProportionalList
            };

            return rc;
        }

        private List<InvMovementUsageProportional> CalculateInvMovementUsageProportional(
            IEnumerable<INVENTORY_MOVEMENT> usageReceivingAll, IEnumerable<INVENTORY_MOVEMENT> usageAll)
        {
            var inventoryMovements = usageReceivingAll as INVENTORY_MOVEMENT[] ?? usageReceivingAll.ToArray();
            var inventoryMovementUsageAll = usageAll as INVENTORY_MOVEMENT[] ?? usageAll.ToArray();

            if (usageReceivingAll == null || inventoryMovements.Length == 0) return new List<InvMovementUsageProportional>();

            var listTotalPerMaterialId = inventoryMovementUsageAll.GroupBy(p => new
            {
                p.MATERIAL_ID
            }).Select(g => new
            {
                MaterialId = g.Key.MATERIAL_ID,
                TotalQty = g.Sum(p => p.QTY.HasValue ? p.QTY.Value : 0)
            }).ToList();

            //grouped by MAT_DOC, MVT, MATERIAL_ID, PLANT_ID, BATCH and ORDR
            var groupedInventoryMovements = inventoryMovements.GroupBy(p => new
            {
                p.MAT_DOC,
                p.MVT,
                p.MATERIAL_ID,
                p.PLANT_ID,
                p.BATCH,
                p.ORDR
            }).Select(g => new
            {
                MatDoc = g.Key.MAT_DOC,
                Mvt = g.Key.MVT,
                MaterialId = g.Key.MATERIAL_ID,
                PlantId = g.Key.PLANT_ID,
                Batch = g.Key.BATCH,
                Ordr = g.Key.ORDR,
                TotalQty = g.Sum(p => p.QTY.HasValue ? p.QTY.Value : 0)
            }).ToList();

            var rc = (from x in groupedInventoryMovements
                      join y in listTotalPerMaterialId on x.MaterialId equals y.MaterialId
                      select new InvMovementUsageProportional()
                      {
                          MaterialId = x.MaterialId,
                          Qty = x.TotalQty,
                          TotalQtyPerMaterialId = y.TotalQty,
                          Order = x.Ordr
                      }).ToList();

            return rc;
        }

        #endregion

        #region ------------------ Summary Report ----------------

        public List<Lack1SummaryReportDto> GetSummaryReportByParam(Lack1GetSummaryReportByParamInput input)
        {
            var lack1Data = _lack1Service.GetSummaryReportByParam(input);
            return Mapper.Map<List<Lack1SummaryReportDto>>(lack1Data);
        }

        public List<int> GetYearList()
        {
            return _lack1Service.GetYearList();
        }

        public List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbckListByCompanyCode(string companyCode)
        {
            var data = _lack1Service.GetByCompanyCode(companyCode);
            if (data.Count > 0)
            {
                var nppbkcList = Mapper.Map<List<ZAIDM_EX_NPPBKCCompositeDto>>(data);
                return nppbkcList.DistinctBy(c => c.NPPBKC_ID).ToList();
            }
            return new List<ZAIDM_EX_NPPBKCCompositeDto>();
        }

        #endregion

        #region --------------------- Detail Report ----------------

        public List<Lack1DetailReportDto> GetDetailReportByParam(Lack1GetDetailReportByParamInput input)
        {
            var dbData = _lack1Service.GetDetailReportByParamInput(input);

            var tempData = Mapper.Map<List<Lack1DetailReportTempDto>>(dbData.ToList());

            DateTime? dtFrom = null;
            DateTime? dtTo = null;

            if (input.PeriodMonthFrom.HasValue && input.PeriodYearFrom.HasValue)
            {
                dtFrom = new DateTime(input.PeriodYearFrom.Value, input.PeriodMonthFrom.Value, 1);
            }
            if (input.PeriodMonthTo.HasValue && input.PeriodYearTo.HasValue)
            {
                dtTo = new DateTime(input.PeriodYearTo.Value, input.PeriodMonthTo.Value, 1);
            }

            if (dtFrom.HasValue && dtTo.HasValue)
            {
                tempData = tempData.Where(c => c.PeriodDate >= dtFrom.Value && c.PeriodDate <= dtTo.Value).ToList();
            }
            else
            {
                if (dtFrom.HasValue)
                {
                    tempData = tempData.Where(c => c.PeriodDate >= dtFrom.Value).ToList();
                }
                if (dtTo.HasValue)
                {
                    tempData = tempData.Where(c => c.PeriodDate <= dtTo.Value).ToList();
                }
            }

            var rc = new List<Lack1DetailReportDto>();
            var uomData = _uomBll.GetAll();

            var uomGram = uomData.FirstOrDefault(c => c.UOM_ID.ToLower() == "g");
            var uomGramDesc = "";
            var uomGramId = "";
            if (uomGram != null)
            {
                uomGramDesc = uomGram.UOM_DESC;
                uomGramId = uomGram.UOM_ID;
            }

            foreach (var data in tempData)
            {
                var item = new Lack1DetailReportDto()
                {
                    Lack1Id = data.LACK1_ID,
                    Lack1Number = data.LACK1_NUMBER,
                    Lack1Level = data.LACK1_LEVEL,
                    BeginingBalance = data.BEGINING_BALANCE,
                    EndingBalance = data.BEGINING_BALANCE + data.TOTAL_INCOME - (data.USAGE + (data.USAGE_TISTOTIS.HasValue ? data.USAGE_TISTOTIS.Value : 0)) - (data.RETURN_QTY.HasValue ? data.RETURN_QTY.Value : 0),
                    TrackingConsolidations = new List<Lack1TrackingConsolidationDetailReportDto>()
                };

                var incomeListExcludeManual =
                    data.LACK1_INCOME_DETAIL.Where(c => c.CK5.CK5_TYPE != Enums.CK5Type.Manual).ToList();

                var incomeListCk5Manual =
                    data.LACK1_INCOME_DETAIL.Where(c => c.CK5.CK5_TYPE == Enums.CK5Type.Manual).ToList();

                var incomeList = (from inc in incomeListExcludeManual
                                  join uom in uomData on inc.CK5.PACKAGE_UOM_ID.ToLower() equals uom.UOM_ID.ToLower() into gj
                                  from subUom in gj.DefaultIfEmpty()
                                  select new Lack1ReceivingDetailReportDto()
                                  {
                                      Ck5Id = inc.CK5.CK5_ID,
                                      Ck5Number = inc.CK5.SUBMISSION_NUMBER,
                                      Ck5RegistrationNumber = inc.CK5.REGISTRATION_NUMBER,
                                      Ck5RegistrationDate = inc.CK5.REGISTRATION_DATE,
                                      Ck5GrDate = inc.CK5.GR_DATE,
                                      StoNumber =
                                          inc.CK5.CK5_TYPE == Enums.CK5Type.Intercompany
                                              ? inc.CK5.STO_RECEIVER_NUMBER
                                              : inc.CK5.CK5_TYPE == Enums.CK5Type.DomesticAlcohol
                                              ? inc.CK5.DN_NUMBER : inc.CK5.STO_SENDER_NUMBER,
                                      Qty = inc.AMOUNT,
                                      UomId = inc.CK5.PACKAGE_UOM_ID,
                                      UomDesc = subUom != null ? subUom.UOM_DESC : string.Empty,
                                      Ck5Type = inc.CK5.CK5_TYPE,
                                      Ck5TypeText = EnumHelper.GetDescription(inc.CK5.CK5_TYPE)
                                  }).ToList();

                var usageConsolidationData = ProcessUsageConsolidationDetailReport(data, incomeList, uomData);

                //add record for CK5 manual
                foreach (var ck5Item in incomeListCk5Manual)
                {
                    var ck5Material = ck5Item.CK5.CK5_MATERIAL.ToList();
                    if (ck5Material.Count <= 0) continue;
                    var uomData1 = uomData.Select(d => new
                    {
                        d.UOM_ID,
                        d.UOM_DESC
                    });
                    var groupedCk5Material = ck5Material.GroupBy(p => new
                    {
                        p.BRAND
                    }).Select(g => new Lack1TrackingConsolidationDetailReportDto()
                    {
                        Ck5Id = g.First().CK5.CK5_ID,
                        Ck5Number = g.First().CK5.SUBMISSION_NUMBER,
                        Ck5RegistrationNumber = g.First().CK5.REGISTRATION_NUMBER,
                        Ck5RegistrationDate = g.First().CK5.REGISTRATION_DATE,
                        Ck5GrDate = g.First().CK5.GR_DATE,
                        Qty = g.Sum(p => p.CONVERTED_QTY.HasValue ? p.CONVERTED_QTY.Value : 0),
                        GiDate = g.First().CK5.GR_DATE,
                        PurchaseDoc = string.Empty,
                        MaterialCode = g.First().BRAND,
                        UsageQty = g.Sum(p => p.CONVERTED_QTY.HasValue ? p.CONVERTED_QTY.Value : 0),
                        OriginalUomId = g.First().UOM,
                        ConvertedUomId = g.First().CONVERTED_UOM,
                        Batch = string.Empty,
                        MaterialCodeUsageRecCount = 1,
                        Ck5TypeText = EnumHelper.GetDescription(g.First().CK5.CK5_TYPE)
                    }).ToList();

                    groupedCk5Material = (from x in groupedCk5Material
                                          join u in uomData on x.OriginalUomId.ToLower() equals u.UOM_ID.ToLower() into gj1
                                          from subU in gj1.DefaultIfEmpty()
                                          join u1 in uomData1 on x.ConvertedUomId.ToLower() equals u1.UOM_ID.ToLower() into gj2
                                          from subU2 in gj2.DefaultIfEmpty()
                                          select new Lack1TrackingConsolidationDetailReportDto()
                                          {
                                              Ck5Id = x.Ck5Id,
                                              Ck5Number = x.Ck5Number,
                                              Ck5RegistrationNumber = x.Ck5RegistrationNumber,
                                              Ck5RegistrationDate = x.Ck5RegistrationDate,
                                              Ck5GrDate = x.Ck5GrDate,
                                              Qty = x.ConvertedUomId.ToLower() == "kg" ? 1000 * x.Qty : x.Qty,
                                              GiDate = x.GiDate,
                                              PurchaseDoc = string.Empty,
                                              MaterialCode = x.MaterialCode,
                                              UsageQty = x.ConvertedUomId.ToLower() == "kg" ? 1000 * x.UsageQty : x.UsageQty,
                                              OriginalUomId = x.OriginalUomId,
                                              OriginalUomDesc = subU != null ? subU.UOM_DESC : string.Empty,
                                              ConvertedUomId = x.ConvertedUomId.ToLower() == "kg" ? uomGramId : x.ConvertedUomId,
                                              ConvertedUomDesc = x.ConvertedUomId.ToLower() == "kg" ? uomGramDesc : (subU2 != null ? subU2.UOM_DESC : string.Empty),
                                              Batch = string.Empty,
                                              MaterialCodeUsageRecCount = x.MaterialCodeUsageRecCount,
                                              Ck5TypeText = x.Ck5TypeText
                                          }).ToList();
                    usageConsolidationData.AddRange(groupedCk5Material);
                }

                item.TrackingConsolidations.AddRange(usageConsolidationData.Distinct().ToList());

                item.TrackingConsolidations = item.TrackingConsolidations.OrderBy(o => o.MaterialCode).ThenBy(o => o.Batch).ToList();
                rc.Add(item);

            }
            return rc.OrderBy(o => o.Lack1Id).ToList();
        }

        //private string GenerateRemarkContent(List<LACK1_INCOME_DETAIL> data, string title)
        //{
        //    var rc = string.Empty;
        //    if (data.Count <= 0) return rc;
        //    rc = title + Environment.NewLine;
        //    //rc += string.Join(Environment.NewLine, data.Select(
        //    //    d =>
        //    //        "CK-5 " + d.REGISTRATION_NUMBER + " - " +
        //    //        (d.REGISTRATION_DATE.HasValue
        //    //            ? d.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy")
        //    //            : string.Empty) + " : " + d.AMOUNT.ToString("N2") + " " + d.PACKAGE_UOM_DESC).ToList());

        //    rc += string.Join(Environment.NewLine, data.Select(
        //       d =>
        //           "CK-5 " + d.REGISTRATION_NUMBER + " - " + (d.REGISTRATION_DATE.HasValue
        //                ? d.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy")
        //                : string.Empty) + " : " + d.AMOUNT.ToString("N2") + " " + (d.CK5.UOM != null ? d.CK5.UOM.UOM_DESC : string.Empty)).ToList());
        //    return rc;
        //}

        private List<Lack1TrackingConsolidationDetailReportDto> ProcessUsageConsolidationDetailReport(Lack1DetailReportTempDto data,
            List<Lack1ReceivingDetailReportDto> incomeList, List<UOM> uomData)
        {
            
            var usageReceiving = new List<Lack1UsageReceivingTrackingDetailDto>();

            //old code, remove hardcoded uom
            //var uomGram = uomData.FirstOrDefault(c => c.UOM_ID.ToLower() == "g");
            //var uomGramDesc = "";
            //var uomGramId = "";
            //if (uomGram != null)
            //{
            //    uomGramDesc = uomGram.UOM_DESC;
            //    uomGramId = uomGram.UOM_ID;
            //}

            if (data.LACK1_TRACKING != null && data.LACK1_TRACKING.Count > 0)
            {
                var receivingMvtType = new List<string>()
                    {
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Receiving101),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Receiving102)
                    };

                var receiving =
                    data.LACK1_TRACKING.Where(
                        c => receivingMvtType.Contains(c.INVENTORY_MOVEMENT.MVT)).Select(d => d.INVENTORY_MOVEMENT).DistinctBy(ds => ds.INVENTORY_MOVEMENT_ID)
                        .ToList();

                var mvtTypeForUsage = new List<string>
                    {
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage261),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage261),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage201),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage202),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage901),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.Usage902),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.UsageZ01),
                        EnumHelper.GetDescription(Enums.MovementTypeCode.UsageZ02)
                    };

                var usage =
                    data.LACK1_TRACKING.Where(c => mvtTypeForUsage.Contains(c.INVENTORY_MOVEMENT.MVT))
                        .Select(d => new
                        {
                            d.INVENTORY_MOVEMENT_ID,
                            d.INVENTORY_MOVEMENT.MATERIAL_ID,
                            d.INVENTORY_MOVEMENT.BATCH,
                            d.INVENTORY_MOVEMENT.POSTING_DATE,
                            d.INVENTORY_MOVEMENT.QTY,
                            d.INVENTORY_MOVEMENT.BUN,
                            d.CONVERTED_UOM_ID,
                            d.CONVERTED_QTY,
                            d.CONVERTED_UOM_DESC
                        }).DistinctBy(ds => ds.INVENTORY_MOVEMENT_ID)
                        .ToList();

                //need to grouping before process
                //usage group by material_id and batch
                var groupedUsage = usage.GroupBy(p => new
                {
                    p.MATERIAL_ID,
                    p.BATCH,
                    p.POSTING_DATE
                }).Select(g => new
                {
                    MaterialId = g.Key.MATERIAL_ID,
                    Qty = g.Sum(p => p.QTY.HasValue ? (-1) * p.QTY.Value : 0),
                    Batch = g.Key.BATCH,
                    PostingDate = g.Key.POSTING_DATE,
                    Bun = g.First().BUN,
                    ConvertedUomId = g.First().CONVERTED_UOM_ID,
                    ConvertedUomDesc = g.First().CONVERTED_UOM_DESC,
                    ConvertedQty = g.Sum(p => p.CONVERTED_QTY.HasValue ? p.CONVERTED_QTY.Value : 0)
                });

                var groupedReceiving = DetailReportTrackingGroupedBy(receiving);

                usageReceiving = (from u in groupedUsage
                                  join rec in groupedReceiving on new { u.Batch, u.MaterialId } equals
                                  new { rec.Batch, rec.MaterialId }
                                  join uom in uomData on u.Bun equals uom.UOM_ID into gj
                                  from subUom in gj.DefaultIfEmpty()
                                  select new Lack1UsageReceivingTrackingDetailDto()
                                  {
                                      PurchaseDoc = rec.PurchDoc,
                                      MaterialCode = u.MaterialId,
                                      UsageQty = u.ConvertedQty,
                                      Batch = rec.Batch,
                                      PostingDate = u.PostingDate,
                                      OriginalUom = u.Bun,
                                      OriginalUomDesc = subUom != null ? subUom.UOM_DESC : string.Empty,
                                      ConvertedUomId = u.ConvertedUomId,
                                      ConvertedUomDesc = u.ConvertedUomDesc
                                  }).ToList();

            }

            //null sto number
            var incNullStoNumber = incomeList.Where(c => string.IsNullOrEmpty(c.StoNumber)).ToList();
            var incNotNullStoNumber = incomeList.Where(c => !string.IsNullOrEmpty(c.StoNumber)).ToList();

            //left join income list and usageReceiving
            var leftJoined = (from inc in incNotNullStoNumber
                              join rec in usageReceiving on inc.StoNumber equals rec.PurchaseDoc into gj
                              from subRec in gj.DefaultIfEmpty()
                              select new Lack1TrackingConsolidationDetailReportDto()
                                                   {
                                                       Ck5Id = inc.Ck5Id,
                                                       Ck5Number = inc.Ck5Number,
                                                       Ck5RegistrationNumber = inc.Ck5RegistrationNumber,
                                                       Ck5RegistrationDate = inc.Ck5RegistrationDate,
                                                       Ck5GrDate = inc.Ck5GrDate,
                                                       Qty = inc.Qty,
                                                       GiDate = subRec != null ? subRec.PostingDate : null,
                                                       PurchaseDoc = subRec != null ? subRec.PurchaseDoc : string.Empty,
                                                       MaterialCode = subRec != null ? subRec.MaterialCode : string.Empty,
                                                       UsageQty = subRec != null ? subRec.UsageQty : null,
                                                       OriginalUomId = subRec != null ? subRec.OriginalUom : string.Empty,
                                                       OriginalUomDesc = subRec != null ? subRec.OriginalUomDesc : string.Empty,
                                                       ConvertedUomId = subRec != null ? subRec.ConvertedUomId : string.Empty,
                                                       ConvertedUomDesc = subRec != null ? subRec.ConvertedUomDesc : string.Empty,
                                                       Batch = subRec != null ? subRec.Batch : string.Empty,
                                                       MaterialCodeUsageRecCount = subRec != null ? subRec.RecordCountForMerge : 1,
                                                       Ck5TypeText = inc.Ck5TypeText
                                                   }).DistinctBy(d => d.Ck5Number).ToList();

            //right join income list and usageReceiving
            var rightJoined = (from rec in usageReceiving
                               join inc in incNotNullStoNumber on rec.PurchaseDoc equals inc.StoNumber into gj
                               from subInc in gj.DefaultIfEmpty()
                               where subInc == null
                               select rec).Distinct().ToList();

            //let's find ck5 data from rightJoined that haven't ck5 in LACK1_INCOME_DETAIL for current LACK1
            var purchDocList = rightJoined.Select(d => d.PurchaseDoc).Where(c => !string.IsNullOrEmpty(c)).ToList();

            var ck5DataByStoNumberList = _ck5Service.GetByStoNumberList(purchDocList).Select(m => new
            {
                m.CK5_ID,
                m.SUBMISSION_NUMBER,
                m.REGISTRATION_DATE,
                m.REGISTRATION_NUMBER,
                m.GR_DATE,
                m.GRAND_TOTAL_EX,
                m.PACKAGE_UOM_ID,
                PACKAGE_UOM_DESC = m.UOM != null ? m.UOM.UOM_DESC : string.Empty,
                Ck5TypeText = EnumHelper.GetDescription(m.CK5_TYPE),
                STO_NUMBER = m.CK5_TYPE == Enums.CK5Type.Intercompany
                                              ? m.STO_RECEIVER_NUMBER
                                              : m.CK5_TYPE == Enums.CK5Type.DomesticAlcohol
                                              ? m.DN_NUMBER : m.STO_SENDER_NUMBER
            }).ToList();

            var newRightJoined = (from x in rightJoined
                                  join y in ck5DataByStoNumberList on x.PurchaseDoc equals y.STO_NUMBER
                                  select new Lack1TrackingConsolidationDetailReportDto()
                                  {
                                      Ck5Id = y.CK5_ID,
                                      Ck5Number = y.SUBMISSION_NUMBER,
                                      Ck5RegistrationNumber = y.REGISTRATION_NUMBER,
                                      Ck5RegistrationDate = y.REGISTRATION_DATE,
                                      Ck5GrDate = y.GR_DATE,
                                      Qty = y.GRAND_TOTAL_EX.HasValue ? y.GRAND_TOTAL_EX.Value : 0,
                                      GiDate = x.PostingDate,
                                      PurchaseDoc = x.PurchaseDoc,
                                      MaterialCode = x.MaterialCode,
                                      UsageQty = x.UsageQty,
                                      OriginalUomId = x.OriginalUom,
                                      OriginalUomDesc = x.OriginalUomDesc,
                                      ConvertedUomId = x.ConvertedUomId,
                                      ConvertedUomDesc = x.ConvertedUomDesc,
                                      Batch = x.Batch,
                                      MaterialCodeUsageRecCount = 1,
                                      Ck5TypeText = y.Ck5TypeText
                                  }).ToList();

            //join leftJoined and rightJoined and distinct as result
            var joinedData = leftJoined;
            joinedData.AddRange(newRightJoined);

            var joinedDataRecordCount = joinedData.GroupBy(p => new
            {
                p.MaterialCode,
                p.Batch,
                p.GiDate
            }).Select(g => new
            {
                g.Key.MaterialCode,
                g.Key.Batch,
                g.Key.GiDate,
                RecordCount = g.Count()
            }).ToList();

            for (var i = 0; i < joinedData.Count; i++)
            {
                var chk =
                    joinedDataRecordCount.FirstOrDefault(c => c.MaterialCode == joinedData[i].MaterialCode
                        && c.Batch == joinedData[i].Batch && c.GiDate == joinedData[i].GiDate);
                if (chk != null)
                {
                    joinedData[i].MaterialCodeUsageRecCount = chk.RecordCount;
                }
            }

            //join joinedData with incomeList that sto number is null or empty
            joinedData.AddRange(incNullStoNumber.Select(inc => new Lack1TrackingConsolidationDetailReportDto()
            {
                Ck5Id = inc.Ck5Id,
                Ck5Number = inc.Ck5Number,
                Ck5RegistrationNumber = inc.Ck5RegistrationNumber,
                Ck5RegistrationDate = inc.Ck5RegistrationDate,
                Ck5GrDate = inc.Ck5GrDate,
                Qty = inc.Qty,
                GiDate = null,
                PurchaseDoc = string.Empty,
                MaterialCode = string.Empty,
                UsageQty = null,
                OriginalUomId = string.Empty,
                OriginalUomDesc = string.Empty,
                ConvertedUomId = string.Empty,
                ConvertedUomDesc = string.Empty,
                Batch = string.Empty,
                MaterialCodeUsageRecCount = 1,
                Ck5TypeText = inc.Ck5TypeText
            }));

            var usageConsolidationData = joinedData.DistinctBy(m => new
            {
                m.Ck5Number,
                m.Ck5TypeText,
                m.Ck5RegistrationNumber,
                m.Ck5RegistrationDate,
                m.Ck5GrDate,
                m.GiDate,
                m.MaterialCode,
                m.Batch
            }).ToList();

            return usageConsolidationData;
        }

        private List<Lack1DetailReportTrackingDetailDto> DetailReportTrackingGroupedBy(IEnumerable<INVENTORY_MOVEMENT> invMovements)
        {
            return invMovements.GroupBy(p => new
            {
                p.MAT_DOC,
                p.MVT,
                p.MATERIAL_ID,
                p.PLANT_ID,
                p.BATCH,
                p.ORDR
            }).Select(g => new Lack1DetailReportTrackingDetailDto()
            {
                Mvt = g.Key.MVT,
                MaterialId = g.Key.MATERIAL_ID,
                MatDoc = g.Key.MAT_DOC,
                PlantId = g.Key.PLANT_ID,
                Batch = g.Key.BATCH,
                Ordr = g.Key.ORDR,
                Bun = g.First().BUN,
                PurchDoc = g.First().PURCH_DOC,
                PostingDate = g.First().POSTING_DATE,
                Qty = g.Sum(p => p.QTY.HasValue ? p.QTY.Value : 0)
            }).ToList();
        }

        #endregion

        #region ------------- Dashboard -----------

        public List<Lack1Dto> GetDashboardDataByParam(Lack1GetDashboardDataByParamInput input)
        {
            var data = _lack1Service.GetDashboardDataByParam(input);
            return Mapper.Map<List<Lack1Dto>>(data);
        }

        #endregion

    }
}
