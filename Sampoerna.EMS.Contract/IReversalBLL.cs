using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IReversalBLL
    {
        List<ReversalDto> GetListDocumentByParam(ReversalGetByParamInput input);
        ReversalDto Save(ReversalDto item, string userId);
        ReversalDto GetById(long id);
        ReversalOutput CheckData(ReversalCreateParamInput reversalInput);
        List<ReversalDto> GetListByParam(string plant, string facode, DateTime prodDate);

        List<REVERSAL> GetAllReversal();
    }
}
