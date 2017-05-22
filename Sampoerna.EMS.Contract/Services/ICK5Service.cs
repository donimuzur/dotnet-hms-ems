using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract.Services
{
    public interface ICK5Service
    {
        CK5 GetById(long id);
        List<CK5> GetForLack1ByParam(Ck5GetForLack1ByParamInput input);
        List<CK5> GetForLack2ByParam(Ck5GetForLack2ByParamInput input);

        WasteStockQuotaOutput GetWasteStockQuota(decimal wasteStock, string plantId, string materialNumber);

        List<CK5> GetByStoNumberList(List<string> stoNumberList);

        List<CK5> GetReconciliationLack1();

        List<string> GetCk5AssignedMatdoc();

        List<CK5> GetAllPreviousForLack1(Ck5GetForLack1ByParamInput input);

        List<CK5> GetCk5WasteByParam(Ck5GetForLack1ByParamInput input);

        List<CK5> GetCk5ReturnByParam(Ck5GetForLack1ByParamInput input);

        List<string> GetMaterialListbyCk5IdList(List<long> ck5idList);

        List<CK5> GetCk5ForLack1DetailTis(Ck5GetForLack1DetailTis input);

        List<CK5> GetCk5ForLack1DetailEa(Ck5GetForLack1DetailEa input);

        
    }
}