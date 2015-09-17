using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IBlockStockBLL
    {

        BLOCK_STOCKDto GetBlockStockById(long id);

        List<BLOCK_STOCKDto> GetBlockStockByPlantAndMaterialId(string plantId, string materialId);
    }
}
