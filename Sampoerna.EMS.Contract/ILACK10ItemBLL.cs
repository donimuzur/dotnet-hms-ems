using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK10ItemBLL
    {
        void DeleteByLack10Id(long lack10Id);
    }
}
