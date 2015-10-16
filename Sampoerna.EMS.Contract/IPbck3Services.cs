
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPbck3Services
    {

        void InsertPbck3FromCk5MarketReturn(InsertPbck3FromCk5MarketReturnInput input);

        void InsertPbck3FromPbck7(InsertPbck3FromPbck7Input input);
    }
}
