using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
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
        private IWasteBLL _wasteBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;

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
            _wasteBll = new WasteBLL(_logger, _uow);
            _uomBll = new UnitOfMeasurementBLL(uow, _logger);
            _userPlantBll = new UserPlantMapBLL(_uow, _logger);
            _poaMapBll = new POAMapBLL(_uow, _logger);
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
            if (!string.IsNullOrEmpty(input.UserId))
            {
                var listUserPlant = _userPlantBll.GetPlantByUserId(input.UserId);

                var listPoaPlant = _poaMapBll.GetPlantByPoaId(input.UserId);

                queryFilter = queryFilter.And(c => listUserPlant.Contains(c.WERKS) || listPoaPlant.Contains(c.WERKS));
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

        public SaveProductionOutput Save(ProductionDto productionDto, string userId)
        {
            var output = new SaveProductionOutput();
            output.isNewData = true;
            output.isFromSap = false;

            #region ----- get description code--------
            var company = _companyBll.GetById(productionDto.CompanyCode);
            var plant = _plantBll.GetT001WById(productionDto.PlantWerks);
            var brandDesc = _brandRegistrationBll.GetById(productionDto.PlantWerks, productionDto.FaCode);

            productionDto.CompanyName = company.BUTXT;
            productionDto.PlantName = plant.NAME1;
            productionDto.BrandDescription = brandDesc.BRAND_CE;
            #endregion

            var dbProduction = Mapper.Map<PRODUCTION>(productionDto);
            dbProduction.QTY_PACKED = productionDto.QtyPackedStr == null ? 0 : Convert.ToDecimal(productionDto.QtyPackedStr);
            dbProduction.QTY = productionDto.QtyStr == null ? 0 : Convert.ToDecimal(productionDto.QtyStr);
            dbProduction.PROD_QTY_STICK = productionDto.ProdQtyStickStr == null ? 0 : Convert.ToDecimal(productionDto.ProdQtyStickStr);

            var origin = _repository.GetByID(productionDto.CompanyCodeX, productionDto.PlantWerksX, productionDto.FaCodeX,
               Convert.ToDateTime(productionDto.ProductionDateX));



            var originDto = Mapper.Map<ProductionDto>(origin);

            //to do ask and to do refactor
            if (originDto != null)
            {

                SetChange(originDto, productionDto, userId);
                output.isNewData = false;
            }

            if (dbProduction.UOM == "TH")
            {
                dbProduction.UOM = "Btg";
                dbProduction.QTY_PACKED = dbProduction.QTY_PACKED * 1000;
                dbProduction.QTY = dbProduction.QTY * 1000;
            }

            dbProduction.CREATED_DATE = DateTime.Now;
            dbProduction.CREATED_BY = userId;


            if (origin != null)
            {
                dbProduction.CREATED_DATE = origin.CREATED_DATE;
                dbProduction.CREATED_BY = origin.CREATED_BY;

                if (dbProduction.COMPANY_CODE != origin.COMPANY_CODE || dbProduction.WERKS != origin.WERKS ||
                  dbProduction.FA_CODE != origin.FA_CODE
                  || Convert.ToDateTime(dbProduction.PRODUCTION_DATE) != Convert.ToDateTime(origin.PRODUCTION_DATE))
                {
                    dbProduction.BATCH = null;
                }

                if (origin.BATCH != null)
                    output.isFromSap = true;
            }



            _repository.InsertOrUpdate(dbProduction);
            _uow.SaveChanges();

            return output;
        }


        public ProductionDto GetById(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {

            var dbData = _repository.GetByID(companyCode, plantWerk, faCode, productionDate);
            var item = Mapper.Map<ProductionDto>(dbData);
            item.QtyPackedStr = item.QtyPacked == null ? string.Empty : item.QtyPacked.ToString();
            item.QtyStr = item.Qty == null ? string.Empty : item.Qty.ToString();
            item.ProdQtyStickStr = item.ProdQtyStick == null ? string.Empty : item.ProdQtyStick.ToString();

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return item;
        }

        public List<ProductionDto> GetByCompPlant(string comp, string plant, string nppbkc, int period, int month, int year, bool isNppbkc)
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
                         join t in _repositoryPlant.GetQuery() on p.WERKS equals t.WERKS
                         select new ProductionDto()
                         {
                             CompanyCode = p.COMPANY_CODE,
                             ProductionDate = p.PRODUCTION_DATE,
                             FaCode = p.FA_CODE,
                             PlantWerks = p.WERKS,
                             BrandDescription = p.BRAND_DESC,
                             PlantName = t.NAME1,
                             TobaccoProductType = g.PRODUCT_TYPE,
                             Hje = b.HJE_IDR,
                             Tarif = b.TARIFF,
                             QtyPacked = p.QTY_PACKED == null ? 0 : p.QTY_PACKED,
                             QtyUnpacked = 0,
                             QtyProduced = p.QTY == null ? 0 : p.QTY,
                             Uom = p.UOM,
                             ProdCode = b.PROD_CODE,
                             ContentPerPack = Convert.ToInt32(b.BRAND_CONTENT),
                             PackedInPack = Convert.ToInt32(p.QTY_PACKED) / Convert.ToInt32(b.BRAND_CONTENT)
                         };

            if (nppbkc != string.Empty && isNppbkc)
            {
                dbData = from p in _repository.Get(p => p.COMPANY_CODE == comp && (p.PRODUCTION_DATE >= startDate && p.PRODUCTION_DATE <= endDate))
                         join n in _repositoryPlant.Get(n => n.NPPBKC_ID == nppbkc) on p.WERKS equals n.WERKS
                         join b in _repositoryBrand.Get(b => b.STATUS == true && (b.IS_DELETED == null || b.IS_DELETED == false)) on new { p.FA_CODE, p.WERKS } equals new { b.FA_CODE, b.WERKS }
                         join g in _repositoryProd.GetQuery() on b.PROD_CODE equals g.PROD_CODE
                         join t in _repositoryPlant.GetQuery() on p.WERKS equals t.WERKS
                         select new ProductionDto()
                         {
                             CompanyCode = p.COMPANY_CODE,
                             ProductionDate = p.PRODUCTION_DATE,
                             FaCode = p.FA_CODE,
                             PlantWerks = p.WERKS,
                             BrandDescription = p.BRAND_DESC,
                             PlantName = t.NAME1,
                             TobaccoProductType = g.PRODUCT_TYPE,
                             Hje = b.HJE_IDR,
                             Tarif = b.TARIFF,
                             QtyPacked = p.QTY_PACKED == null ? 0 : p.QTY_PACKED,
                             QtyUnpacked = 0,
                             QtyProduced = p.QTY == null ? 0 : p.QTY,
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


        private void SetChange(ProductionDto origin, ProductionDto data, string userId)
        {
            var changeData = new Dictionary<string, bool>();
            changeData.Add("COMPANY_CODE", origin.CompanyCode == data.CompanyCode);
            changeData.Add("WERKS", origin.PlantWerks == data.PlantWerks);
            changeData.Add("FA_CODE", origin.FaCode == data.FaCode);
            changeData.Add("PRODUCTION_DATE", origin.ProductionDate == data.ProductionDate);
            changeData.Add("BRAND_DESC", origin.BrandDescription == data.BrandDescription);
            changeData.Add("QTY_PACKED", origin.QtyPacked == data.QtyPacked);
            changeData.Add("QTY", origin.Qty == data.Qty);
            changeData.Add("UOM", origin.Uom == data.Uom);
            changeData.Add("PROD_QTY_STICK", origin.ProdQtyStick == data.ProdQtyStick);

            string isFromSapString = string.IsNullOrEmpty(origin.Batch) ? "" : "[FROM SAP]";

            foreach (var listChange in changeData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.CK4C,
                        FORM_ID = "Daily_" + data.CompanyCode + "_" + data.PlantWerks + "_" + data.FaCode + "_" + data.ProductionDate.ToString("ddMMMyyyy"),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };

                    switch (listChange.Key)
                    {
                        case "COMPANY_CODE":
                            changes.OLD_VALUE = origin.CompanyCode;
                            changes.NEW_VALUE = data.CompanyCode;
                            changes.FIELD_NAME = "Company" + isFromSapString;
                            break;
                        case "WERKS":
                            changes.OLD_VALUE = origin.PlantWerks;
                            changes.NEW_VALUE = data.PlantWerks;
                            changes.FIELD_NAME = "Plant";
                            break;
                        case "FA_CODE":
                            changes.OLD_VALUE = origin.FaCode;
                            changes.NEW_VALUE = data.FaCode;
                            changes.FIELD_NAME = "Finish Goods";
                            break;
                        case "PRODUCTION_DATE":
                            changes.OLD_VALUE = origin.ProductionDate.ToString();
                            changes.NEW_VALUE = data.ProductionDate.ToString();
                            changes.FIELD_NAME = "Daily Production Date";
                            break;
                        case "BRAND_DESC":
                            changes.OLD_VALUE = origin.BrandDescription;
                            changes.NEW_VALUE = data.BrandDescription;
                            changes.FIELD_NAME = "Brand Description";
                            break;
                        case "QTY_PACKED":
                            changes.OLD_VALUE = origin.QtyPacked.ToString();
                            changes.NEW_VALUE = data.QtyPacked.ToString();
                            changes.FIELD_NAME = "Qty Packed" + isFromSapString;
                            break;
                        case "QTY":
                            changes.OLD_VALUE = origin.Qty.ToString();
                            changes.NEW_VALUE = data.Qty.ToString();
                            changes.FIELD_NAME = "Produced Qty" + isFromSapString;
                            break;
                        case "UOM":
                            changes.OLD_VALUE = origin.Uom;
                            changes.NEW_VALUE = data.Uom;
                            changes.FIELD_NAME = "Uom";
                            break;
                        default: break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }

        }

        public void DeleteOldData(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {
            var dbData = _repository.GetByID(companyCode, plantWerk, faCode, productionDate);

            if (dbData == null)
            {
                _logger.Error(new BLLException(ExceptionCodes.BLLExceptions.DataNotFound));
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            else
            {
                _repository.Delete(dbData);
                _uow.SaveChanges();
            }
        }

        public List<ProductionDto> GetExactResult(List<ProductionDto> listItem)
        {
            List<ProductionDto> list = new List<ProductionDto>();

            var unpacked = Convert.ToDecimal(0);

            var plant = string.Empty;

            var facode = string.Empty;

            foreach (var item in listItem)
            {
                //set unpacked to 0 if plant or fa code different
                if (plant != item.PlantWerks || facode != item.FaCode)
                {
                    unpacked = 0;
                }

                if (unpacked == 0)
                {
                    var oldData = GetOldSaldo(item.CompanyCode, item.PlantWerks, item.FaCode, item.ProductionDate).LastOrDefault();

                    unpacked = oldData == null ? 0 : oldData.QtyUnpacked.Value;

                    plant = item.PlantWerks;

                    facode = item.FaCode;
                }

                var wasteData = _wasteBll.GetExistDto(item.CompanyCode, item.PlantWerks, item.FaCode, item.ProductionDate);

                var oldUnpacked = unpacked;

                var oldWaste = wasteData == null ? 0 : wasteData.PACKER_REJECT_STICK_QTY;

                var prodWaste = oldWaste < item.QtyProduced ? oldWaste : 0;

                var unpackedQty = oldUnpacked + item.QtyProduced - item.QtyPacked - oldWaste;

                var prodQty = item.QtyProduced - prodWaste;

                item.QtyUnpacked = unpackedQty;

                item.QtyProduced = prodQty;

                list.Add(item);

                unpacked = unpackedQty.Value;
            }

            return list;
        }

        public List<ProductionUploadItemsOutput> ValidationDailyUploadDocumentProcess(List<ProductionUploadItemsInput> inputs, string qtyPacked, string qty)
        {
            var messageList = new List<string>();
            var outputList = new List<ProductionUploadItemsOutput>();

            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                var output = Mapper.Map<ProductionUploadItemsOutput>(inputItem);
                output.IsValid = true;

                var checkCountdataDailyProduction =
                    inputs.Where(
                        c =>
                            c.CompanyCode == output.CompanyCode && c.PlantWerks == output.PlantWerks &&
                            c.FaCode == output.FaCode && c.ProductionDate == output.ProductionDate).ToList();
                if (checkCountdataDailyProduction.Count > 1)
                {
                    //double Daily Production Data
                    output.IsValid = false;
                    messageList.Add("Duplicate Daily Production Data  [" + output.CompanyCode + ", " + output.PlantWerks + ", "
                        + output.FaCode + ", " + output.ProductionDate + "]");
                }

                #region -------------- Company Code Validation --------------
                List<string> messages;
                T001 companyTypeData = null;

                if (ValidateCompanyCode(output.CompanyCode, out messages, out companyTypeData))
                {
                    output.CompanyCode = companyTypeData.BUKRS;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region -------------- Plant Code Validation --------------

                Plant plantTypeData = null;
                if (ValidatePlantCode(output.PlantWerks, out messages, out plantTypeData))
                {
                    output.PlantWerks = plantTypeData.WERKS;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region ---------------FaCode validation-----------------
                ZAIDM_EX_BRAND brandTypeData;

                if (ValidateFaCode(output.PlantWerks, output.FaCode, out messages, out brandTypeData))
                {
                    output.FaCode = brandTypeData.FA_CODE;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region -------------Brand Description--------------------

                if (ValidateBrandCe(output.PlantWerks, output.FaCode, output.BrandDescription, out messages, out brandTypeData))
                {
                    output.BrandDescription = brandTypeData.BRAND_CE;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region ---------------Production Date validation-------------

                int temp;
                DateTime dateTemp;
                if (Int32.TryParse(output.ProductionDate, out temp))
                {
                    try
                    {
                        output.ProductionDate = DateTime.FromOADate(Convert.ToDouble(output.ProductionDate)).ToString("dd MMM yyyy");
                    }
                    catch (Exception)
                    {
                        messageList.Add("Production Date [" + output.ProductionDate + "] not valid");
                    }

                }
                else
                {
                    messageList.Add("Production Date [" + output.ProductionDate + "] not valid");
                }
                #endregion

                #region -------Quantity Production validation--------
                decimal tempDecimal;
                if (decimal.TryParse(output.QtyPacked, out tempDecimal) || output.QtyPacked == "" || output.QtyPacked == "-")
                {
                    output.QtyPacked = output.QtyPacked == "" || output.QtyPacked == "-" ? "0" : output.QtyPacked;

                }

                else
                {
                    output.QtyPacked = output.QtyPacked;
                    messageList.Add("Quantity Packed [" + output.QtyPacked + "] not valid");
                }
                #endregion

                #region -----------Quantity Validation-------------
                if (decimal.TryParse(output.Qty, out tempDecimal) || output.Qty == "" || output.Qty == "-")
                {
                    output.Qty = output.Qty == "" || output.Qty == "-" ? "0" : output.Qty;
                }
                else
                {
                    output.Qty = output.QtyPacked;
                    messageList.Add("Quantity [" + output.Qty + "] not valid");
                }
                #endregion

                #region -------------- UOM Validation --------------------
                UOM uomTypeData = null;

                if (ValidateUomId(output.Uom, out messages, out uomTypeData))
                {
                    output.Uom = uomTypeData.UOM_ID;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region -------------- Set Message Info if exists ---------------

                if (messageList.Count > 0)
                {
                    output.IsValid = false;
                    output.Message = "";
                    foreach (var message in messageList)
                    {
                        output.Message += message + ";";
                    }
                }

                else
                {
                    output.IsValid = true;
                    output.Message = string.Empty;
                }

                #endregion

                outputList.Add(output);
            }

            return outputList;

        }

        private bool ValidateCompanyCode(string companyCode, out List<string> message,
           out T001 companyData)
        {
            companyData = null;
            var valResult = false;
            var messageList = new List<string>();

            #region ------------Company Code Validation-------------
            if (!string.IsNullOrWhiteSpace(companyCode))
            {

                companyData = _companyBll.GetById(companyCode);
                if (companyData == null)
                {
                    messageList.Add("Company Code [" + companyCode + "] not valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Company Code is empty");
            }

            #endregion

            message = messageList;

            return valResult;
        }

        private bool ValidatePlantCode(string plantCode, out List<string> message,
          out Plant plantData)
        {
            plantData = null;
            var valResult = false;
            var messageList = new List<string>();

            #region ------------Plant Code Validation-------------
            if (!string.IsNullOrWhiteSpace(plantCode))
            {
                plantData = _plantBll.GetId(plantCode);
                if (plantData == null)
                {
                    messageList.Add("Plant Code/WERKS [" + plantCode + "] not valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Plant Code/WERKS is empty");
            }

            #endregion

            message = messageList;

            return valResult;
        }

        private bool ValidateFaCode(string plantWerks, string faCode, out List<string> message,
            out ZAIDM_EX_BRAND brandData)
        {
            brandData = null;
            var valResult = false;
            var messageList = new List<string>();

            #region ----------FA Code Validation--------------

            if (!string.IsNullOrWhiteSpace(faCode))
            {
                brandData = _brandRegistrationBll.GetByFaCode(plantWerks, faCode);
                if (brandData == null)
                {
                    messageList.Add("Finish Goods [" + faCode + "] not valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Finish Goods Code is empty");
            }

            #endregion

            message = messageList;
            return valResult;
        }

        private bool ValidateBrandCe(string plantWerk, string faCode, string brandCe, out List<string> message,
            out ZAIDM_EX_BRAND brandData)
        {
            brandData = null;
            var valResult = false;
            var messageList = new List<string>();

            #region ----------BrandCE Validation--------------

            if (!string.IsNullOrWhiteSpace(brandCe))
            {
                brandData = _brandRegistrationBll.GetBrandCe(plantWerk, faCode, brandCe);
                if (brandData == null)
                {
                    messageList.Add("Brand Description [" + brandCe + "] not registered yet in plant [" + plantWerk + "]");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Brand Description  is empty");
            }

            #endregion

            message = messageList;
            return valResult;
        }

        private bool ValidateUomId(string uomId, out List<string> message, out UOM uomData)
        {
            uomData = null;
            var valResult = false;
            var messageList = new List<string>();
            #region ----------------UOM Validation-------------------------
            if (!string.IsNullOrWhiteSpace(uomId))
            {
                uomData = _uomBll.GetById(uomId);
                if (uomData == null)
                {
                    messageList.Add("Uom Id  [" + uomId + "] not valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Uom Id  is empty");
            }
            #endregion

            message = messageList;
            return valResult;
        }

        public List<ProductionDto> GetOldSaldo(string company, string plant, string facode, DateTime prodDate)
        {
            List<ProductionDto> data = new List<ProductionDto>();

            var list = _repository.Get(p => p.COMPANY_CODE == company && p.WERKS == plant && p.FA_CODE == facode && p.PRODUCTION_DATE < prodDate).OrderBy(p => p.PRODUCTION_DATE).ToList();

            var lastUnpacked = Convert.ToDecimal(0);

            foreach (var item in list)
            {
                var wasteData = _wasteBll.GetExistDto(item.COMPANY_CODE, item.WERKS, item.FA_CODE, item.PRODUCTION_DATE);

                var oldWaste = wasteData == null ? 0 : wasteData.PACKER_REJECT_STICK_QTY;

                var prodQty = item.QTY == null ? 0 : item.QTY;

                var packed = item.QTY_PACKED == null ? 0 : item.QTY_PACKED;

                var prodWaste = oldWaste < prodQty ? oldWaste : 0;

                var prod = new ProductionDto
                {
                    PlantWerks = item.WERKS,
                    FaCode = item.FA_CODE,
                    ProductionDate = item.PRODUCTION_DATE,
                    QtyProduced = prodQty - prodWaste,
                    QtyPacked = packed,
                    QtyUnpacked = lastUnpacked + prodQty - packed - oldWaste
                };

                lastUnpacked = prod.QtyUnpacked == null ? 0 : prod.QtyUnpacked.Value;

                data.Add(prod);
            }

            return data;
        }
    }
}
