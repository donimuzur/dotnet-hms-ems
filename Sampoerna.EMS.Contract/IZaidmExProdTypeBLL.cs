using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExProdTypeBLL
    {
        ZAIDM_EX_PRODTYP GetById(long id);

        List<ZAIDM_EX_PRODTYP> GetAll();
        ZAIDM_EX_PRODTYP GetByCode(int Code);
    }
}