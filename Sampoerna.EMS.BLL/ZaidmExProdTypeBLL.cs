using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExProdTypeBLL : IZaidmExProdTypeBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_PRODTYP> _repository;
        
        public ZaidmExProdTypeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
        }

        public ZAIDM_EX_PRODTYP GetById(string id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_PRODTYP> GetAll()
        {
            return _repository.Get().ToList();
        }
        public ZAIDM_EX_PRODTYP GetByCode(string Code)
        {
            return _repository.Get(p=>p.PROD_CODE == Code).OrderByDescending(x=>x.CREATED_DATE).FirstOrDefault();
        }

        public ZAIDM_EX_PRODTYP GetByAlias(string aliasName)
        {
            return _repository.Get(p => p.PRODUCT_ALIAS == aliasName).OrderByDescending(x => x.CREATED_DATE).FirstOrDefault();
        }

        public void UpdateProductType(ProductTypeSaveInput input)
        {
            var dbData = _repository.GetByID(input.ProductType.PROD_CODE);
            if (dbData == null) throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;
            dbData.CK4CEDITABLE = input.ProductType.CK4CEDITABLE;

            _uow.SaveChanges();

        }
    }
}
