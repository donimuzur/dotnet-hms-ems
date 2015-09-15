using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IT001WService
    {
        T001W GetById(string werks);
        List<T001W> GetByNppbkcId(string nppbkcId);
    }
}