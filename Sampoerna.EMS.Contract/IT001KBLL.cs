using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IT001KBLL
    {
        T001KDto GetByBwkey(string bwkey);
        T001KDto GetByNppbkcIdAndMainPlant(string nppbkcId);
    }
}