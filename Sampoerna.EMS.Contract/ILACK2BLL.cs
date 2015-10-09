using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK2BLL
    {
        List<Lack2Dto> GetAll();
        List<Lack2Dto> GetDocumentByParam(Lack2GetByParamInput input);
        void Lack2Workflow(Lack2WorkflowDocumentInput input);
    }
}
