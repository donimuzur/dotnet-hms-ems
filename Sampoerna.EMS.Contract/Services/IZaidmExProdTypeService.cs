using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IZaidmExProdTypeService
    {
        List<ZAIDM_EX_PRODTYP> GetAll();
    }
}