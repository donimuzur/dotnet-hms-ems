using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.DAL
{
    public class SqlSPRepository : ISqlSPRepository
    {
        internal EMSEntities _context;

        private ILogger _logger;

        public SqlSPRepository(EMSEntities context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        
    }
}
