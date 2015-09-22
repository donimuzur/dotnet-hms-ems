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
        private ChangesHistoryBLL _changesHistoryBll;

        public ProductionBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PRODUCTION>();
            _repositoryBrand = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryProd = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
            _repositoryUom = _uow.GetGenericRepository<UOM>();
            _repositoryPlant = _uow.GetGenericRepository<T001W>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
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

        public void Save(ProductionDto productionDto, string userId)
        {
            var dbProduction = Mapper.Map<PRODUCTION>(productionDto);

            var origin = _repository.GetByID(dbProduction.COMPANY_CODE, dbProduction.WERKS, dbProduction.FA_CODE,
                dbProduction.PRODUCTION_DATE);

                //_repository.Get(
                //    x =>
                //        x.COMPANY_CODE == productionDto.CompanyCode && x.WERKS == productionDto.PlantWerks &&
                //        x.FA_CODE == productionDto.FaCode && x.PRODUCTION_DATE == productionDto.ProductionDate);
         
            SetChange(origin, productionDto, userId);
            dbProduction.CREATED_DATE = DateTime.Now;

            if (dbProduction.UOM == "KG")
            {
                dbProduction.UOM = "G";
                dbProduction.QTY_PACKED = dbProduction.QTY_PACKED * 1000;
                dbProduction.QTY_UNPACKED = dbProduction.QTY_UNPACKED * 1000;
            }

            if (dbProduction.UOM == "TH")
            {
                dbProduction.UOM = "Btg";
                dbProduction.QTY_PACKED = dbProduction.QTY_PACKED * 1000;
                dbProduction.QTY_UNPACKED = dbProduction.QTY_UNPACKED * 1000;
            }

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

            if (period == 2)
            {
                startDate = new DateTime(year, month, 15);
                endDate = firstDay.AddMonths(1).AddDays(-1);
            }

            var dbData = from p in _repository.Get(p => p.COMPANY_CODE == comp && p.WERKS == plant && (p.PRODUCTION_DATE >= startDate && p.PRODUCTION_DATE <= endDate))
                         join b in _repositoryBrand.Get(b => b.STATUS == true && (b.IS_DELETED == null || b.IS_DELETED == false)) on new { p.FA_CODE, p.WERKS } equals new { b.FA_CODE, b.WERKS }
                         join g in _repositoryProd.GetQuery() on b.PROD_CODE equals g.PROD_CODE
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
                             QtyProduced = p.QTY == null ? 0 : p.QTY,
                             Uom = p.UOM,
                             QtyPacked = p.QTY_PACKED,
                             QtyUnpacked = p.QTY_UNPACKED,
                             ProdCode = b.PROD_CODE
                         };

            if (nppbkc != string.Empty)
            {
                dbData = from p in _repository.Get(p => p.COMPANY_CODE == comp && (p.PRODUCTION_DATE >= startDate && p.PRODUCTION_DATE <= endDate))
                         join n in _repositoryPlant.Get(n => n.NPPBKC_ID == nppbkc) on p.WERKS equals n.WERKS
                         join b in _repositoryBrand.Get(b => b.STATUS == true && (b.IS_DELETED == null || b.IS_DELETED == false)) on new { p.FA_CODE, p.WERKS } equals new { b.FA_CODE, b.WERKS }
                         join g in _repositoryProd.GetQuery() on b.PROD_CODE equals g.PROD_CODE
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
                             QtyProduced = p.QTY == null ? 0 : p.QTY,
                             Uom = p.UOM,
                             QtyPacked = p.QTY_PACKED,
                             QtyUnpacked = p.QTY_UNPACKED,
                             ProdCode = b.PROD_CODE
                         };
            }

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return dbData.OrderByDescending(x => x.ProductionDate).ToList();
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


        public void SaveUpload(ProductionUploadItems uploadItems, string userId)
        {
            var dbUpload = Mapper.Map<PRODUCTION>(uploadItems);

            _repository.InsertOrUpdate(dbUpload);
            dbUpload.CREATED_DATE = DateTime.Now;
            _uow.SaveChanges();
        }

        private void SetChange(PRODUCTION origin, ProductionDto data, string userId)
        {
            var changeData = new Dictionary<string, bool>();
            changeData.Add("COMPANY_CODE", origin.COMPANY_CODE == data.CompanyCode);
            changeData.Add("WERKS", origin.WERKS == data.PlantWerks);
            changeData.Add("FA_CODE", origin.FA_CODE == data.FaCode);
            changeData.Add("PRODUCTION_DATE", origin.PRODUCTION_DATE == data.ProductionDate);
            changeData.Add("BRAND_DESC", origin.BRAND_DESC == data.BrandDescription);
            changeData.Add("QTY_PACKED", origin.QTY_PACKED == data.QtyPacked);
            changeData.Add("QTY_UNPACKED", origin.QTY_UNPACKED == data.QtyUnpacked);
            changeData.Add("UOM", origin.UOM == data.Uom);

            foreach (var listChange in changeData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.CK4C,
                        FORM_ID = data.CompanyCode+"_" + data.PlantWerks+"_" + data.FaCode+"_" + data.ProductionDate.ToString("ddMMMyyyy"),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };

                    switch (listChange.Key)
                    {
                        case "COMPANY_CODE":
                            changes.OLD_VALUE = origin.COMPANY_CODE;
                            changes.NEW_VALUE = data.CompanyCode;
                            break;
                        case "WERKS":
                            changes.OLD_VALUE = origin.WERKS;
                            changes.NEW_VALUE = data.PlantWerks;
                            break;
                        case "FA_CODE":
                            changes.OLD_VALUE = origin.FA_CODE;
                            changes.NEW_VALUE = data.FaCode;
                            break;
                        case "PRODUCTION_DATE":
                            changes.OLD_VALUE = origin.PRODUCTION_DATE.ToString();
                            changes.NEW_VALUE = data.ProductionDate.ToString();
                            break;
                        case "BRAND_DESC":
                            changes.OLD_VALUE = origin.BRAND_DESC;
                            changes.NEW_VALUE = data.BrandDescription;
                            break;
                        case "QTY_PACKED":
                            changes.OLD_VALUE = origin.QTY_PACKED.ToString();
                            changes.NEW_VALUE = data.QtyPacked.ToString();
                            break;
                        case "QTY_UNPACKED":
                            changes.OLD_VALUE = origin.QTY_UNPACKED.ToString();
                            changes.NEW_VALUE = data.QtyUnpacked.ToString();
                            break;
                        case "UOM":
                            changes.OLD_VALUE = origin.UOM;
                            changes.NEW_VALUE = data.Uom;
                            break;
                        default: break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }

        }

    }
}
