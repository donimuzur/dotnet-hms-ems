using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class LFA1BLL : ILFA1BLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LFA1> _repository;
        private string includeTables = "";

        public LFA1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LFA1>();
        }
        public List<LFA1Dto> GetAll()
        {
            return Mapper.Map<List<LFA1Dto>>(_repository.Get(null, null, includeTables).ToList());
        }

        public LFA1Dto GetById(string id)
        {
            return Mapper.Map<LFA1Dto>(_repository.Get(c => c.LIFNR == id, null, includeTables).FirstOrDefault());
        }
    }
}
