using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public  interface IXmlDataReader
    {
        MovedFileOutput InsertToDatabase();

        List<string> GetErrorList();
    }
}
