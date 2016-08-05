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

            if (input.AllowedOrder != null && input.AllowedOrder.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.AllowedOrder.Contains(c.ORDR));
            }

            var dbData = _repository.Get(queryFilter);

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var retData = dbData.GroupBy(x => new { x.ORDR, x.COMPANY_CODE, x.WERKS, x.FA_CODE,x.PRODUCTION_DATE })
                .Select(x=>
                {
                    var zaapShiftRpt = x.FirstOrDefault();
                    return zaapShiftRpt != null ? new ZAAP_SHIFT_RPT()
                                {
                                    ORDR = zaapShiftRpt.ORDR,
                                    COMPANY_CODE = zaapShiftRpt.COMPANY_CODE,
                                    WERKS = zaapShiftRpt.WERKS,
                                    FA_CODE = zaapShiftRpt.FA_CODE,
                                    PRODUCTION_DATE = zaapShiftRpt.PRODUCTION_DATE,
                                    UOM = zaapShiftRpt.UOM
                                } : null;
                }).ToList();
                //(from data in dbData
                //           group new {}
                //           select new ZAAP_SHIFT_RPT()
                //           {
                //               ORDR = data.ORDR,
                //               COMPANY_CODE = data.COMPANY_CODE,
                //               WERKS = data.WERKS,
                //               FA_CODE = data.FA_CODE,
                //               PRODUCTION_DATE = data.PRODUCTION_DATE
                //           }).Distinct().ToList();
            return retData;

        }

        public List<ZAAP_SHIFT_RPT> GetCompleteData(ZaapShiftRptGetForLack1ByParamInput input)
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

            if (input.AllowedOrder != null && input.AllowedOrder.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.AllowedOrder.Contains(c.ORDR));
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
