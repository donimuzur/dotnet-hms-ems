using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
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
        public ZAIDM_EX_KPPBCDto GetById(string id)
        {
            return Mapper.Map<ZAIDM_EX_KPPBCDto>(_repository.GetByID(id));
        }

        public ZAIDM_EX_KPPBC GetKppbcById(string id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_KPPBCDto> GetAll()
        {
            return Mapper.Map<List<ZAIDM_EX_KPPBCDto>>(_repository.Get().ToList());
        }

        public void Save(ZAIDM_EX_KPPBC kppbc)
        {
            _repository.InsertOrUpdate(kppbc);
            _uow.SaveChanges();
        }
    }
}
