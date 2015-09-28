using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class CK1BLL : ICK1BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<CK1> _repository;

        private string includeTables = "";

        public CK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _repository = _uow.GetGenericRepository<CK1>();
        }


        public CK1Dto GetCk1ByCk1Number(string ck1Number)
        {
            var dtData = _repository.Get(c => c.CK1_NUMBER == ck1Number, null, includeTables).FirstOrDefault();
           
            return Mapper.Map<CK1Dto>(dtData);

        }

        
    }
}
