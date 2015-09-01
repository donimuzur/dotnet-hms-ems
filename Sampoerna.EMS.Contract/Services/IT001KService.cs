using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IT001KService
    {
        T001K GetByBwkey(string input);
    }
}