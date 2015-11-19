using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
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
    public class WasteBLL : IWasteBLL
    {
        private IGenericRepository<WASTE> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_BRAND> _repositoryBrand;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repositoryGood;
        private IGenericRepository<UOM> _repositoryUom;
        private IGenericRepository<T001W> _repositoryPlant;
        private IGenericRepository<T001> _repositoryCompany;
        private ICompanyBLL _companyBll;
        private IPlantBLL _plantBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private ChangesHistoryBLL _changesHistoryBll;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;

        private IWasteStockBLL _wasteStockBll;
        private IMaterialBLL _materialBll;

        public WasteBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<WASTE>();
            _repositoryBrand = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryGood = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
            _repositoryUom = _uow.GetGenericRepository<UOM>();
            _repositoryPlant = uow.GetGenericRepository<T001W>();
            _repositoryCompany = _uow.GetGenericRepository<T001>();
            _changesHistoryBll = new ChangesHistoryBLL(uow, logger);
            _companyBll = new CompanyBLL(_uow, _logger);
            _plantBll = new PlantBLL(_uow, _logger);
            _brandRegistrationBll = new BrandRegistrationBLL(_uow, _logger);
            _userPlantBll = new UserPlantMapBLL(_uow, _logger);
            _poaMapBll = new POAMapBLL(_uow, _logger);
            _wasteStockBll = new WasteStockBLL(_uow, _logger);
            _materialBll = new MaterialBLL(_uow, _logger);
        }
        public List<WasteDto> GetAllByParam(WasteGetByParamInput input)
        {
            Expression<Func<WASTE, bool>> queryFilter = PredicateHelper.True<WASTE>();
            if (!string.IsNullOrEmpty(input.Company))
            {
                queryFilter = queryFilter.And(c => c.COMPANY_CODE == input.Company);
            }
            if (!string.IsNullOrEmpty(input.Plant))
            {
                queryFilter = queryFilter.And(c => c.WERKS == input.Plant);
            }
            if (!string.IsNullOrEmpty(input.WasteProductionDate))
            {
                var dt = Convert.ToDateTime(input.WasteProductionDate);
                queryFilter = queryFilter.And(c => c.WASTE_PROD_DATE == dt);
            }
            if (!string.IsNullOrEmpty(input.UserId))
            {
                var listUserPlant = _userPlantBll.GetPlantByUserId(input.UserId);

                var listPoaPlant = _poaMapBll.GetPlantByPoaId(input.UserId);

                queryFilter = queryFilter.And(c => listUserPlant.Contains(c.WERKS) || listPoaPlant.Contains(c.WERKS));
            }


            Func<IQueryable<WASTE>, IOrderedQueryable<WASTE>> orderBy = null;
            {
                if (!string.IsNullOrEmpty(input.ShortOrderColumn))
                {
                    orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<WASTE>(input.ShortOrderColumn));
                }

                var dbData = _repository.Get(queryFilter, orderBy);
                if (dbData == null)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                }
                var mapResult = Mapper.Map<List<WasteDto>>(dbData.ToList());

                return mapResult;
            }
        }

        public List<WasteDto> GetAllWaste()
        {
            var dbData = _repository.Get().ToList();
            return Mapper.Map<List<WasteDto>>(dbData);

        }

        public bool Save(WasteDto wasteDto, string userId)
        {
            var isNewData = true;

            #region ------get description code-----
            var company = _companyBll.GetById(wasteDto.CompanyCode);
            var plant = _plantBll.GetT001WById(wasteDto.PlantWerks);
            var brandDesc = _brandRegistrationBll.GetById(wasteDto.PlantWerks, wasteDto.FaCode);

            wasteDto.CompanyName = company.BUTXT;
            wasteDto.PlantName = plant.NAME1;
            wasteDto.BrandDescription = brandDesc.BRAND_CE;

            #endregion

            var dbWaste = Mapper.Map<WASTE>(wasteDto);

            dbWaste.MARKER_REJECT_STICK_QTY = wasteDto.MarkerStr == null ? 0 : Convert.ToDecimal(wasteDto.MarkerStr);
            dbWaste.PACKER_REJECT_STICK_QTY = wasteDto.PackerStr == null ? 0 : Convert.ToDecimal(wasteDto.PackerStr);
            dbWaste.DUST_WASTE_GRAM_QTY = wasteDto.DustGramStr == null ? 0 : Convert.ToDecimal(wasteDto.DustGramStr);
            dbWaste.FLOOR_WASTE_GRAM_QTY = wasteDto.FloorGramStr == null ? 0 : Convert.ToDecimal(wasteDto.FloorGramStr);
            dbWaste.DUST_WASTE_STICK_QTY = wasteDto.DustStickStr == null ? 0 : Convert.ToDecimal(wasteDto.DustStickStr);
            dbWaste.FLOOR_WASTE_STICK_QTY = wasteDto.FloorStickStr == null ? 0 : Convert.ToDecimal(wasteDto.FloorStickStr);
            dbWaste.STAMP_WASTE_QTY = wasteDto.StampWasteQtyStr == null ? 0 : Convert.ToDecimal(wasteDto.StampWasteQtyStr);

            var origin = _repository.GetByID(wasteDto.CompanyCodeX, wasteDto.PlantWerksX, wasteDto.FaCodeX,
                wasteDto.WasteProductionDateX);

            var originDto = Mapper.Map<WasteDto>(origin);

            dbWaste.CREATED_BY = userId;
            dbWaste.CREATED_DATE = DateTime.Now;

            if (originDto != null)
            {
                dbWaste.CREATED_BY = origin.CREATED_BY;
                dbWaste.CREATED_DATE = origin.CREATED_DATE;

                SetChange(originDto, wasteDto, userId);
                isNewData = false;
            }

            _repository.InsertOrUpdate(dbWaste);

            //update waste stock table
            UpdateWasteStockTable(dbWaste, userId, isNewData);


            _uow.SaveChanges();

            return isNewData;
        }

        private void UpdateWasteStockTable(WASTE dbWaste, string userId, bool isNewData)
        {

            var dbdata = GetAllByParam(new WasteGetByParamInput());
            var dbQtyWaste = CalculateWasteQuantity(dbdata);
            var listWasteStockDto = new List<WasteStockDto>();

            foreach (var item in dbQtyWaste)
            {
                decimal? updateValueFloor = item.FloorWasteGramQty;
                decimal? updateValueDust = item.DustWasteGramQty;
                decimal? updateValueStamp = item.StampWasteQty;

                if (isNewData && item.PlantWerks == dbWaste.WERKS)
                {
                    updateValueFloor = item.FloorWasteGramQty  + dbWaste.FLOOR_WASTE_GRAM_QTY;
                    updateValueDust = item.DustWasteGramQty  + dbWaste.DUST_WASTE_GRAM_QTY;
                    updateValueStamp = item.StampWasteQty  + dbWaste.STAMP_WASTE_QTY;
  
                }
               
                var wasStockWsapoon = new WasteStockDto();
                wasStockWsapoon.WERKS = item.PlantWerks;
                wasStockWsapoon.MATERIAL_NUMBER = Constans.WasteSapon;
                wasStockWsapoon.STOCK = Convert.ToDecimal(updateValueFloor);
                wasStockWsapoon.CREATED_BY = userId;

                listWasteStockDto.Add(wasStockWsapoon);

                var wasteStockGagang = new WasteStockDto();
                wasteStockGagang.WERKS = item.PlantWerks;
                wasteStockGagang.MATERIAL_NUMBER = Constans.WasteGagang;
                wasteStockGagang.STOCK = Convert.ToDecimal(updateValueDust);
                wasteStockGagang.CREATED_BY = userId;

                listWasteStockDto.Add(wasteStockGagang);

                var wasteStockStem = new WasteStockDto();
                wasteStockStem.WERKS = item.PlantWerks;
                wasteStockStem.MATERIAL_NUMBER = Constans.WasteStem;
                wasteStockStem.STOCK = Convert.ToDecimal(updateValueStamp);
                wasteStockStem.CREATED_BY = userId;

                listWasteStockDto.Add(wasteStockStem);

            }

            foreach (var wasteStockDto in listWasteStockDto)
            {
                //CHECK ON MATERIAL PLANT AND STICKER CODE EXIST
                var dbMaterial = _materialBll.GetByPlantIdAndStickerCode(wasteStockDto.WERKS, wasteStockDto.MATERIAL_NUMBER);
                if (dbMaterial != null)
                {
                    _wasteStockBll.UpdateWasteStockFromWaste(wasteStockDto);
                }
            }

        }
        public WasteDto GetById(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            var dbData = _repository.GetByID(companyCode, plantWerk, faCode, wasteProductionDate);
            var item = Mapper.Map<WasteDto>(dbData);

            item.MarkerStr = item.MarkerRejectStickQty == null ? string.Empty : item.MarkerRejectStickQty.ToString();
            item.PackerStr = item.PackerRejectStickQty == null ? string.Empty : item.PackerRejectStickQty.ToString();
            item.DustGramStr = item.DustWasteGramQty == null ? string.Empty : item.DustWasteGramQty.ToString();
            item.FloorGramStr = item.FloorWasteGramQty == null ? string.Empty : item.FloorWasteGramQty.ToString();
            item.DustStickStr = item.DustWasteStickQty == null ? string.Empty : item.DustWasteStickQty.ToString();
            item.FloorStickStr = item.FloorWasteStickQty == null ? string.Empty : item.FloorWasteStickQty.ToString();
            item.StampWasteQtyStr = item.StampWasteQty == null ? string.Empty : item.StampWasteQty.ToString();

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return item;
        }

        public WASTE GetExistDto(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            return
                _uow.GetGenericRepository<WASTE>()
                    .Get(
                        p =>
                            p.COMPANY_CODE == companyCode && p.WERKS == plantWerk && p.FA_CODE == faCode &&
                            p.WASTE_PROD_DATE == wasteProductionDate)
                    .FirstOrDefault();
        }

        public List<WasteDto> CalculateWasteQuantity(List<WasteDto> wasteDtos)
        {
            var result = wasteDtos.GroupBy(p => p.PlantWerks)
                .Select(p => new WasteDto
                {

                    PlantWerks = p.Key,
                    CompanyCode = p.Select(x => x.CompanyCode).FirstOrDefault(),
                    FaCode = p.Select(x => x.FaCode).FirstOrDefault(),
                    WasteProductionDate = p.Select(x => x.WasteProductionDate).FirstOrDefault(),
                    FloorWasteGramQty = p.Sum(x => x.FloorWasteGramQty),
                    DustWasteGramQty = p.Sum(x => x.DustWasteGramQty),
                    StampWasteQty = p.Sum(x => x.StampWasteQty)

                }).ToList();



            return result;
        }

        public void SaveUpload(WasteUploadItems wasteUpload, string userId)
        {
            bool isNewData = false;
            var dbUpload = Mapper.Map<WASTE>(wasteUpload);

            var dbResult = _repository.GetByID(dbUpload.COMPANY_CODE, dbUpload.WERKS, dbUpload.FA_CODE,
                dbUpload.WASTE_PROD_DATE);

            if (dbResult == null)
            {
                isNewData = true;
            }
            _repository.InsertOrUpdate(dbUpload);

            UpdateWasteStockTable(dbUpload, userId, isNewData);
            _uow.SaveChanges();
        }

        private void SetChange(WasteDto origin, WasteDto data, string userId)
        {
            var changeData = new Dictionary<string, bool>();
            changeData.Add("COMPANY_CODE", origin.CompanyCode == data.CompanyCode);
            changeData.Add("WERKS", origin.PlantWerks == data.PlantWerks);
            changeData.Add("FA_CODE", origin.FaCode == data.FaCode);
            changeData.Add("WASTE_DATE", origin.WasteProductionDate == data.WasteProductionDate);
            changeData.Add("BRAND_DESC", origin.BrandDescription == data.BrandDescription);
            changeData.Add("PLANT_NAME", origin.PlantName == data.PlantName);
            changeData.Add("COMPANY_NAME", origin.CompanyName == data.CompanyName);
            changeData.Add("MARKER_REJECT_STICK_QTY", origin.MarkerRejectStickQty == data.MarkerRejectStickQty);
            changeData.Add("PACKER_REJECT_STICK_QTY", origin.PackerRejectStickQty == data.PackerRejectStickQty);
            changeData.Add("DUST_WASTE_GRAM_QTY", origin.DustWasteGramQty == data.DustWasteGramQty);
            changeData.Add("FLOOR_WASTE_GRAM_QTY", origin.FloorWasteGramQty == data.FloorWasteGramQty);
            changeData.Add("DUST_WASTE_STICK_QTY", origin.DustWasteStickQty == data.DustWasteStickQty);
            changeData.Add("FLOOR_WASTE_STICK_QTY", origin.FloorWasteStickQty == data.FloorWasteStickQty);

            foreach (var listChange in changeData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY()
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.CK4C,
                        FORM_ID = "Waste_" + data.CompanyCode + "_" + data.PlantWerks + "_" + data.FaCode + "_" +
                            data.WasteProductionDate.ToString("ddMMMyyyy"),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };

                    switch (listChange.Key)
                    {
                        case "COMPANY_CODE":
                            changes.OLD_VALUE = origin.CompanyCode;
                            changes.NEW_VALUE = data.CompanyCode;
                            changes.FIELD_NAME = "Company";
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
                        case "WASTE_DATE":
                            changes.OLD_VALUE = origin.WasteProductionDate.ToString();
                            changes.NEW_VALUE = data.WasteProductionDate.ToString();
                            changes.FIELD_NAME = "Waste Production Date";
                            break;
                        case "BRAND_DESC":
                            changes.OLD_VALUE = origin.BrandDescription;
                            changes.NEW_VALUE = data.BrandDescription;
                            changes.FIELD_NAME = "Brand Description";
                            break;
                        case "PLANT_NAME":
                            changes.OLD_VALUE = origin.PlantName;
                            changes.NEW_VALUE = data.PlantName;
                            changes.FIELD_NAME = "Plant";
                            break;
                        case "COMPANY_NAME":
                            changes.OLD_VALUE = origin.CompanyName;
                            changes.NEW_VALUE = data.CompanyName;
                            changes.FIELD_NAME = "Company";
                            break;
                        case "MARKER_REJECT_STICK_QTY":
                            changes.OLD_VALUE = origin.MarkerRejectStickQty.ToString();
                            changes.NEW_VALUE = data.MarkerRejectStickQty.ToString();
                            changes.FIELD_NAME = "Maker Reject Cigarette(stick)";
                            break;
                        case "PACKER_REJECT_STICK_QTY":
                            changes.OLD_VALUE = origin.PackerRejectStickQty.ToString();
                            changes.NEW_VALUE = data.PackerRejectStickQty.ToString();
                            changes.FIELD_NAME = "Packer Reject Cigarette(stick)";
                            break;
                        case "DUST_WASTE_GRAM_QTY":
                            changes.OLD_VALUE = origin.DustWasteGramQty.ToString();
                            changes.NEW_VALUE = data.DustWasteGramQty.ToString();
                            changes.FIELD_NAME = "Dust Waste QTY (gram)";
                            break;
                        case "FLOOR_WASTE_GRAM_QTY":
                            changes.OLD_VALUE = origin.FloorWasteGramQty.ToString();
                            changes.NEW_VALUE = data.FloorWasteGramQty.ToString();
                            changes.FIELD_NAME = "Floor Waste QTY (gram)";
                            break;
                        case "DUST_WASTE_STICK_QTY":
                            changes.OLD_VALUE = origin.DustWasteStickQty.ToString();
                            changes.NEW_VALUE = data.DustWasteStickQty.ToString();
                            changes.FIELD_NAME = "Dust Waste QTY (Stick)";
                            break;
                        case "FLOOR_WASTE_STICK_QTY":
                            changes.OLD_VALUE = origin.FloorWasteStickQty.ToString();
                            changes.NEW_VALUE = data.FloorWasteStickQty.ToString();
                            changes.FIELD_NAME = "Floor Waste QTY (Stick)";
                            break;
                        default: break;
                    }
                    _changesHistoryBll.AddHistory(changes);

                }

            }

        }

        public void DeleteOldData(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            var dbData = _repository.GetByID(companyCode, plantWerk, faCode, wasteProductionDate);

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

        public List<WasteUploadItemsOuput> ValidationWasteUploadDocumentProcess(List<WasteUploadItemsInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<WasteUploadItemsOuput>();

            foreach (var inputItem in inputs)
            {
                messageList.Clear();
                var output = Mapper.Map<WasteUploadItemsOuput>(inputItem);

                output.IsValid = true;

                var checkCountdataWasteProduction =
                    inputs.Where(
                        c =>
                            c.CompanyCode == output.CompanyCode && c.PlantWerks == output.PlantWerks &&
                            c.FaCode == output.FaCode && c.WasteProductionDate == output.WasteProductionDate).ToList();

                if (checkCountdataWasteProduction.Count > 1)
                {
                    //Existing Waste Production data
                    output.IsValid = false;
                    messageList.Add("Duplicate Waste Production Data  [" + output.CompanyCode + ", " + output.PlantWerks + ", "
                        + output.FaCode + ", " + output.WasteProductionDate + "]");
                }

                List<string> messages;

                #region -------------- Company Code Validation ---------------

                T001 companyTypedata = null;
                if (ValidateCompanyCode(output.CompanyCode, out messages, out companyTypedata))
                {
                    output.CompanyCode = companyTypedata.BUKRS;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }
                #endregion

                #region -------------- Plant Code Validation ---------------

                Plant plantTypeData = null;
                if (ValidationPlantCode(output.PlantWerks, out messages, out plantTypeData))
                {
                    output.PlantWerks = plantTypeData.WERKS;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }

                #endregion

                #region -------------- Fa Code Vlidation ------------------

                ZAIDM_EX_BRAND brandTypeData = null;
                if (ValidationFaCode(output.PlantWerks, output.FaCode, out messages, out brandTypeData))
                {
                    output.FaCode = brandTypeData.FA_CODE;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }
                #endregion

                #region ------------ Brand Description Validation -----------------

                if (ValidationBrandCe(output.PlantWerks, output.FaCode, output.BrandDescription, out messages, out brandTypeData))
                {
                    output.BrandDescription = brandTypeData.BRAND_CE;
                }
                else
                {
                    output.IsValid = false;
                    messageList.AddRange(messages);
                }
                #endregion

                #region ---------------Waste Production Date validation-------------
                int temp;
                DateTime dateTemp;
                if (Int32.TryParse(output.WasteProductionDate, out temp))
                {
                    try
                    {
                        output.WasteProductionDate = DateTime.FromOADate(Convert.ToDouble(output.WasteProductionDate)).ToString("dd MMM yyyy");
                    }
                    catch (Exception)
                    {
                        messageList.Add("Waste Production Date [" + output.WasteProductionDate + "] not valid");
                    }

                }
                else
                {
                    messageList.Add("Waste Production Date [" + output.WasteProductionDate + "] not valid");
                }
                #endregion

                #region -----------MarkerRejectStickQty Validation-------------

                decimal tempDecimal;
                if (decimal.TryParse(output.MarkerRejectStickQty, out tempDecimal) || output.MarkerRejectStickQty == "" || output.MarkerRejectStickQty == "-")
                {
                    output.MarkerRejectStickQty = output.MarkerRejectStickQty == "" || output.MarkerRejectStickQty == "-" ? "0" : output.MarkerRejectStickQty;
                }
                else
                {
                    output.MarkerRejectStickQty = output.MarkerRejectStickQty;
                    messageList.Add("Marker Reject Stick Qty [" + output.MarkerRejectStickQty + "] not valid");
                }
                #endregion

                #region -----------PackerRejectStickQty Validation-------------
                if (decimal.TryParse(output.PackerRejectStickQty, out tempDecimal) || output.PackerRejectStickQty == "" || output.PackerRejectStickQty == "-")
                {
                    output.PackerRejectStickQty = output.PackerRejectStickQty == "" || output.PackerRejectStickQty == "-" ? "0" : output.PackerRejectStickQty;
                }
                else
                {
                    output.PackerRejectStickQty = output.PackerRejectStickQty;
                    messageList.Add("Packer Reject Stick Qty [" + output.PackerRejectStickQty + "] not valid");
                }
                #endregion

                #region -----------DustWasteGramQty Validation-------------
                if (decimal.TryParse(output.DustWasteGramQty, out tempDecimal) || output.DustWasteGramQty == "" || output.DustWasteGramQty == "-")
                {
                    output.DustWasteGramQty = output.DustWasteGramQty == "" || output.DustWasteGramQty == "-" ? "0" : output.DustWasteGramQty;
                }
                else
                {
                    output.DustWasteGramQty = output.DustWasteGramQty;
                    messageList.Add("Dust Waste Gram Qty [" + output.DustWasteGramQty + "] not valid");
                }
                #endregion

                #region -----------FloorWasteGramQty Validation-------------
                if (decimal.TryParse(output.FloorWasteGramQty, out tempDecimal) || output.FloorWasteGramQty == "" || output.FloorWasteGramQty == "-")
                {
                    output.FloorWasteGramQty = output.FloorWasteGramQty == "" || output.FloorWasteGramQty == "-" ? "0" : output.FloorWasteGramQty;
                }
                else
                {
                    output.FloorWasteGramQty = output.FloorWasteGramQty;
                    messageList.Add("Floor Waste Gram Qty [" + output.FloorWasteGramQty + "] not valid");
                }
                #endregion

                #region -----------DustWasteStickQty Validation-------------
                //if (decimal.TryParse(output.DustWasteStickQty, out tempDecimal) || output.DustWasteStickQty == "" || output.DustWasteStickQty == "-")
                //{
                //    output.DustWasteStickQty = output.DustWasteStickQty == "" || output.DustWasteStickQty == "-" ? "0" : output.DustWasteStickQty;
                //}
                //else
                //{
                //    output.DustWasteStickQty = output.DustWasteStickQty;
                //    messageList.Add("Dust Waste Stick Qty [" + output.DustWasteStickQty + "] not valid");
                //}
                #endregion

                #region -----------FloorWasteStickQty Validation-------------
                //if (decimal.TryParse(output.FloorWasteStickQty, out tempDecimal) || output.FloorWasteStickQty == "" || output.FloorWasteStickQty == "-")
                //{
                //    output.FloorWasteStickQty = output.FloorWasteStickQty == "" || output.FloorWasteStickQty == "-" ? "0" : output.FloorWasteStickQty;
                //}
                //else
                //{
                //    output.FloorWasteStickQty = output.FloorWasteStickQty;
                //    messageList.Add("Floor Waste stick Qty [" + output.FloorWasteStickQty + "] not valid");
                //}
                #endregion

                #region -----------StampWasteGramQty Validation-------------
                if (decimal.TryParse(output.StampWasteQty, out tempDecimal) || output.StampWasteQty == "" || output.StampWasteQty == "-")
                {
                    output.StampWasteQty = output.StampWasteQty == "" || output.StampWasteQty == "-" ? "0" : output.StampWasteQty;
                }
                else
                {
                    output.StampWasteQty = output.StampWasteQty;
                    messageList.Add("Stem Waste Gram Qty [" + output.StampWasteQty + "] not valid");
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
                    output.Message = string.Empty;
                    output.IsValid = true;
                }

                #endregion


                outputList.Add(output);
            }

            return outputList;
        }

        private bool ValidateCompanyCode(string companyCode, out List<string> message, out T001 companyData)
        {
            companyData = null;
            var valResult = false;
            var messageList = new List<string>();

            #region --------------Company Code Validation-------------

            if (!string.IsNullOrWhiteSpace(companyCode))
            {
                companyData = _repositoryCompany.GetByID(companyCode);
                if (companyData == null)
                {
                    messageList.Add("Company Code [" + companyCode + "] not Valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Company Code is Empty");
            }

            #endregion

            message = messageList;

            return valResult;

        }

        private bool ValidationPlantCode(string plantCode, out List<string> message, out Plant plantData)
        {
            plantData = null;
            var valResult = false;
            var messageList = new List<string>();

            #region ---------Plant Code Validation----------

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
                messageList.Add("Plant Code is Empty");
            }
            #endregion

            message = messageList;

            return valResult;
        }

        private bool ValidationFaCode(string plantWerks, string faCode, out List<string> message, out ZAIDM_EX_BRAND brandData)
        {
            brandData = null;
            var valResult = false;
            var messageLits = new List<string>();

            #region --------- Fa Code Validation -------------

            if (!string.IsNullOrWhiteSpace(faCode))
            {
                brandData = _brandRegistrationBll.GetByFaCode(plantWerks, faCode);
                if (brandData == null)
                {
                    messageLits.Add("Finish Goods [" + faCode + "] not valid");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageLits.Add("Fa Code is Empty");
            }
            #endregion

            message = messageLits;
            return valResult;
        }

        private bool ValidationBrandCe(string plantWerks, string faCode, string brandCe, out List<string> message, out ZAIDM_EX_BRAND brandData)
        {
            brandData = null;
            var valResult = false;
            var messageList = new List<string>();

            #region ------------- Brand Ce Validation --------------------

            if (!string.IsNullOrWhiteSpace(brandCe))
            {
                brandData = _brandRegistrationBll.GetBrandCe(plantWerks, faCode, brandCe);
                if (brandData == null)
                {
                    messageList.Add("Brand Description [" + brandCe + "] not registered yet in plant [" + plantWerks + "]");
                }
                else
                {
                    valResult = true;
                }
            }
            else
            {
                messageList.Add("Brand Description is Empty");
            }
            #endregion

            message = messageList;
            return valResult;
        }


    }
}
