using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.Contract.Services;

namespace Sampoerna.EMS.BLL.Services
{
    public class CK4CItemService : ICK4CItemService
    {
        private IGenericRepository<CK4C_ITEM> _ck4CItemRepository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public CK4CItemService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _ck4CItemRepository = _uow.GetGenericRepository<CK4C_ITEM>();
        }

        public List<CK4C_ITEM> GetByParam(CK4CItemGetByParamInput input)
        {
            Expression<Func<CK4C_ITEM, bool>> queryFilterCk4C = PredicateHelper.True<CK4C_ITEM>();

            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilterCk4C = queryFilterCk4C.And(c => c.CK4C.COMPANY_ID == input.CompanyCode);
            }

            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilterCk4C = queryFilterCk4C.And(c => c.CK4C.NPPBKC_ID == input.NppbkcId);
            }

            if (input.PeriodMonth.HasValue && input.PeriodYear.HasValue)
            {
                queryFilterCk4C =
                    queryFilterCk4C.And(
                        c => c.CK4C.REPORTED_MONTH == input.PeriodMonth && c.CK4C.REPORTED_YEAR == input.PeriodYear);
            }

            if (input.Lack1Level == Core.Enums.Lack1Level.Plant)
            {
                queryFilterCk4C = queryFilterCk4C.And(c => c.CK4C.PLANT_ID == input.ReceivedPlantId);
            }

            if (input.IsHigherFromApproved)
            {
                queryFilterCk4C =
                    queryFilterCk4C.And(c => (int) c.CK4C.STATUS >= (int) Core.Enums.DocumentStatus.Approved);
            }

            return _ck4CItemRepository.Get(queryFilterCk4C, null, "CK4C, UOM").ToList();
        }

        public List<string> GetFaCodeListByParam(CK4CItemGetByParamInput input)
        {
            Expression<Func<CK4C_ITEM, bool>> queryFilterCk4C = PredicateHelper.True<CK4C_ITEM>();

            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilterCk4C = queryFilterCk4C.And(c => c.CK4C.COMPANY_ID == input.CompanyCode);
            }

            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilterCk4C = queryFilterCk4C.And(c => c.CK4C.NPPBKC_ID == input.NppbkcId);
            }

            if (input.PeriodMonth.HasValue && input.PeriodYear.HasValue)
            {
                queryFilterCk4C =
                    queryFilterCk4C.And(
                        c => c.CK4C.REPORTED_MONTH == input.PeriodMonth && c.CK4C.REPORTED_YEAR == input.PeriodYear);
            }

            if (input.Lack1Level == Core.Enums.Lack1Level.Plant)
            {
                queryFilterCk4C = queryFilterCk4C.And(c => c.CK4C.PLANT_ID == input.ReceivedPlantId);
            }

            if (input.IsHigherFromApproved)
            {
                queryFilterCk4C =
                    queryFilterCk4C.And(c => (int)c.CK4C.STATUS >= (int)Core.Enums.DocumentStatus.Approved);
            }

            var ck4CItemData = _ck4CItemRepository.Get(queryFilterCk4C, null, "CK4C").ToList();
            
            return  ck4CItemData.Select(c => c.FA_CODE).Distinct().ToList();

        }
        
    }
}
