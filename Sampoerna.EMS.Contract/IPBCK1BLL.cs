using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPBCK1BLL
    {

        List<Pbck1> GetPBCK1ByParam(Pbck1GetByParamInput input);

        Pbck1 GetById(long id);

        SavePbck1Output Save(Pbck1SaveInput input);

        DeletePBCK1Output Delete(long id);

    }
}
