using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IBack1Services
    {
        void SaveBack1ByCk5Id(SaveBack1ByCk5IdInput input);

        BACK1 GetBack1ByCk5Id(long ck5Id);
    }
}
