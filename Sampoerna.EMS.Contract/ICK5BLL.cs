using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
   public interface ICK5BLL
   {

       CK5Dto GetById(long id);

       CK5 GetByIdIncludeTables(long id);

       List<CK5Dto> GetAll();

       List<CK5Dto> GetCK5ByParam(CK5GetByParamInput input);

       CK5SaveOutput SaveCk5(CK5SaveInput input);

       List<CK5> GetCK5ByType(Enums.CK5Type ck5Type);

       List<CK5MaterialOutput> CK5MaterialProcess(List<CK5MaterialInput> inputs);

       CK5DetailsOutput GetDetailsCk5(long id);
   }
}
