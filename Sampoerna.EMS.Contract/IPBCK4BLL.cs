using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
   public interface IPBCK4BLL
   {
       List<Pbck4Dto> GetPbck4ByParam(Pbck4GetByParamInput input);
   }
}
