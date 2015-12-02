using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface ICK1Services
    {
        CK1 GetCk1ByCk1Number(string ck1Number);

        List<CK1> GetCk1ByNppbkc(string nppbkcId);

        CK1 GetCk1ById(long ck1Id);

        List<CK1> GetCk1ByPlant(string plant);
    }
}
