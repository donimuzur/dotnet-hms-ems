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
        private IGenericRepository<T1001W> _repositoryPlantT001W;
        private IGenericRepository<ZAIDM_EX_SERIES> _repositorySeries;
      

        public BrandRegistrationBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryPlantT001W = _uow.GetGenericRepository<T1001W>();
            _repositorySeries = _uow.GetGenericRepository<ZAIDM_EX_SERIES>();

        }
        public List<BrandRegistrationOutput> GetAll()
        {
            //var repoBrand = _repository.GetQuery();
            //var repoPlant = _repositoryPlantT001W.GetQuery();
            //var repoSeries = _repositorySeries.GetQuery();

            var result = from b in _repository.GetQuery()
                          join p in _repositoryPlantT001W.GetQuery() on b.PLANT_ID equals p.PLANT_ID
                          join s in _repositorySeries.GetQuery() on b.SERIES_ID equals s.SERIES_ID
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

        public BrandRegistrationOutput save(ZAIDM_EX_BRAND brandRegistrasionExBrand)
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
                    output.BrandIdZaidmExBrand = brandRegistrasionExBrand.BRAND_ID;
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
