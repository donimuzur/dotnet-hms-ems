using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IStatusGovBLL
    {
        STATUS_GOV GetById(int id);

        List<STATUS_GOV> GetAll();

    }
}