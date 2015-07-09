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

        ZAIDM_EX_BRAND GetByIdIncludeChild(long id);

        void Save(ZAIDM_EX_BRAND brandRegistration);

        List<ZAIDM_EX_BRAND> GetAllBrands();

        void Delete(long id);

        ZAIDM_EX_BRAND GetByFaCode(string faCode);

        ZAIDM_EX_BRAND GetByPlantIdAndFaCode(long plantId, string faCode);
    }

}
