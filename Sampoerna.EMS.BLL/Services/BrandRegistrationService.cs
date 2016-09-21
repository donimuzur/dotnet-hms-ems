using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
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

        public List<ZAIDM_EX_BRAND> GetByFaCodeListAndPlantList(List<string> facodeList,List<string> plantList)
        {
            return _repository.Get(c => facodeList.Contains(c.FA_CODE) && plantList.Contains(c.WERKS), null, "ZAIDM_EX_PRODTYP").ToList();
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

        public ZAIDM_EX_BRAND GetByPlantIdAndStickerCode(string plantId, string stickerCode)
        {
            var dbData = _repository.Get(b => b.WERKS == plantId && b.STICKER_CODE == stickerCode, null, "ZAIDM_EX_PRODTYP, ZAIDM_EX_SERIES").FirstOrDefault();
            return dbData;
        }

        public List<ZAIDM_EX_BRAND> GetBrandByPlantAndListProdCode(string plant, List<string> prodCode )
        {
            var dbData = _repository.Get(c => c.WERKS == plant && prodCode.Contains(c.PROD_CODE));
            return dbData.ToList();
        }

        public List<ZAIDM_EX_BRAND> GetByPlantAndFaCode(List<string> plant, List<string> faCode)
        {
            Expression<Func<ZAIDM_EX_BRAND, bool>> queryFilter = PredicateHelper.True<ZAIDM_EX_BRAND>();

            var dbData = new List<ZAIDM_EX_BRAND>();

            if(plant.Count > 0 && faCode.Count > 0)
            {
                queryFilter = queryFilter.And(b => plant.Contains(b.WERKS) && faCode.Contains(b.FA_CODE));

                dbData = _repository.Get(queryFilter).ToList();
            }

            return dbData;
        }
    }
}
