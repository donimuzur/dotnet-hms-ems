using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class PoaDelegationServices : IPoaDelegationServices
    {
         private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<POA_DELEGATION> _repository;

        private string _includeTables = "POA, USER";

        public PoaDelegationServices(IUnitOfWork uow, ILogger logger)
        {
           _uow = uow;
           _logger = logger;
           _repository = _uow.GetGenericRepository<POA_DELEGATION>();

        }

        public List<string> GetListPoaDelegateByDate(List<string> listPoa, DateTime date)
        {
            var inputDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            var result = _repository.Get(c => listPoa.Contains(c.POA_FROM) && c.DATE_FROM <= inputDate && c.DATE_TO >= inputDate);
            //var result = _repository.Get(c => listPoa.Contains(c.POA_FROM));
            return result.Select(c => c.POA_TO).Distinct().ToList();
        }
    }
}
