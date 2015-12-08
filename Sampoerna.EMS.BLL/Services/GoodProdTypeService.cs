using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class GoodProdTypeService : IGoodProdTypeService
    {

        private IGenericRepository<GOOD_PROD_TYPE> _repository;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _goodTypeRepository;
        private IGenericRepository<ZAIDM_EX_PRODTYP> _prodTypeRepository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public GoodProdTypeService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<GOOD_PROD_TYPE>();
            _goodTypeRepository = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
            _prodTypeRepository = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
        }

        public ZAIDM_EX_GOODTYP GetGoodTypeByProdCode(string prodCode)
        {
            var goodTypeData = _repository.Get(c => c.PROD_CODE == prodCode).FirstOrDefault();
            return goodTypeData == null ? null : _goodTypeRepository.Get(c => c.EXC_GOOD_TYP == goodTypeData.EXC_GOOD_TYP).FirstOrDefault();
        }

        public ZAIDM_EX_PRODTYP GetProdCodeByGoodTypeId(string goodTypeId)
        {
            var goodTypeData = _repository.Get(c => c.EXC_GOOD_TYP == goodTypeId).FirstOrDefault();
            return goodTypeData == null ? null : _prodTypeRepository.Get(c => c.PROD_CODE == goodTypeData.PROD_CODE).FirstOrDefault();
        }
    }
}
