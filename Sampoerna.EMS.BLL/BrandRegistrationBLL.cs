using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class BrandRegistrationBLL : IBrandRegistrationBLL
    {
        private IGenericRepository<ZAIDM_EX_BRAND> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<T001W> _repositoryPlantT001W;
        private IGenericRepository<ZAIDM_EX_SERIES> _repositorySeries;
        // private IChangesHistoryBLL _changesHistoryBll;

        public BrandRegistrationBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryPlantT001W = _uow.GetGenericRepository<T001W>();
            _repositorySeries = _uow.GetGenericRepository<ZAIDM_EX_SERIES>();
            //_changesHistoryBll = changesHistoryBll;
        }

        public List<ZAIDM_EX_BRAND> GetAllBrands()
        {
            return _repository.Get(null, null, "T001W,ZAIDM_EX_SERIES").ToList();
        }

        public ZAIDM_EX_BRAND GetBrandByBrandCEAndProdCode(string brand, string prodCode)
        {
            var dbData = _repository.Get(c => c.BRAND_CE == brand && c.PROD_CODE == prodCode).FirstOrDefault();
 
            return dbData;
        }

        public ZAIDM_EX_BRAND GetById(string plant, string facode)
        {
            var dbData = _repository.GetByID(plant, facode);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return dbData;
        }

        public ZAIDM_EX_BRAND GetByIdIncludeChild(string plant, string facode)
        {
            var dbData = _repository.Get(a => a.WERKS == plant && a.FA_CODE == facode, null, "T001W , ZAIDM_EX_PRODTYP, ZAIDM_EX_SERIES, ZAIDM_EX_GOODTYP, ZAIDM_EX_MARKET").FirstOrDefault();
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData;
        }

        public List<BrandRegistrationOutput> GetAll()
        {
            //var repoBrand = _repository.GetQuery();
            //var repoPlant = _repositoryPlantT001W.GetQuery();
            //var repoSeries = _repositorySeries.GetQuery();

            var result = from b in _repository.GetQuery()
                         join p in _repositoryPlantT001W.GetQuery() on b.WERKS equals p.WERKS
                         join s in _repositorySeries.GetQuery() on b.SERIES_CODE equals s.SERIES_CODE
                         //where (b.IS_DELETED == false || b.IS_DELETED == null && b.STATUS == true)
                         select new BrandRegistrationOutput()
                         {
                             StickerCode = b.STICKER_CODE,
                             Name1 = p.NAME1,
                             FaCode = b.FA_CODE,
                             BrandCe = b.BRAND_CE,
                             SeriesValue = s.SERIES_VALUE,
                             PrintingPrice = b.PRINTING_PRICE,
                             CutFilterCode = b.CUT_FILLER_CODE

                         };

            return result.ToList();
        }

        public void Save(ZAIDM_EX_BRAND brandRegistration)
        {

            _repository.InsertOrUpdate(brandRegistration);
            _uow.SaveChanges();

        }

        public bool Delete(string plant, string facode)
        {
            var dbBrand = _repository.GetByID(plant, facode);
            if (dbBrand == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbBrand.IS_DELETED.HasValue && dbBrand.IS_DELETED.Value)
            {
                dbBrand.IS_DELETED = false;
            }
            else
            {
                dbBrand.IS_DELETED = true;
            }

            
            //_repository.Update(dbBrand);
            _uow.SaveChanges();

            return dbBrand.IS_DELETED.Value;
        }


        public ZAIDM_EX_BRAND GetByFaCode(string plantWerk, string faCode)
        {
            //var dbData = _repository.Get(b => b.FA_CODE.Equals(faCode)).FirstOrDefault();
            var dbData = _repository.Get(b => b.WERKS == plantWerk && b.FA_CODE == faCode && b.IS_DELETED !=true).FirstOrDefault();
            return dbData;
        }

        

        public List<ZAIDM_EX_BRAND> GetByPlantId(string plantId)
        {
            var dbData = _repository.Get(b => b.WERKS == plantId).ToList();
            //var dbData = _repository.Get(b => b.WERKS == plantId && b.STATUS == true && b.IS_DELETED != true).ToList();
            return dbData;
        }


        public List<ZAIDM_EX_BRAND> GetBrandCeBylant(string plantWerk)
        {
            //var dbData = _repository.Get(c => c.WERKS == plantWerk).ToList();
            var dbData = _repository.Get(b => b.WERKS == plantWerk && b.IS_DELETED != true && b.STATUS == true ).ToList();
            //var dbData = _repository.Get(b => b.WERKS == plantWerk && b.IS_DELETED != true && b.STATUS == true).ToList();
            return dbData;
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
