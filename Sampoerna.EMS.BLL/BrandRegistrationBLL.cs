using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

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

        public ZAIDM_EX_BRAND GetById(string plant, string facode)
        {
            var dbData = _repository.GetByID(plant, facode);
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData;
        }

        public ZAIDM_EX_BRAND GetByIdIncludeChild(string plant, string facode)
        {
            var dbData = _repository.Get(a => a.WERKS == plant && a.FA_CODE == facode, null, "T001W , ZAIDM_EX_PCODE, ZAIDM_EX_PRODTYP, ZAIDM_EX_SERIES, ZAIDM_EX_GOODTYP, ZAIDM_EX_MARKET").FirstOrDefault();
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


            try
            {
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.Error(exception);

            }
        }

        public void Delete(string plant, string facode)
        {
            var dbBrand = _repository.GetByID(plant, facode);
            if (dbBrand == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbBrand.IS_DELETED.HasValue && dbBrand.IS_DELETED.Value)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbBrand.IS_DELETED = true;


            _uow.SaveChanges();


        }


        public ZAIDM_EX_BRAND GetByFaCode(string faCode)
        {
            var dbData = _repository.Get(b => b.FA_CODE.Equals(faCode)).FirstOrDefault();
           
            return dbData;
        }


    }
}
