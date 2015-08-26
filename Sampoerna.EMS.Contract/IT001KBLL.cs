using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IT001KBLL
    {
        T001KDto GetByBwkey(string bwkey);
        T001KDto GetByNppbkcIdAndMainPlant(string nppbkcId);
        List<T001KCompositDto> GetCompositListByCompany(string companyId);
        List<T001KCompositDto> GetCompositListByPlant(string plantId);

    }
}