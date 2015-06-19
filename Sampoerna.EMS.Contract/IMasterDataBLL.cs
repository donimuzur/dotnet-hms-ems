using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IMasterDataBLL
    {
        List<string> GetDataCompany();

        List<EX_SETTLEMENT> GetAllExciseExSettlements();

        List<EX_STATUS> GetAllExciseStatus();

        List<REQUEST_TYPE> GetAllRequestTypes();
    }
}
