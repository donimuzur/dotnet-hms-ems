using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IGoodProdTypeService
    {
        ZAIDM_EX_GOODTYP GetGoodTypeByProdCode(string prodCode);
        ZAIDM_EX_PRODTYP GetProdCodeByGoodTypeId(string goodTypeId);
    }
}