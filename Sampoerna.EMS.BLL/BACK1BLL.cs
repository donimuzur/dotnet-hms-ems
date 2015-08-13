using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BLL
{
    public class BACK1BLL : IBACK1BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        //private IGenericRepository<BACK1> _repository;
        
        public BACK1BLL(IUnitOfWork uow,ILogger logger)
        {
           _uow = uow;
           _logger = logger;
            //_repository = _uow.GetGenericRepository<BACK1>();

        }

        public Back1Dto GetId(int id)
        {
            throw new NotImplementedException();
            //return _repository.getByID(id);
        }

        public List<Back1Dto> GetAll()
        {
            throw new NotImplementedException();
            //return _repository.Get().ToList();
        }
    }
}
