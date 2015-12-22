
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteStockServices
    {
        List<WASTE_STOCK> GetListByPlant(string plantId);

        WASTE_STOCK GetByPlantAndMaterialNumber(string plantId, string materialNumber);
    }
}
