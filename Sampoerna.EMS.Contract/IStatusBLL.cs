using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IStatusBLL
    {
        STATUS GetById(int id);
        List<STATUS> GetAll();
    }
}