using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class PBCK1Service : IPBCK1Service
    {
        private IGenericRepository<PBCK1> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public PBCK1Service(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1>();
        }

        public List<PBCK1> GetForLack1ByParam(Pbck1GetDataForLack1ParamInput input)
        {
            return _repository.Get(c => c.NPPBKC_ID == input.NppbkcId 
                && c.NPPBKC_BUKRS == input.CompanyCode && c.EXC_GOOD_TYP == input.ExcisableGoodsTypeId 
                && c.SUPPLIER_PLANT_WERKS == input.SupplierPlantId && c.STATUS == Enums.DocumentStatus.Completed && (c.PERIOD_FROM.Value.Month <= input.PeriodMonth && c.PERIOD_TO.Value.Month >= input.PeriodMonth && c.PERIOD_FROM.Value.Year == input.PeriodYear), null, "").ToList();
        }
    }
}
