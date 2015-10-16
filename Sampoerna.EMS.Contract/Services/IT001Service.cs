using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IT001Service
    {
        T001 GetById(string id);
    }
}