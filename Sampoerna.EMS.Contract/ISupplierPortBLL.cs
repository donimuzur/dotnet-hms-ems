using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface ISupplierPortBLL
    {
        SUPPLIER_PORT GetById(int id);
        List<SUPPLIER_PORT> GetAll();
    }
}