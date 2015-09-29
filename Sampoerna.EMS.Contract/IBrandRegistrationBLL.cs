using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IBrandRegistrationBLL
    {
        List<BrandRegistrationOutput> GetAll();

        ZAIDM_EX_BRAND GetById(string plant, string facode);

        ZAIDM_EX_BRAND GetByIdIncludeChild(string plant, string facode);

        void Save(ZAIDM_EX_BRAND brandRegistration);

        List<ZAIDM_EX_BRAND> GetAllBrands();

        bool Delete(string plant, string facode);

        ZAIDM_EX_BRAND GetByFaCode(string plantWerk, string faCode );
        
        List<ZAIDM_EX_BRAND> GetByPlantId(string plantId);

        List<ZAIDM_EX_BRAND> GetBrandCeBylant(string plantWerk);

        ZAIDM_EX_GOODTYP GetGoodTypeByProdCodeInBrandRegistration(string prodCode);

    }

}
