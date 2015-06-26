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

        ZAIDM_EX_BRAND GetById(long id);

        BrandRegistrationOutput save(ZAIDM_EX_BRAND brandRegistrasionExBrand);

        List<ZAIDM_EX_BRAND> GetAllBrands();
    }

}
