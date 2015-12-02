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
    public class CK5MaterialService : ICK5MaterialService
    {

        private IGenericRepository<CK5_MATERIAL> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "CK5";

        public CK5MaterialService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK5_MATERIAL>();
        }

        public List<CK5_MATERIAL> GetForLack1ByParam(Ck5MaterialGetForLackByParamInput input)
        {
            //&& !string.IsNullOrEmpty(c.STO_RECEIVER_NUMBER)
            Expression<Func<CK5_MATERIAL, bool>> queryFilterCk5 = c => c.CK5.SOURCE_PLANT_NPPBKC_ID == input.NppbkcId && c.CK5.SOURCE_PLANT_COMPANY_CODE == input.CompanyCode
                                             && (int)c.CK5.EX_GOODS_TYPE == input.ExGroupTypeId && c.CK5.SOURCE_PLANT_ID == input.SupplierPlantId
                                             && (c.CK5.GR_DATE.HasValue && c.CK5.GR_DATE.Value.Month == input.PeriodMonth && c.CK5.GR_DATE.Value.Year == input.PeriodYear)
                                             && c.CK5.STATUS_ID >= Enums.DocumentStatus.Completed
                                             ;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilterCk5 = queryFilterCk5.And(c => c.CK5.DEST_PLANT_ID == input.ReceivedPlantId);
                
            }

            if (input.IsExcludeSameNppbkcId)
            {
                queryFilterCk5 = queryFilterCk5.And(c => c.CK5.SOURCE_PLANT_ID != c.CK5.DEST_PLANT_NPPBKC_ID);
            }

            return _repository.Get(queryFilterCk5).ToList();
        }
    }
}
