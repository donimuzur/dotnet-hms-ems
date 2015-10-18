

using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IBack3Services
    {
        BACK3 GetBack3ByPbck3Id(int pbck3Id);

        void SaveBack3ByPbck3Id(SaveBack3ByPbck3IdInput input);

        bool IsExistBack3DocumentByPbck3(int pbck3Id);
    }
}
