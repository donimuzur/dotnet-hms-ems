using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Contract
{
    public interface IMasterDataAprovalBLL
    {
        T MasterDataApprovalValidation<T>(int pageId, string userId, T oldObject, T newObject);
    }
}
