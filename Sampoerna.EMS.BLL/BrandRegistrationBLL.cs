using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.LinqExtensions;
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
        private IGenericRepository<ZAIDM_EX_MARKET> _repositoryMarket;

        private IZaidmExProdTypeBLL _zaidmExProdTypeBLL;
        private IMasterDataAprovalBLL _masterDataAprovalBLL;
        private IExcisableGoodsTypeService _excisableGoodsTypeService;
        private IPlantBLL _plantBll;
        
        // private IChangesHistoryBLL _changesHistoryBll;

        public BrandRegistrationBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryPlantT001W = _uow.GetGenericRepository<T001W>();
            _repositorySeries = _uow.GetGenericRepository<ZAIDM_EX_SERIES>();
            _repositoryMarket = _uow.GetGenericRepository<ZAIDM_EX_MARKET>();
            _plantBll = new PlantBLL(_uow, _logger);
            _masterDataAprovalBLL = new MasterDataApprovalBLL(_uow,_logger);
            _zaidmExProdTypeBLL = new ZaidmExProdTypeBLL(_uow,_logger);
            _excisableGoodsTypeService = new ExcisableGoodsTypeService(_uow,_logger);
            //_changesHistoryBll = changesHistoryBll;
        }

        public List<ZAIDM_EX_BRAND> GetAllBrands()
        {
            return _repository.Get(null, null, "T001W,ZAIDM_EX_SERIES,ZAIDM_EX_GOODTYP, ZAIDM_EX_PRODTYP, ZAIDM_EX_MARKET").ToList();
            //ZAIDM_EX_GOODTYP, ZAIDM_EX_PRODTYP, ZAIDM_EX_MARKET
        }


        public List<ZAIDM_EX_BRAND> GetAllBrandsOnly()
        {
            return _repository.Get().ToList();
        }


        public ZAIDM_EX_BRAND GetBrandForProdConv(string brand, string prodCode, string nppbkc)
        {
            var plant = _plantBll.GetPlantByNppbkc(nppbkc).Select(s => s.WERKS).ToList();

            var dbData = _repository.Get(c => c.BRAND_CE == brand && c.PROD_CODE == prodCode && plant.Contains(c.WERKS)).FirstOrDefault();

            return dbData;
        }

        public ZAIDM_EX_BRAND GetById(string plant, string facode)
        {
            var dbData = _repository.Get(a => a.WERKS == plant && a.FA_CODE == facode, null, "").FirstOrDefault();
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return dbData;
        }

        public ZAIDM_EX_BRAND GetById(string plant, string facode, string stickercode)
        {
            //var dbData = _repository.GetByID(plant, facode, stickercode);
            var dbData = _repository.Get(c => c.WERKS == plant && c.FA_CODE == facode && c.STICKER_CODE == stickercode, null, "ZAIDM_EX_GOODTYP, ZAIDM_EX_PRODTYP, ZAIDM_EX_SERIES, ZAIDM_EX_MARKET").FirstOrDefault();
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

        public bool Delete(string plant, string facode, string stickercode,string userId)
        {
            var dbBrand = _repository.GetByID(plant, facode, stickercode);
            if (dbBrand == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var oldDbBrand = Mapper.Map<BrandXmlDto>(dbBrand);
            var oldObject = Mapper.Map<ZAIDM_EX_BRAND>(oldDbBrand);

            if (dbBrand.IS_DELETED.HasValue && dbBrand.IS_DELETED.Value)
            {
                dbBrand.IS_DELETED = false;
            }
            else
            {
                dbBrand.IS_DELETED = true;
            }

            dbBrand = _masterDataAprovalBLL.MasterDataApprovalValidation((int) Core.Enums.MenuList.BrandRegistration, userId,
                oldObject, dbBrand);
            _repository.Update(dbBrand);
            _uow.SaveChanges();

            return dbBrand.IS_DELETED.HasValue && dbBrand.IS_DELETED.Value;
        }


        public ZAIDM_EX_BRAND GetByFaCode(string plantWerk, string faCode)
        {
            //var dbData = _repository.Get(b => b.FA_CODE.Equals(faCode)).FirstOrDefault();
            var dbData = _repository.Get(b => b.WERKS == plantWerk && b.FA_CODE == faCode && b.IS_DELETED != true).FirstOrDefault();
            return dbData;
        }



        public List<ZAIDM_EX_BRAND> GetByPlantId(string plantId)
        {
            const string incTables = "ZAIDM_EX_PRODTYP";

            var dbData = _repository.Get(b => b.WERKS == plantId && b.IS_DELETED != true && b.STATUS == true, null, incTables).ToList();

            return dbData;
        }


        public List<ZAIDM_EX_BRAND> GetBrandCeBylant(string plantWerk)
        {
            //var dbData = _repository.Get(c => c.WERKS == plantWerk).ToList();
            var dbData = _repository.Get(b => b.WERKS == plantWerk && b.IS_DELETED != true && b.STATUS == true).ToList();
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

        public ZAIDM_EX_BRAND GetBrandCe(string plant, string facode,string brandCe)
        {
            var dbData =
                _repository.Get(
                    x =>
                        x.WERKS == plant && x.FA_CODE == facode && x.BRAND_CE == brandCe).FirstOrDefault();

            return dbData;
        }

        public ZAIDM_EX_MARKET GetMarket(string marketId)
        {
            return _repositoryMarket.GetByID(marketId);
        }

        public ZAIDM_EX_SERIES GetSeries(string seriesCode)
        {
            return _repositorySeries.GetByID(seriesCode);
        }

        public BrandXmlDto GetDataForXml(string werks, string facode, string stickerCode)
        {
            var brandToxml = GetById(werks, facode, stickerCode);
            if (brandToxml != null)
            {
                if (brandToxml.IS_FROM_SAP == true)
                {
                    var brandXmlDto = Mapper.Map<BrandXmlDto>(brandToxml);

                    var prodType = _zaidmExProdTypeBLL.GetByCode(brandXmlDto.PROD_CODE);
                    var series = GetSeries(brandXmlDto.SERIES_CODE);
                    var market = GetMarket(brandXmlDto.MARKET_ID);
                    var goodType = _excisableGoodsTypeService.GetById(brandXmlDto.EXC_GOOD_TYP);

                    brandXmlDto.EXC_TYP_DESC = goodType.EXT_TYP_DESC;
                    brandXmlDto.MARKET_DESC = market.MARKET_DESC;
                    brandXmlDto.SERIES_VALUE = series.SERIES_VALUE.HasValue ? series.SERIES_VALUE.Value : 0;
                    brandXmlDto.PRODUCT_ALIAS = prodType.PRODUCT_ALIAS;
                    brandXmlDto.PRODUCT_TYPE = prodType.PRODUCT_TYPE;

                    return brandXmlDto;
                }

                return null;
            }
            return null;
        }


    }
}
