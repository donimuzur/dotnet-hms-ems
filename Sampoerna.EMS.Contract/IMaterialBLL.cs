using Sampoerna.EMS.BusinessObject;
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
        ZAIDM_EX_MATERIAL getByID(long materialId);

        List<ZAIDM_EX_MATERIAL> getAll();

        MaterialOutput Save(ZAIDM_EX_MATERIAL data);
    }
}
