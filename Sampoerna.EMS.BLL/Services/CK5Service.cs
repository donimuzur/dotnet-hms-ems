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
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class CK5Service : ICK5Service
    {
        private IGenericRepository<CK5> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public CK5Service(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK5>();

        }
        public List<CK5> GetForLack1ByParam(Ck5GetForLack1ByParamInput input)
        {
            //&& !string.IsNullOrEmpty(c.STO_RECEIVER_NUMBER)
            Expression<Func<CK5, bool>> queryFilterCk5 = c => c.DEST_PLANT_NPPBKC_ID == input.NppbkcId && c.DEST_PLANT_COMPANY_CODE == input.CompanyCode
                                             && (int)c.EX_GOODS_TYPE == input.ExGroupTypeId && c.SOURCE_PLANT_ID == input.SupplierPlantId
                                             && (c.GR_DATE.HasValue && c.GR_DATE.Value.Month == input.PeriodMonth && c.GR_DATE.Value.Year == input.PeriodYear)
                                             && c.STATUS_ID == Enums.DocumentStatus.Completed
                                             ;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilterCk5 = queryFilterCk5.And(c => c.DEST_PLANT_ID == input.ReceivedPlantId);
            }

            if (input.IsExcludeSameNppbkcId)
            {
                queryFilterCk5 = queryFilterCk5.And(c => c.SOURCE_PLANT_NPPBKC_ID != c.DEST_PLANT_NPPBKC_ID);
            }

            if (input.Pbck1DecreeIdList.Count > 0)
            {
                queryFilterCk5 =
                    queryFilterCk5.And(
                        c => c.PBCK1_DECREE_ID.HasValue && input.Pbck1DecreeIdList.Contains(c.PBCK1_DECREE_ID.Value));
            }

            return _repository.Get(queryFilterCk5).ToList();
        }

        public List<CK5> GetForLack2ByParam(Ck5GetForLack2ByParamInput input)
        {
            var data = _repository.Get(
                    p => p.SOURCE_PLANT_NPPBKC_ID == input.NppbkcId && p.SOURCE_PLANT_COMPANY_CODE == input.CompanyCode &&
                        p.GI_DATE.HasValue && p.GI_DATE.Value.Month == input.PeriodMonth && p.GI_DATE.Value.Year == input.PeriodYear &&
                        p.CK5_TYPE != Enums.CK5Type.Export && p.STATUS_ID == Enums.DocumentStatus.Completed &&
                        p.SOURCE_PLANT_ID == input.SourcePlantId && (int)p.EX_GOODS_TYPE == input.ExGroupTypeId).ToList();

            return data;

        }

    }
}
