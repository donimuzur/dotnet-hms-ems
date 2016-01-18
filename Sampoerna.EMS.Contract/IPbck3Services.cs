
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPbck3Services
    {

        string InsertPbck3FromCk5MarketReturn(InsertPbck3FromCk5MarketReturnInput input);

        string InsertPbck3FromPbck7(InsertPbck3FromPbck7Input input);

        PBCK3 GetPbck3ByCk5Id(long ck5Id);
    }
}
