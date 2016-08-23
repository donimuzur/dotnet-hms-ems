using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class InventoryMovementService : IInventoryMovementService
    {

        private IGenericRepository<INVENTORY_MOVEMENT> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAAP_SHIFT_RPT> _zaapShiftRptRepository;
        private ZaidmExMaterialService _materialService;
        private IMaterialUomService _materialUomService;

        public InventoryMovementService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<INVENTORY_MOVEMENT>();
            _zaapShiftRptRepository = _uow.GetGenericRepository<ZAAP_SHIFT_RPT>();
            _materialService = new ZaidmExMaterialService(_uow, _logger);
            _materialUomService = new MaterialUomService(_uow, _logger);
        }

        public List<INVENTORY_MOVEMENT> GetUsageByParam(InvMovementGetUsageByParamInput input)
        {
            var usageMvtType = new List<string>()
            {
                //EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201),
                //EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage202),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage262),
                //EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage901),
                //EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage902),
                //EnumHelper.GetDescription(Core.Enums.MovementTypeCode.UsageZ01),
                //EnumHelper.GetDescription(Core.Enums.MovementTypeCode.UsageZ02)
            };

            //original irman
            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue
                && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;

            //var tempyear = input.PeriodMonth == 12 ? input.PeriodYear + 1 : input.PeriodYear;
            //var tempmonth = input.PeriodMonth == 12 ? 1 : input.PeriodMonth + 1;


            //Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue && c.POSTING_DATE.Value < new DateTime(tempyear, tempmonth, 1);


            if (input.PlantIdList.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.PlantIdList.Contains(c.PLANT_ID));
            }
            
            queryFilter = queryFilter.And(c => usageMvtType.Contains(c.MVT));

            

            if (input.IsEtilAlcohol) return _repository.Get(queryFilter).ToList();

            var allOrderInZaapShiftRpt = _zaapShiftRptRepository.Get().Select(d => d.ORDR).Distinct().ToList();

            //Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter2 = queryFilter;
 
            var result = _repository.Get(queryFilter).ToList();

            //queryFilter = input.IsTisToTis
            //    ? queryFilter.And(c => !allOrderInZaapShiftRpt.Contains(c.ORDR))
            //    : //queryFilter;
            //    queryFilter.And(c => allOrderInZaapShiftRpt.Contains(c.ORDR));

            if(input.IsTisToTis)
            {
                result = result.Where(c => !allOrderInZaapShiftRpt.Contains(c.ORDR)).ToList();
            }
            else
            {
                result = result.Where(c => allOrderInZaapShiftRpt.Contains(c.ORDR)).ToList();
            }

            return result;

        }

        public List<INVENTORY_MOVEMENT> GetReceivingByParam(InvMovementGetReceivingByParamInput input)
        {
            var receivingMvtType = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving102)
            };

            //original by irman
            //Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue
            //    && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;

            var tempyear = input.PeriodMonth == 12 ? input.PeriodYear + 1 : input.PeriodYear;
            var tempmonth = input.PeriodMonth == 12 ? 1 : input.PeriodMonth + 1;


            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue && c.POSTING_DATE.Value < new DateTime(tempyear,tempmonth, 1);

            //if (input.IsEtilAlcohol)
            //{
            //    queryFilter = c => c.POSTING_DATE.HasValue
            //    && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;
            //}

            if (input.PlantIdList.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.PlantIdList.Contains(c.PLANT_ID));
            }

            queryFilter = queryFilter.And(c => receivingMvtType.Contains(c.MVT));

            return _repository.Get(queryFilter).ToList();
        }

        public List<INVENTORY_MOVEMENT> GetReceivingByBatch201(ZaapShiftRptGetForLack1ReportByParamInput input)
        {
            var receivingMvtType = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage202),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage901),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage902),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.UsageZ01),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.UsageZ02)
            };

            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue && c.POSTING_DATE.Value <= input.EndDate;

            queryFilter = queryFilter.And(c => c.POSTING_DATE.HasValue && c.POSTING_DATE >= input.BeginingDate);



            queryFilter = queryFilter.And(c => input.Werks.Contains(c.PLANT_ID));




            


            queryFilter = queryFilter.And(c => receivingMvtType.Contains(c.MVT));

            



            return _repository.Get(queryFilter).ToList();
        }

    

        public List<INVENTORY_MOVEMENT> GetReceivingByParamZaapShiftRpt(InvGetReceivingByParamZaapShiftRptInput input)
        {
            var receivingMvtType = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage262),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage202)
            };

            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue && c.POSTING_DATE.Value <= input.EndDate;

            queryFilter = queryFilter.And(c => c.POSTING_DATE.HasValue && c.POSTING_DATE >= input.StartDate);
            

            
            queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            
            
            
            
            queryFilter = queryFilter.And(c => c.ORDR == input.Ordr);
            

            queryFilter = queryFilter.And(c => receivingMvtType.Contains(c.MVT));

            

            

            return _repository.Get(queryFilter).ToList();
        }


        public INVENTORY_MOVEMENT GetReceivingByProcessOrderAndPlantId(string processOrder, string plantId)
        {
            var mvtReceiving = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101);
            return _repository.Get(c => c.ORDR == processOrder && c.PLANT_ID == plantId && c.MVT == mvtReceiving).FirstOrDefault();
        }

        public INVENTORY_MOVEMENT GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public INVENTORY_MOVEMENT GetUsageByBatchAndPlantId(string batch, string plantId)
        {
            var mvtUsage = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261);
            var data =
                _repository.Get(
                    c =>
                        c.BATCH == batch && c.PLANT_ID == plantId &&
                        c.MVT == mvtUsage)
                    .OrderByDescending(o => o.POSTING_DATE);
            return data.FirstOrDefault();
        }

        public List<INVENTORY_MOVEMENT> GetUsageByBatchAndPlantIdInPeriod(GetUsageByBatchAndPlantIdInPeriodParamInput input, List<BOM> bomMapList)
        {

            var mvtUsage = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage262)
            };
            BOM bom = new BOM();
            string bomMaterial;
            if (input.TrackLevel == 1)
            {
                bom = bomMapList.Where(x => x.LEVEL1 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL1 : null;
            }
            else if (input.TrackLevel == 2)
            {
                bom = bomMapList.Where(x => x.LEVEL2 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL2 : null;
            }
            else if (input.TrackLevel == 3)
            {
                bom = bomMapList.Where(x => x.LEVEL3 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL3 : null;
            }
            else if (input.TrackLevel == 4)
            {
                bom = bomMapList.Where(x => x.LEVEL4 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL4 : null;
            }
            else if (input.TrackLevel == 5)
            {
                bom = bomMapList.Where(x => x.LEVEL5 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL5 : null;
            }
            else if (input.TrackLevel == 6)
            {
                bom = bomMapList.Where(x => x.LEVEL6 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL6 : null;
            }
            else if (input.TrackLevel == 7)
            {
                bom = bomMapList.Where(x => x.LEVEL7 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL7 : null;
            }
            else if (input.TrackLevel == 8)
            {
                bom = bomMapList.Where(x => x.LEVEL8 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL8 : null;
            }
            else if (input.TrackLevel == 9)
            {
                bom = bomMapList.Where(x => x.LEVEL9 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL9 : null;
            }
            else if (input.TrackLevel == 10)
            {
                bom = bomMapList.Where(x => x.LEVEL10 == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.LEVEL10 : null;
            }
            else
            {
                bom = bomMapList.Where(x => x.MATERIAL_ID == input.LastMaterialId).FirstOrDefault();
                bomMaterial = bom != null ? bom.MATERIAL_ID : null;
            }

            //if (bom != null)
            //{
                var data =
                    _repository.Get(
                        c =>
                            c.BATCH == input.Batch && c.PLANT_ID == input.PlantId &&
                            mvtUsage.Contains(c.MVT)
                            //&& c.MATERIAL_ID == bomMaterial
                            //&& c.POSTING_DATE.HasValue 
                            //&& c.POSTING_DATE.Value.Year <= input.PeriodYear && c.POSTING_DATE.Value.Month <= input.PeriodMonth
                            );
                

                return data.ToList();
            //}
            //else
            //{
            //    return new List<INVENTORY_MOVEMENT>();
            //}


        }

        public List<INVENTORY_MOVEMENT> GetReceivingByOrderAndPlantIdInPeriod(GetReceivingByOrderAndPlantIdInPeriodParamInput input, List<BOM> bomMapList)
        {

            var mvtReceiving = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving102)
            };

            //BOM bom = new BOM();
            List<string> bomMaterial = new List<string>();
            int prevLevel = input.TrackLevel - 1;

            if (prevLevel <= 0)
            {
                bomMaterial = bomMapList.Where(x => x.MATERIAL_ID == input.LastMaterialId).Select(x => x.LEVEL1).Distinct().ToList();
            }
            else if (prevLevel == 1)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL1 == input.LastMaterialId).Select(x => x.LEVEL2).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL1 : null;
            }
            else if (prevLevel == 2)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL2 == input.LastMaterialId).Select(x => x.LEVEL3).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL2 : null;
            }
            else if (prevLevel == 3)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL3 == input.LastMaterialId).Select(x => x.LEVEL4).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL3 : null;
            }
            else if (prevLevel == 4)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL4 == input.LastMaterialId).Select(x => x.LEVEL5).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL4 : null;
            }
            else if (prevLevel == 5)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL5 == input.LastMaterialId).Select(x => x.LEVEL6).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL5 : null;
            }
            else if (prevLevel == 6)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL6 == input.LastMaterialId).Select(x => x.LEVEL7).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL6 : null;
            }
            else if (prevLevel == 7)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL7 == input.LastMaterialId).Select(x => x.LEVEL8).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL7 : null;
            }
            else if (prevLevel == 8)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL8 == input.LastMaterialId).Select(x => x.LEVEL9).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL8 : null;
            }
            else if (prevLevel == 9)
            {
                bomMaterial = bomMapList.Where(x => x.LEVEL9 == input.LastMaterialId).Select(x => x.LEVEL10).Distinct().ToList();
                //bomMaterial = bom != null ? bom.LEVEL9 : null;
            }
            else
            {
                bomMaterial = new List<string>();
                //bomMaterial = bom != null ? bom.MATERIAL_ID : null;
            }

            if (bomMaterial.Count > 0 )
            {

                var data =
                    _repository.Get(
                        c =>
                            c.ORDR == input.Ordr && c.PLANT_ID == input.PlantId &&
                            mvtReceiving.Contains(c.MVT) 
                            && bomMaterial.Contains(c.MATERIAL_ID)
                            //&& c.POSTING_DATE.HasValue 
                            //&& c.POSTING_DATE.Value.Year <= input.PeriodYear && c.POSTING_DATE.Value.Month <= input.PeriodMonth
                            );
                

                return data.ToList();
            }
            return new List<INVENTORY_MOVEMENT>();
        }

        
 
        public List<INVENTORY_MOVEMENT> GetMvt201(InvMovementGetUsageByParamInput input,bool isAssigned = false)
        {
            var mvtType201 = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage202),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage901),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage902),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.UsageZ01),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.UsageZ02)
            };

            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue
                && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;

            if (input.PlantIdList.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.PlantIdList.Contains(c.PLANT_ID));
            }

            queryFilter = queryFilter.And(c => mvtType201.Contains(c.MVT));


            var data = _repository.Get(queryFilter).ToList();
            if (!isAssigned)
            {
                return data;
            }
            else
            {
                
                return null;
            }
        }

        public List<INVENTORY_MOVEMENT> GetMvt201NotUsed(List<long> usedList)
        {

            var usage201 = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201);
            List<INVENTORY_MOVEMENT> data = _repository.Get(x => (!usedList.Contains(x.INVENTORY_MOVEMENT_ID)) && x.MVT == usage201).ToList();    
            

            return data;
        }

        public List<INVENTORY_MOVEMENT> GetLack1PrimaryResultsCfProduced(GetLack1PrimaryResultsInput input)
        {

            var receiving101 = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101);


            var data = _repository.Get(x => x.MVT == receiving101 && (!input.ListOrdrZaapShiftReport.Contains(x.ORDR)) 
                    && x.POSTING_DATE >= input.DateFrom && x.POSTING_DATE < input.DateTo
                    && string.Compare(x.PLANT_ID, input.PlantFrom) >= 0 
                    && string.Compare(x.PLANT_ID, input.PlantTo)  <= 0).ToList();


            data = data.Where(x => !string.IsNullOrEmpty(x.ORDR)).ToList();
           
            return data;
        }

        public List<INVENTORY_MOVEMENT> GetLack1PrimaryResultsBkc(GetLack1PrimaryResultsInput input)
        {

            var listMvt = new List<string>();
            listMvt.Add(EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261));
            listMvt.Add(EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage262));

            var data = _repository.Get(x=> input.ListOrder.Contains(x.ORDR)
                    && x.POSTING_DATE >= input.DateFrom && x.POSTING_DATE < input.DateTo
                    && string.Compare(x.PLANT_ID, input.PlantFrom) >= 0 && 
                        string.Compare(x.PLANT_ID, input.PlantTo) <= 0
                    && listMvt.Contains(x.MVT)).ToList();


            return data;
        }

        public List<INVENTORY_MOVEMENT> GetBatchByPurchDoc(string purchDoc)
        {
            var receiving101 = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101);
            List<INVENTORY_MOVEMENT> data = _repository.Get(x => x.PURCH_DOC == purchDoc && x.MVT == receiving101).ToList();

            return data;
        }

        public List<INVENTORY_MOVEMENT> GetLack1DetailTis(GetLack1DetailTisInput input)
        {

            var listMvt = new List<string>();
            listMvt.Add(EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101));
            listMvt.Add(EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving102));

            var data = _repository.Get(x => input.ListBatch.Contains(x.BATCH)
                    && x.POSTING_DATE >= input.DateFrom && x.POSTING_DATE <= input.DateTo
                    && !listMvt.Contains(x.MVT)).ToList();

            return data;
        }

        public List<INVENTORY_MOVEMENT> GetLack1DetailEa(GetLack1DetailEaInput input)
        {

            var listMvt = new List<string>();
            listMvt.Add(EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261));

            var data = _repository.Get(x => input.ListBatch.Contains(x.BATCH)
                    && x.POSTING_DATE >= input.DateFrom && x.POSTING_DATE <= input.DateTo
                    && listMvt.Contains(x.MVT)).ToList();

            return data;
        }

        public List<InventoryMovementLevelDto> GetLack1DetailLevel(GetLack1DetailLevelInput input)
        {
            var list = GetLack1DetailLevelList(input);

            var groupedList = list.GroupBy(x => new { x.Level, x.FlavorCode, x.FlavorDesc, x.CfProdCode, x.CfProdDesc, x.CfProdQty, x.CfProdUom, x.ProdPostingDate, x.ProdDate })
                .Select(x => new InventoryMovementLevelDto()
                {
                    Level = x.FirstOrDefault().Level,
                    FlavorCode = x.FirstOrDefault().FlavorCode,
                    FlavorDesc = x.FirstOrDefault().FlavorDesc,
                    CfProdCode = x.FirstOrDefault().CfProdCode,
                    CfProdDesc = x.FirstOrDefault().CfProdDesc,
                    CfProdQty = x.FirstOrDefault().CfProdQty,
                    CfProdUom = x.FirstOrDefault().CfProdUom,
                    ProdPostingDate = x.FirstOrDefault().ProdPostingDate,
                    ProdDate = x.FirstOrDefault().ProdDate
                }).ToList();

            return groupedList;
        }

        private List<InventoryMovementLevelDto> GetLack1DetailLevelList(GetLack1DetailLevelInput input)
        {
            var list = new List<InventoryMovementLevelDto>();

            var mvtList101 = new List<string>();
            mvtList101.Add(EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101));

            var data = _repository.Get(x => input.ListOrdr.Contains(x.ORDR)
                && x.POSTING_DATE >= input.DateFrom && x.POSTING_DATE <= input.DateTo
                && mvtList101.Contains(x.MVT)).ToList();

            var batchList = data.Select(x => x.BATCH).Distinct().ToList();

            var mvtList261 = new List<string>();
            mvtList261.Add(EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261));

            var dataList = _repository.Get(x => batchList.Contains(x.BATCH)
                && x.POSTING_DATE >= input.DateFrom && x.POSTING_DATE <= input.DateTo
                && mvtList261.Contains(x.MVT)).ToList();

            var ordrList = new List<string>();

            foreach (var item in dataList)
            {
                var levelNew = new InventoryMovementLevelDto();
                levelNew.Level = input.Level.ToString();
                levelNew.FlavorCode = item.MATERIAL_ID;
                levelNew.FlavorDesc = string.Empty;
                levelNew.CfProdCode = string.Empty;
                levelNew.CfProdDesc = string.Empty;
                levelNew.CfProdQty = string.Empty;
                levelNew.CfProdUom = string.Empty;
                levelNew.ProdPostingDate = string.Empty;
                levelNew.ProdDate = string.Empty;

                var checkMaterial = _materialService.GetByMaterialAndPlantId(item.MATERIAL_ID, item.PLANT_ID);

                if (checkMaterial == null)
                {
                    ordrList.Add(item.ORDR);
                }
                else
                {
                    var materialItem = new List<string>();
                    materialItem.Add(item.MATERIAL_ID);

                    var umren = Convert.ToDecimal(1);
                    var materialUom = _materialUomService.GetByMaterialListAndPlantId(materialItem, item.PLANT_ID);
                    if (materialUom.Count > 0)
                    {
                        var umrenGram = materialUom.Where(x => x.MEINH == "G").FirstOrDefault();
                        if (umrenGram != null) umren = umrenGram.UMREN.Value;
                    }

                    levelNew.FlavorCode = string.Empty;
                    levelNew.FlavorDesc = string.Empty;
                    levelNew.CfProdCode = item.MATERIAL_ID;
                    levelNew.CfProdDesc = checkMaterial.MATERIAL_DESC;
                    levelNew.CfProdQty = (item.QTY.Value / umren).ToString("N2");
                    levelNew.CfProdUom = "Gram";
                    levelNew.ProdPostingDate = item.POSTING_DATE.Value.ToString("dd-MMM-yy");
                    levelNew.ProdDate = item.POSTING_DATE.Value.ToString("dd-MMM-yy");
                }

                list.Add(levelNew);
            }

            if (ordrList.Count > 0)
            {
                var inputLevelMvt = new GetLack1DetailLevelInput();
                inputLevelMvt.DateFrom = input.DateFrom;
                inputLevelMvt.DateTo = input.DateTo;
                inputLevelMvt.ListOrdr = ordrList;
                inputLevelMvt.Level = input.Level + 1;

                var nextLevel = GetLack1DetailLevel(inputLevelMvt);
                list.AddRange(nextLevel);
            }

            return list;
        }
    }
}
