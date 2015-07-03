using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public  interface IZaidmExKPPBCBLL
    {
        ZAIDM_EX_KPPBC GetById(long id);

        List<ZAIDM_EX_KPPBC> GetAll();
        
    }
}
