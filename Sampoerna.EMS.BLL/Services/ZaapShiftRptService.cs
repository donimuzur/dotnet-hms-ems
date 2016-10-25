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

        public List<ZAAP_SHIFT_RPT> GetForLack1ByParam(InvGetReceivingByParamZaapShiftRptInput input)
        {
            Expression<Func<ZAAP_SHIFT_RPT, bool>> queryFilter = c => c.POSTING_DATE.HasValue && c.POSTING_DATE.Value <= input.EndDate;

            queryFilter = queryFilter.And(c => c.POSTING_DATE.HasValue && c.POSTING_DATE >= input.StartDate);



            queryFilter = queryFilter.And(c => c.WERKS == input.PlantId);
            
            queryFilter = queryFilter.And(c => c.FA_CODE == input.FaCode);
            

            queryFilter = queryFilter.And(c => c.ORDR == input.Ordr);


            //queryFilter = queryFilter.And(c => receivingMvtType.Contains(c.MVT));





            return _repository.Get(queryFilter).ToList();
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

        public List<ZAAP_SHIFT_RPT> GetReversalDataByDate(GetProductionDailyProdByParamInput input)
        {
            Expression<Func<ZAAP_SHIFT_RPT, bool>> queryFilter = PredicateHelper.True<ZAAP_SHIFT_RPT>();

            string receiving102 = EnumHelper.GetDescription(Enums.MovementTypeCode.Receiving102);

            queryFilter = queryFilter.And(c => c.MVT == receiving102);

            queryFilter = queryFilter.And(x => x.PRODUCTION_DATE >= input.DateFrom && x.PRODUCTION_DATE <= input.DateTo
                 && string.Compare(x.WERKS, input.PlantFrom) >= 0 && string.Compare(x.WERKS, input.PlantTo) <= 0);

            var dbData = _repository.Get(queryFilter).GroupBy(x=> new {x.PRODUCTION_DATE, x.FA_CODE, x.WERKS})
                .Select(x=> new ZAAP_SHIFT_RPT()
                {
                    FA_CODE = x.Key.FA_CODE,
                    WERKS = x.Key.WERKS,
                    PRODUCTION_DATE = x.Key.PRODUCTION_DATE,
                    QTY = x.Sum(y=> y.QTY * 1000)
                });

            return dbData.ToList();
        }

        public List<ZAAP_SHIFT_RPT> GetForCFVsFa(ZaapShiftRptGetForLack1ReportByParamInput input)
        {
            Expression<Func<ZAAP_SHIFT_RPT, bool>> queryFilter = PredicateHelper.True<ZAAP_SHIFT_RPT>();

            queryFilter = queryFilter.And(c => input.Werks.Distinct().Contains(c.WERKS));

            queryFilter = queryFilter.And(x => x.POSTING_DATE >= input.BeginingDate && x.POSTING_DATE <= input.EndDate);

            var data = _repository.Get(queryFilter);

            return data.ToList();
        }

        public ZAAP_SHIFT_RPT GetById(int? id)
        {
            var data = _repository.GetQuery(x => x.ZAAP_SHIFT_RPT_ID == id).FirstOrDefault();

            return data;
        }
    }
}
