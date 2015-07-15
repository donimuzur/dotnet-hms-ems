using System.Collections.Generic;
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

        List<Pbck1ProdConverterOutput> ValidatePbck1ProdConverterUpload(IEnumerable<Pbck1ProdConverterInput> inputs);
        List<Pbck1ProdPlanOutput> ValidatePbck1ProdPlanUpload(IEnumerable<Pbck1ProdPlanInput> inputs);

        List<Pbck1Dto> GetByDocumentStatus(Pbck1GetByDocumentStatusParam input);

    }
}
