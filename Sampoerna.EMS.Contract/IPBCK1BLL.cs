using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPBCK1BLL
    {

        Pbck1GetByParamOutput Pbck1GetByParam(Pbck1GetByParamInput input);

        Pbck1Dto GetById(long id);

        SavePbck1Output Save(Pbck1SaveInput input);
        
        DeletePbck1Output Delete(long id);

        string GetPbckNumberById(long id);

    }
}
