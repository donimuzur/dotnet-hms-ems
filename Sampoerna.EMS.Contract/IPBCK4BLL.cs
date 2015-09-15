﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
   public interface IPBCK4BLL
   {
       List<Pbck4Dto> GetPbck4ByParam(Pbck4GetByParamInput input);

       Pbck4Dto GetPbck4ById(int id);

       Pbck4DetailsOutput GetDetailsPbck4(int id);

       Pbck4Dto SavePbck4(Pbck4SaveInput input);

       List<Pbck4ItemsOutput> Pbck4ItemProcess(List<Pbck4ItemsInput> inputs);

       void PBCK4Workflow(Pbck4WorkflowDocumentInput input);
   }
}
