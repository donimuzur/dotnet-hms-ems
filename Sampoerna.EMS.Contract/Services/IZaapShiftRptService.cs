﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using System;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IZaapShiftRptService
    {
        List<ZAAP_SHIFT_RPT> GetForLack1ByParam(ZaapShiftRptGetForLack1ByParamInput input);
        List<ZAAP_SHIFT_RPT> GetAll();
        List<ZAAP_SHIFT_RPT> GetReversalData(string plant, string facode);
        ZAAP_SHIFT_RPT GetById(int? id);

        List<ZAAP_SHIFT_RPT> GetCompleteData(ZaapShiftRptGetForLack1ByParamInput input);

        List<ZAAP_SHIFT_RPT> GetForCFVsFa(ZaapShiftRptGetForLack1ReportByParamInput input);

        List<ZAAP_SHIFT_RPT> GetForLack1ByParam(InvGetReceivingByParamZaapShiftRptInput input);

        List<ZAAP_SHIFT_RPT> GetReversalDataByDate(GetProductionDailyProdByParamInput input);
    }
}