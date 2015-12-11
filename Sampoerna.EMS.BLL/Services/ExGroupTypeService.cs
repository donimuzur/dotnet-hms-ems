using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class ExGroupTypeService : IExGroupTypeService
    {
        private IGenericRepository<EX_GROUP_TYPE_DETAILS> _groupTypeRepository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public ExGroupTypeService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _groupTypeRepository = _uow.GetGenericRepository<EX_GROUP_TYPE_DETAILS>();
        }

        public EX_GROUP_TYPE_DETAILS GetGroupTypeDetailByGoodsType(string input)
        {
            return _groupTypeRepository.Get(c => c.GOODTYPE_ID == input, null, "").FirstOrDefault();
        }
    }
}
