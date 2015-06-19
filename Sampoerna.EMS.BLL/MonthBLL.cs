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
    public class MonthBLL : IMonth
    {
        private IGenericRepository<MONTH> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public MonthBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<MONTH>();
        }

        public MONTH GetMonth(int id)
        {
            return _repository.GetByID(id);
        }
    }
}
