using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public  interface IZaidmExKPPBCBLL
    {
        ZAIDM_EX_KPPBCDto GetById(string id);

        List<ZAIDM_EX_KPPBCDto> GetAll();
        
    }
}
