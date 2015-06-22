using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IBrandRegistrationBLL
    {
        List<BrandRegistrationOutput> GetAll();

        BrandRegistrationOutput save(ZAIDM_EX_BRAND brandRegistrasionExBrand);
    }

}
