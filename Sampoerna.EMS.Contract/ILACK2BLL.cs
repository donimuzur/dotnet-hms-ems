using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK2BLL
    {
        List<Lack2Dto> GetAll(Lack2GetByParamInput input);

        List<Lack2Dto> GetAllCompleted();

        Lack2Dto GetById(int id);

        Lack2Dto Insert(Lack2Dto item);
    }
}
