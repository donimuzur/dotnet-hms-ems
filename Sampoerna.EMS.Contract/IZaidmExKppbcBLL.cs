using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public  interface IZaidmExKPPBCBLL
    {
        ZAIDM_EX_KPPBCDto GetById(string id);
        ZAIDM_EX_KPPBC GetKppbcById(string id);
        List<ZAIDM_EX_KPPBCDto> GetAll();

        void Save(ZAIDM_EX_KPPBC kppbc);

        

    }
}
