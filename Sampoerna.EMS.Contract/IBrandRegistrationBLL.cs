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

        ZAIDM_EX_BRAND GetById(string plant, string facode);

        ZAIDM_EX_BRAND GetByIdIncludeChild(string plant, string facode);

        string Save(ZAIDM_EX_BRAND brandRegistration);

        List<ZAIDM_EX_BRAND> GetAllBrands();

        void Delete(string plant, string facode);

        ZAIDM_EX_BRAND GetByFaCode(string faCode);

        ZAIDM_EX_BRAND GetByPlantIdAndFaCode(string plantId, string faCode);
    }

}
