using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
   public interface ICK5BLL
   {
       List<CK5> GetAll();

       List<CK5> GetCK5ByParam(CK5Input input);
   }
}
