using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
   public interface ICK5BLL
   {

       CK5 GetById(long id);

       List<CK5> GetAll();

       List<CK5> GetCK5ByParam(CK5Input input);

       void SaveCk5(CK5 ck5);

       List<CK5> GetCK5ByType(Enums.CK5Type ck5Type);
   }
}
