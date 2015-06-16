using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class BrandRegistrationBLL : IBrandRegistrationBLL
    {
        private IGenericRepository<ZAIDM_EX_BRAND> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
      

        public BrandRegistrationBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            
        }
        public List<BrandRegistrationOutput> GetAll()
        {
            throw new NotImplementedException();
        }

        public BrandRegistrationOutput save(BusinessObject.ZAIDM_EX_BRAND brandRegistrasionExBrand)
        {
            {
                if (brandRegistrasionExBrand.BRAND_ID > 0)
                {
                    //update
                    _repository.Update(brandRegistrasionExBrand);
                }
                else
                {
                    //Insert
                    _repository.Insert(brandRegistrasionExBrand);
                }

                var output = new BrandRegistrationOutput();

                try
                {
                    _uow.SaveChanges();
                    output.Success = true;
                    output.bran = brandRegistrasionExBrand.BRAND_ID;
                }
                catch (Exception exception)
                {
                    _logger.Error(exception);
                    output.Success = false;
                    output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                    output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                }
                return output;
            }
        }
    }
}
