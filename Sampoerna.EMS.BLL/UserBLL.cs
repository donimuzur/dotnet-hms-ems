using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class UserBLL
    {

        private IGenericRepository<USER> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public UserBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<USER>();
        }
        
    }
}
