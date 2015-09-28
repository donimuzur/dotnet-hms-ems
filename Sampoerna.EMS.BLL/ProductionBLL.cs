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
        private IGenericRepository<T001> _repositoryCompany;
        private ICompanyBLL _companyBll;
        private IPlantBLL _plantBll;
        private IBrandRegistrationBLL _brandRegistrationBll;

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
            _companyBll = new CompanyBLL(_uow, _logger);
            _plantBll = new PlantBLL(_uow, _logger);
            _brandRegistrationBll = new BrandRegistrationBLL(_uow, _logger);
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

            var originDto = Mapper.Map<ProductionDto>(origin);

            //to do ask and to do refactor
            //SetChange(originDto, productionDto, userId);

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
            dbProduction.CREATED_DATE = DateTime.Now;

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
                             QtyPacked = p.QTY_PACKED == null ? 0 : p.QTY_PACKED,
                             QtyUnpacked = p.QTY_UNPACKED == null ? 0 : p.QTY_UNPACKED,
                             QtyProduced = p.QTY == null ? p.QTY_PACKED + p.QTY_UNPACKED : p.QTY,
                             Uom = p.UOM,
                             ProdCode = b.PROD_CODE,
                             ContentPerPack = Convert.ToInt32(b.BRAND_CONTENT),
                             PackedInPack = Convert.ToInt32(p.QTY_PACKED) / Convert.ToInt32(b.BRAND_CONTENT)
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
                             QtyPacked = p.QTY_PACKED == null ? 0 : p.QTY_PACKED,
                             QtyUnpacked = p.QTY_UNPACKED == null ? 0 : p.QTY_UNPACKED,
                             QtyProduced = p.QTY == null ? p.QTY_PACKED + p.QTY_UNPACKED : p.QTY,
                             Uom = p.UOM,
                             ProdCode = b.PROD_CODE,
                             ContentPerPack = Convert.ToInt32(b.BRAND_CONTENT),
                             PackedInPack = Convert.ToInt32(p.QTY_PACKED) / Convert.ToInt32(b.BRAND_CONTENT)
                         };
            }

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return dbData.OrderBy(x => x.ProductionDate).ToList();
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

            _uow.SaveChanges();
        }

        private List<ProductionUploadItems> ValidateProductionUpload(List<ProductionUploadItems> input)
        {
            var messageList = new List<string>();
            var outputList = new List<ProductionUploadItems>();

            foreach (var productionUploadItems in input)
            {
                messageList.Clear();

                var output = Mapper.Map<ProductionUploadItems>(productionUploadItems);

                var dbCompany = _companyBll.GetById(productionUploadItems.CompanyCode);
                if (dbCompany == null)
                    messageList.Add("Company Code is Not valid");

                var dbPlant = _plantBll.GetId(productionUploadItems.PlantWerks);
                if (dbPlant == null)
                    messageList.Add("Plant Id is not valid");

                var dbBrand = _brandRegistrationBll.GetById(productionUploadItems.PlantWerks, productionUploadItems.FaCode);
                if (dbBrand == null)
                    messageList.Add("Fa Code is not Register");

                if (string.IsNullOrEmpty(productionUploadItems.ProductionDate))
                    messageList.Add("Daily Production Date is not valid");

                var dbproduction = GetExistDto(productionUploadItems.CompanyCode, productionUploadItems.PlantWerks,
                    productionUploadItems.FaCode, Convert.ToDateTime(productionUploadItems.ProductionDate));
                if (dbproduction == null)
                    messageList.Add("Production data all ready Exist");

                if (messageList.Count > 0)
                {
                    output.IsValid = false;
                    output.Message = " ";
                    foreach (var message in messageList)
                    {
                        output.Message += message + ";";
                    }

                }
                     
                else
                {
                    output.IsValid = true;

                }
                outputList.Add(output);
            }
            return outputList;
        }

        private void SetChange(ProductionDto origin, ProductionDto data, string userId)
        {
            var changeData = new Dictionary<string, bool>();
            changeData.Add("COMPANY_CODE", origin.CompanyCode == data.CompanyCode);
            changeData.Add("WERKS", origin.PlantWerks == data.PlantWerks);
            changeData.Add("FA_CODE", origin.FaCode == data.FaCode);
            changeData.Add("PRODUCTION_DATE", origin.ProductionDate == data.ProductionDate);
            changeData.Add("BRAND_DESC", origin.BrandDescription == data.BrandDescription);
            changeData.Add("QTY_PACKED", origin.QtyPacked == data.QtyPacked);
            changeData.Add("QTY_UNPACKED", origin.QtyUnpacked == data.QtyUnpacked);
            changeData.Add("UOM", origin.Uom == data.Uom);

            foreach (var listChange in changeData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.CK4C,
                        FORM_ID = data.CompanyCode + "_" + data.PlantWerks + "_" + data.FaCode + "_" + data.ProductionDate.ToString("ddMMMyyyy"),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };

                    switch (listChange.Key)
                    {
                        case "COMPANY_CODE":
                            changes.OLD_VALUE = origin.CompanyCode;
                            changes.NEW_VALUE = data.CompanyCode;
                            break;
                        case "WERKS":
                            changes.OLD_VALUE = origin.PlantWerks;
                            changes.NEW_VALUE = data.PlantWerks;
                            break;
                        case "FA_CODE":
                            changes.OLD_VALUE = origin.FaCode;
                            changes.NEW_VALUE = data.FaCode;
                            break;
                        case "PRODUCTION_DATE":
                            changes.OLD_VALUE = origin.ProductionDate.ToString();
                            changes.NEW_VALUE = data.ProductionDate.ToString();
                            break;
                        case "BRAND_DESC":
                            changes.OLD_VALUE = origin.BrandDescription;
                            changes.NEW_VALUE = data.BrandDescription;
                            break;
                        case "QTY_PACKED":
                            changes.OLD_VALUE = origin.QtyPacked.ToString();
                            changes.NEW_VALUE = data.QtyPacked.ToString();
                            break;
                        case "QTY_UNPACKED":
                            changes.OLD_VALUE = origin.QtyUnpacked.ToString();
                            changes.NEW_VALUE = data.QtyUnpacked.ToString();
                            break;
                        case "UOM":
                            changes.OLD_VALUE = origin.Uom;
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
