using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExKPPBCBLL :IZaidmExKPPBCBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_KPPBC > _repository;

        public ZaidmExKPPBCBLL(ILogger logger, IUnitOfWork uow)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_KPPBC>();
        }
        public ZAIDM_EX_KPPBC GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_KPPBC> GetAll()
        {
            return _repository.Get().ToList();
        }
    }
}
