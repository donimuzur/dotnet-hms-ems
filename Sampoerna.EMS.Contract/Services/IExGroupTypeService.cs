using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IExGroupTypeService
    {
        EX_GROUP_TYPE_DETAILS GetGroupTypeDetailByGoodsType(string input);
    }
}