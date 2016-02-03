using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IZaapShiftRptService
    {
        List<ZAAP_SHIFT_RPT> GetForLack1ByParam(ZaapShiftRptGetForLack1ByParamInput input);
        List<ZAAP_SHIFT_RPT> GetAll();
        List<ZAAP_SHIFT_RPT> GetReversalData(string plant, string facode);
        ZAAP_SHIFT_RPT GetById(int id);
    }
}