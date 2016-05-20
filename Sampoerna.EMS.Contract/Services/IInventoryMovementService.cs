using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IInventoryMovementService
    {
        
        List<INVENTORY_MOVEMENT> GetUsageByParam(InvMovementGetUsageByParamInput input);

        List<INVENTORY_MOVEMENT> GetReceivingByParam(InvMovementGetReceivingByParamInput input);

        INVENTORY_MOVEMENT GetReceivingByProcessOrderAndPlantId(string processOrder, string plantId);

        INVENTORY_MOVEMENT GetById(long id);

        INVENTORY_MOVEMENT GetUsageByBatchAndPlantId(string batch, string plantId);

        List<INVENTORY_MOVEMENT> GetUsageByBatchAndPlantIdInPeriod(GetUsageByBatchAndPlantIdInPeriodParamInput input);

        List<INVENTORY_MOVEMENT> GetReceivingByOrderAndPlantIdInPeriod(
            GetReceivingByOrderAndPlantIdInPeriodParamInput input);

        List<INVENTORY_MOVEMENT> GetMvt201(InvMovementGetUsageByParamInput input, bool isAssigned = false);

        List<INVENTORY_MOVEMENT> GetMvt201NotUsed(List<long> usedList);

        List<INVENTORY_MOVEMENT> GetLack1PrimaryResultsCfProduced(GetLack1PrimaryResultsInput input);

        List<INVENTORY_MOVEMENT> GetLack1PrimaryResultsBkc(GetLack1PrimaryResultsInput input);

        List<INVENTORY_MOVEMENT> GetReceivingByParamZaapShiftRpt(InvGetReceivingByParamZaapShiftRptInput input);

        List<INVENTORY_MOVEMENT> GetBatchByPurchDoc(string purchDoc);

        List<INVENTORY_MOVEMENT> GetLack1DetailTis(GetLack1DetailTisInput input);

        List<INVENTORY_MOVEMENT> GetLack1DetailEa(GetLack1DetailEaInput input);

        List<InventoryMovementLevelDto> GetLack1DetailLevel(GetLack1DetailLevelInput input);
    }
}