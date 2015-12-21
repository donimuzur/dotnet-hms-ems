using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IZaidmExMaterialService
    {
        ZAIDM_EX_MATERIAL GetByMaterialAndPlantId(string materialId, string plantId);
    }
}