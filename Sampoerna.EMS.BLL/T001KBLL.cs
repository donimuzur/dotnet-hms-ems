using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class T001KBLL : IT001KBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<T001K> _repository;
        private string includeTables = "T001";

        public T001KBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T001K>();
        }

        public T001KDto GetByBwkey(string bwkey)
        {
            var dbData = _repository.Get(c => c.BWKEY == bwkey, null, includeTables).FirstOrDefault();
            return Mapper.Map<T001KDto>(dbData);
        }

    }
}
