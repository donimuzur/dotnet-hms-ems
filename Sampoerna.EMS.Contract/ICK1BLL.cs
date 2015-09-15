using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface ICK1BLL
    {
        CK1Dto GetCk1ByCk1Number(string ck1Number);
    }
}
