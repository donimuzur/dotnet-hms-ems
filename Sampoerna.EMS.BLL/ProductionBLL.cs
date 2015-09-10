using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ProductionBLL : IProductionBLL
    {
        private IGenericRepository<PRODUCTION> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_BRAND> _repositoryBrand;
        private IGenericRepository<ZAIDM_EX_PRODTYP> _repositoryProd;
        private IGenericRepository<UOM> _repositoryUom;
        private IGenericRepository<T001W> _repositoryPlant;

        public ProductionBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PRODUCTION>();
            _repositoryBrand = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryProd = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
            _repositoryUom = _uow.GetGenericRepository<UOM>();
            _repositoryPlant = _uow.GetGenericRepository<T001W>();
        }

        public List<ProductionDto> GetAllByParam(ProductionGetByParamInput input)
        {
            Expression<Func<PRODUCTION, bool>> queryFilter = PredicateHelper.True<PRODUCTION>();
            if (!string.IsNullOrEmpty(input.Company))
            {
                queryFilter = queryFilter.And(c => c.COMPANY_CODE == input.Company);
            }
            if (!string.IsNullOrEmpty(input.Plant))
            {
                queryFilter = queryFilter.And(c => c.WERKS == input.Plant);
            }
            if (!string.IsNullOrEmpty(input.ProoductionDate))
            {
                var dt = Convert.ToDateTime(input.ProoductionDate);
                queryFilter = queryFilter.And(c => c.PRODUCTION_DATE == dt);
            }

            Func<IQueryable<PRODUCTION>, IOrderedQueryable<PRODUCTION>> orderBy = null;
            {
                if (!string.IsNullOrEmpty(input.ShortOrderColumn))
                {
                    orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PRODUCTION>(input.ShortOrderColumn));
                }

                var dbData = _repository.Get(queryFilter, orderBy);
                if (dbData == null)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                }
                var mapResult = Mapper.Map<List<ProductionDto>>(dbData.ToList());

                return mapResult;

            }

        }

        public List<ProductionDto> GetAllProduction()
        {
            var dtData = _repository.Get().ToList();
            return Mapper.Map<List<ProductionDto>>(dtData);
        }

        public void Save(ProductionDto productionDto)
        {
            PRODUCTION dbProduction = new PRODUCTION();
            dbProduction = Mapper.Map<PRODUCTION>(productionDto);

            _repository.InsertOrUpdate(dbProduction);
            _uow.SaveChanges();
        }


        public ProductionDto GetById(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {

            var dbData = _repository.GetByID(companyCode, plantWerk, faCode, productionDate);
            var item = Mapper.Map<ProductionDto>(dbData);

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return item;
        }

        public List<ProductionDto> GetByCompPlant(string comp, string plant, string nppbkc, int period, int month, int year)
        {
            DateTime firstDay = new DateTime(year, month, 1);
            DateTime startDate = firstDay;
            DateTime endDate = new DateTime(year, month, 14);

            if(period == 2)
            {
                startDate = new DateTime(year, month, 15);
                endDate = firstDay.AddMonths(1).AddDays(-1);
            }

            var dbData = from p in _repository.Get(p => p.COMPANY_CODE == comp && p.WERKS == plant && (p.PRODUCTION_DATE >= startDate && p.PRODUCTION_DATE <= endDate))
                         join b in _repositoryBrand.GetQuery() on p.FA_CODE equals b.FA_CODE
                         join g in _repositoryProd.GetQuery() on b.PROD_CODE equals g.PROD_CODE
                         join u in _repositoryUom.GetQuery() on p.UOM equals u.UOM_ID
                         select new ProductionDto() { 
                            ProductionDate = p.PRODUCTION_DATE,
                            FaCode = p.FA_CODE,
                            PlantWerks = p.WERKS,
                            BrandDescription = p.BRAND_DESC,
                            PlantName = p.PLANT_NAME,
                            TobaccoProductType = g.PRODUCT_TYPE,
                            Hje = b.HJE_IDR,
                            Tarif = b.TARIFF,
                            QtyProduced = p.QTY_PACKED + p.QTY_UNPACKED,
                            Uom = u.UOM_DESC,
                            QtyPacked = p.QTY_PACKED,
                            QtyUnpacked = p.QTY_UNPACKED,
                            UomProd = p.UOM,
                            ProdCode = b.PROD_CODE
                         };

            if(nppbkc != string.Empty)
            {
                dbData = from p in _repository.Get(p => p.COMPANY_CODE == comp && (p.PRODUCTION_DATE >= startDate && p.PRODUCTION_DATE <= endDate))
                         join n in _repositoryPlant.Get(n => n.NPPBKC_ID == nppbkc) on p.WERKS equals n.WERKS
                         join b in _repositoryBrand.GetQuery() on p.FA_CODE equals b.FA_CODE
                         join g in _repositoryProd.GetQuery() on b.PROD_CODE equals g.PROD_CODE
                         join u in _repositoryUom.GetQuery() on p.UOM equals u.UOM_ID
                         select new ProductionDto()
                         {
                             ProductionDate = p.PRODUCTION_DATE,
                             FaCode = p.FA_CODE,
                             PlantWerks = p.WERKS,
                             BrandDescription = p.BRAND_DESC,
                             PlantName = p.PLANT_NAME,
                             TobaccoProductType = g.PRODUCT_TYPE,
                             Hje = b.HJE_IDR,
                             Tarif = b.TARIFF,
                             QtyProduced = p.QTY_PACKED + p.QTY_UNPACKED,
                             Uom = u.UOM_DESC,
                             QtyPacked = p.QTY_PACKED,
                             QtyUnpacked = p.QTY_UNPACKED,
                             UomProd = p.UOM,
                             ProdCode = b.PROD_CODE
                         };
            }

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return dbData.ToList();
        }


        public PRODUCTION GetExistDto(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {
            return
                _uow.GetGenericRepository<PRODUCTION>()
                    .Get(
                        p =>
                            p.COMPANY_CODE == companyCode && p.WERKS == plantWerk && p.FA_CODE == faCode &&
                            p.PRODUCTION_DATE == productionDate)
                    .FirstOrDefault();
        }
    }
}
