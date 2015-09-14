using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPBCK7And3BLL
    {
        List<Pbck7AndPbck3Dto> GetAllByParam(Pbck7AndPbck3Input input);

        void Insert(Pbck7AndPbck3Dto pbck7AndPbck3Dto);
    }
}
