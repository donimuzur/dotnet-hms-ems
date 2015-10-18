using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{

    public class BrandRegistrationService : IBrandRegistrationService
    {
        private IGenericRepository<ZAIDM_EX_BRAND> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public BrandRegistrationService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
        }

        public List<ZAIDM_EX_BRAND> GetByFaCodeList(List<string> input)
        {
            return _repository.Get(c => input.Contains(c.FA_CODE), null, "ZAIDM_EX_PRODTYP").ToList();
        }

        public ZAIDM_EX_BRAND GetByPlantIdAndFaCode(string plantId, string faCode)
        {
            var dbData = _repository.Get(b => b.WERKS == plantId && b.FA_CODE == faCode, null, "ZAIDM_EX_PRODTYP, ZAIDM_EX_SERIES").FirstOrDefault();
            return dbData;
        }

        public List<ZAIDM_EX_BRAND> GetBrandByPlant(string plant)
        {
            var dbData = _repository.Get(c => c.WERKS == plant);
            return dbData.ToList();
        }

        public ZAIDM_EX_GOODTYP GetGoodTypeByProdCodeInBrandRegistration(string prodCode)
        {
            const string incTables = "ZAIDM_EX_GOODTYP";
            var dbData = _repository.Get(c => c.PROD_CODE == prodCode, null, incTables).FirstOrDefault();
            if (dbData != null)
            {
                return dbData.ZAIDM_EX_GOODTYP;
            }
            return null;
        }
    }
}
