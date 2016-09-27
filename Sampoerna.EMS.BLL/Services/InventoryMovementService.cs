using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class InventoryMovementService : IInventoryMovementService
    {

        private IGenericRepository<INVENTORY_MOVEMENT> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAAP_SHIFT_RPT> _zaapShiftRptRepository;
        private IGenericRepository<ZAIDM_EX_BRAND> _zaidmExBrandRepository;

        public InventoryMovementService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<INVENTORY_MOVEMENT>();
            _zaapShiftRptRepository = _uow.GetGenericRepository<ZAAP_SHIFT_RPT>();
            _zaidmExBrandRepository = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
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

        public INVENTORY_MOVEMENT GetReceivingByProcessOrderAndPlantId(string processOrder, string plantId)
        {
            var mvtReceiving = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101);
            return _repository.Get(c => c.ORDR == processOrder && c.PLANT_ID == plantId && c.MVT == mvtReceiving).FirstOrDefault();
        }

        public INVENTORY_MOVEMENT GetById(long? id)
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

        public List<INVENTORY_MOVEMENT> GetReversalData(string plant, string facode)
        {
            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = PredicateHelper.True<INVENTORY_MOVEMENT>();

            string receiving102 = EnumHelper.GetDescription(Enums.MovementTypeCode.Receiving102);

            queryFilter = queryFilter.And(c => c.MVT == receiving102);

            if (plant != null)
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == plant);
            }

            if (facode != null)
            {
                queryFilter = queryFilter.And(c => c.MATERIAL_ID == facode);
            }

            //var dbData = _repository.Get(queryFilter).Join(_zaidmExBrandRepository, i => new { i.MATERIAL_ID, i.PLANT_ID }, b => new { MATERIAL_ID=b.});

            var result = from i in _repository.GetQuery()
                         join b in _zaidmExBrandRepository.GetQuery()
                         on new { i.MATERIAL_ID, i.PLANT_ID }
                         equals new { MATERIAL_ID = b.FA_CODE, PLANT_ID=b.WERKS }
                         where i.MVT == receiving102 && b.PROD_CODE=="05" && b.EXC_GOOD_TYP == "02" &&
                               i.PLANT_ID==plant && i.MATERIAL_ID == facode
                         select i;

            //return result.Where(queryFilter).ToList();
            return result.ToList();
        }
    }
}
