using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.BusinessObject;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class CompanyBLL : ICompanyBLL
    {
        private IGenericRepository<T1001> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public CompanyBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T1001>();
        }

        public List<T1001> GetMasterData()
        {
           var queryData = (from aa in _repository.GetQuery()
                join b in
                    (from a in _repository.GetQuery()
                        group a by new {a.BUKRS}
                        into g
                        
                        select new
                       
                        {
                            BUKRS = g.Key.BUKRS,
                            MAXDATE = g.Max(x => x.CREATED_DATE)
                        }) on aa.BUKRS equals b.BUKRS
                where aa.CREATED_DATE == b.MAXDATE
                select aa);
            return queryData.ToList();
        }

        public List<T1001> GetAllData()
        {
            return _repository.Get().ToList();
        }
    }
}
