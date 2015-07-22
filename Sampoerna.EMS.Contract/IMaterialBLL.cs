using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Contract
{
    public interface IMaterialBLL
    {
        ZAIDM_EX_MATERIAL getByID(string materialnumber);

        List<ZAIDM_EX_MATERIAL> getAll();

        MaterialOutput Save(ZAIDM_EX_MATERIAL data, string userId);

        void Delete(string id, string userId);
    }
}
