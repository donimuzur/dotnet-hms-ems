using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Pbck1ProdConverterService : IPbck1ProdConverterService
    {

        private IGenericRepository<PBCK1_PROD_CONVERTER> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public Pbck1ProdConverterService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1_PROD_CONVERTER>();
        }

        public List<PBCK1_PROD_CONVERTER> GetProductionLack1TisToTis(Pbck1GetProductionLack1TisToTisParamInput input)
        {
            Expression<Func<PBCK1_PROD_CONVERTER, bool>> queryFilter =
                c => c.PBCK1.NPPBKC_ID == input.NppbkcId && c.PBCK1.EXC_GOOD_TYP == input.ExcisableGoodsTypeId
                     && c.PBCK1.SUPPLIER_PLANT_WERKS == input.SupplierPlantId &&
                     c.PBCK1.SUPPLIER_NPPBKC_ID == input.SupplierPlantNppbkcId;

            queryFilter = queryFilter.And(c => c.PROD_CODE == "05");
            return _repository.Get(queryFilter, null, "PBCK1").ToList();
        }
    }
}
