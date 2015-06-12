using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExPOAMapBLL : IZaidmPOAMapBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_POA_MAP> _repository;
        private string includeTables = "ZAIDM_EX_POA";

        public ZaidmExPOAMapBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_POA_MAP>();
        }

        public List<ZAIDM_EX_POA> GetPOAByNPPBKCID(string NPPBKCID)
        {
            Expression<Func<ZAIDM_POA_MAP, bool>> queryFilter = PredicateHelper.True<ZAIDM_POA_MAP>();

            if (!string.IsNullOrEmpty(NPPBKCID))
            {
                queryFilter = queryFilter.And(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && c.NPPBKC_ID.Contains(NPPBKCID));
            }

            var dbData = _repository.Get(queryFilter, null, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return dbData.Select(s => s.ZAIDM_EX_POA).Distinct().ToList();

        }

    }
}
