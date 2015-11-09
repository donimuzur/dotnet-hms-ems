using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using System.Linq;

namespace Sampoerna.EMS.BLL.Services
{
    public class ZaapShiftRptService : IZaapShiftRptService
    {
        private IGenericRepository<ZAAP_SHIFT_RPT> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public ZaapShiftRptService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAAP_SHIFT_RPT>();
        }

        public List<ZAAP_SHIFT_RPT> GetForLack1ByParam(ZaapShiftRptGetForLack1ByParamInput input)
        {
            Expression<Func<ZAAP_SHIFT_RPT, bool>> queryFilter = c => c.COMPANY_CODE == input.CompanyCode 
                && c.PRODUCTION_DATE.Year == input.PeriodYear && c.PRODUCTION_DATE.Month == input.PeriodMonth;

            if (input.FaCodeList != null && input.FaCodeList.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.FaCodeList.Contains(c.FA_CODE));
            }

            if (input.Werks.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.Werks.Contains(c.WERKS));
            }

            var dbData = _repository.Get(queryFilter);

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return dbData.ToList();

        }
    }
}
