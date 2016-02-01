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
using Enums = Sampoerna.EMS.Core.Enums;

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

            queryFilter = queryFilter.And(c => input.Werks.Contains(c.WERKS));

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

        public List<ZAAP_SHIFT_RPT> GetAll()
        {
            return _repository.Get().ToList();
        }

        public List<ZAAP_SHIFT_RPT> GetReversalData(string plant, string facode)
        {
            Expression<Func<ZAAP_SHIFT_RPT, bool>> queryFilter = PredicateHelper.True<ZAAP_SHIFT_RPT>();

            string receiving102 = EnumHelper.GetDescription(Enums.MovementTypeCode.Receiving102);

            queryFilter = queryFilter.And(c => c.MVT == receiving102);

            if (plant != null)
            {
                queryFilter = queryFilter.And(c => c.WERKS == plant);
            }

            if (facode != null)
            {
                queryFilter = queryFilter.And(c => c.FA_CODE == facode);
            }

            var dbData = _repository.Get(queryFilter);

            return dbData.ToList();
        }

        public ZAAP_SHIFT_RPT GetById(int id)
        {
            var data = _repository.GetQuery(x => x.ZAAP_SHIFT_RPT_ID == id).FirstOrDefault();

            return data;
        }
    }
}
