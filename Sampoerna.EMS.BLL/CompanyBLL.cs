using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.BusinessObject;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class CompanyBLL : ICompanyBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        public CompanyBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
        }

        public List<T1001_H> GetMasterData()
        {
            var repoCompany = _uow.GetGenericRepository<T1001_H>();
            var queryData = (from aa in repoCompany.GetQuery() join b in
                (from a in repoCompany.GetQuery()
                    group a by new {a.BUKRS}
                    into g
                    select new
                    {
                        BUKRS = g.Key.BUKRS,
                        MAXDATE = g.Max(x => x.CREATED_DATE)
                    }) on aa.BUKRS equals  b.BUKRS
                                 where aa.CREATED_DATE == b.MAXDATE
                 select aa);
            
            return queryData.ToList();
        }
    }
}
