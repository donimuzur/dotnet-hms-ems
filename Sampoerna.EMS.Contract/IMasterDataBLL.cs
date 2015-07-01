using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IMasterDataBLL
    {
        List<string> GetDataCompany();

        List<AutoCompletePlant> GetAutoCompletePlant();
    }
}
