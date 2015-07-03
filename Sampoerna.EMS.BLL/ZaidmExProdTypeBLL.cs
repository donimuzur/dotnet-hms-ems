using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExProdTypeBLL : IZaidmExProdTypeBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_PRODTYP> _repository;
        
        public ZaidmExProdTypeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
        }

        public ZAIDM_EX_PRODTYP GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_PRODTYP> GetAll()
        {
            return _repository.Get().ToList();
        }
        public ZAIDM_EX_PRODTYP GetByCode(int Code)
        {
            return _repository.Get(p=>p.PRODUCT_CODE == Code).OrderByDescending(x=>x.CREATED_DATE).FirstOrDefault();
        }
    }
}
