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

namespace Sampoerna.EMS.BLL.Services
{
    public class InventoryMovementService : IInventoryMovementService
    {

        private IGenericRepository<INVENTORY_MOVEMENT> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAAP_SHIFT_RPT> _zaapShiftRptRepository;

        public InventoryMovementService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<INVENTORY_MOVEMENT>();
            _zaapShiftRptRepository = _uow.GetGenericRepository<ZAAP_SHIFT_RPT>();
        }

        public List<INVENTORY_MOVEMENT> GetUsageByParam(InvMovementGetUsageByParamInput input)
        {
            var usageMvtType = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage202),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage262),
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

            queryFilter = queryFilter.And(c => usageMvtType.Contains(c.MVT));

            if (input.IsEtilAlcohol) return _repository.Get(queryFilter).ToList();

            var allOrderInZaapShiftRpt = _zaapShiftRptRepository.Get().Select(d => d.ORDR).Distinct().ToList();

            queryFilter = input.IsTisToTis ? queryFilter.And(c => !allOrderInZaapShiftRpt.Contains(c.ORDR)) : queryFilter.And(c => allOrderInZaapShiftRpt.Contains(c.ORDR));

            return _repository.Get(queryFilter).ToList();

        }

        public List<INVENTORY_MOVEMENT> GetReceivingByParam(InvMovementGetReceivingByParamInput input)
        {
            var receivingMvtType = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving102)
            };

            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue
                && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;

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

        public List<INVENTORY_MOVEMENT> GetUsageByBatchAndPlantIdInPeriod(GetUsageByBatchAndPlantIdInPeriodParamInput input)
        {

            var mvtUsage = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage261);
            var data =
                _repository.Get(
                    c =>
                        c.BATCH == input.Batch && c.PLANT_ID == input.PlantId &&
                        c.MVT == mvtUsage && c.POSTING_DATE.HasValue &&
                        c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth);

            return data.ToList();
        }

        public List<INVENTORY_MOVEMENT> GetReceivingByOrderAndPlantIdInPeriod(GetReceivingByOrderAndPlantIdInPeriodParamInput input)
        {

            var mvtReceiving = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101);

            var data =
                _repository.Get(
                    c =>
                        c.ORDR == input.Ordr && c.PLANT_ID == input.PlantId &&
                        c.MVT == mvtReceiving && c.POSTING_DATE.HasValue &&
                        c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth);

            return data.ToList();
        }

        public List<INVENTORY_MOVEMENT> GetMvt201(InvMovementGetUsageByParamInput input)
        {
            var mvtType201 = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201)
            };

            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue
                && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;

            if (input.PlantIdList.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.PlantIdList.Contains(c.PLANT_ID));
            }

            queryFilter = queryFilter.And(c => mvtType201.Contains(c.MVT));

            return _repository.Get(queryFilter).ToList();
        }
    }
}
