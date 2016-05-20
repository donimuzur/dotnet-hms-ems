using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExProdTypeBLL
    {
        ZAIDM_EX_PRODTYP GetById(string id);

        List<ZAIDM_EX_PRODTYP> GetAll();
        ZAIDM_EX_PRODTYP GetByCode(string Code);

        ZAIDM_EX_PRODTYP GetByAlias(string aliasName);

        void UpdateProductType(ProductTypeSaveInput input);
    }
}