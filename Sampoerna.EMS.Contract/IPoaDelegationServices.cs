using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface IPoaDelegationServices
    {
        List<string> GetListPoaDelegateByDate(List<string> listPoa, DateTime date);
    }
}
