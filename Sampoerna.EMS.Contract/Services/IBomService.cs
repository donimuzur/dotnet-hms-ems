using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IBomService
    {
        List<BOM> GetBomByPlantIdAndMaterial(string plantId, string materialId);
    }
}
