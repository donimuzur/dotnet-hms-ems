using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
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
        private ChangesHistoryBLL _changesHistoryBll;

        public WasteBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<WASTE>();
            _repositoryBrand = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryGood = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
            _repositoryUom = _uow.GetGenericRepository<UOM>();
            _repositoryPlant = _uow.GetGenericRepository<T001W>();
            _changesHistoryBll = new ChangesHistoryBLL(uow, logger);
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

        public void Save(WasteDto wasteDto, string userId)
        {

            var dbWaste = Mapper.Map<WASTE>(wasteDto);

            var origin = _repository.GetByID(dbWaste.COMPANY_CODE, dbWaste.WERKS, dbWaste.FA_CODE,
                dbWaste.WASTE_PROD_DATE);

            var originDto = Mapper.Map<WasteDto>(origin);
            dbWaste.MODIFIED_DATE = DateTime.Now;

            SetChange(originDto, wasteDto, userId);
            _repository.InsertOrUpdate(dbWaste);
            _uow.SaveChanges();
        }

        public WasteDto GetById(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            var dbData = _repository.GetByID(companyCode, plantWerk, faCode, wasteProductionDate);
            var item = Mapper.Map<WasteDto>(dbData);

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


        public void SaveUpload(WasteUploadItems wasteUpload)
        {
            var dbUpload = Mapper.Map<WASTE>(wasteUpload);
            _repository.InsertOrUpdate(dbUpload);
            _uow.SaveChanges();
        }

        private void SetChange(WasteDto origin, WasteDto data, string userId)
        {
            var changeData = new Dictionary<string, bool>();
            changeData.Add("COMPANY_CODE", origin.CompanyCode == data.CompanyCode);
            changeData.Add("WERKS", origin.PlantWerks == data.PlantWerks);
            changeData.Add("FA_CODE", origin.FaCode == data.FaCode);
            changeData.Add("PRODUCTION_DATE", origin.WasteProductionDate == data.WasteProductionDate);
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
                        FORM_ID =
                            data.CompanyCode + "_" + data.PlantWerks + "_" + data.FaCode + "_" +
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
                            changes.OLD_VALUE = origin.WasteProductionDate.ToString();
                            changes.NEW_VALUE = data.WasteProductionDate.ToString();
                            break;
                        case "BRAND_DESC":
                            changes.OLD_VALUE = origin.BrandDescription;
                            changes.NEW_VALUE = data.BrandDescription;
                            break;
                        case "PLANT_NAME":
                            changes.OLD_VALUE = origin.PlantName;
                            changes.NEW_VALUE = data.PlantName;
                            break;
                        case "COMPANY_NAME":
                            changes.OLD_VALUE = origin.CompanyName;
                            changes.NEW_VALUE = data.CompanyName;
                            break;
                        case "MARKER_REJECT_STICK_QTY":
                            changes.OLD_VALUE = origin.MarkerRejectStickQty.ToString();
                            changes.NEW_VALUE = data.MarkerRejectStickQty.ToString();
                            break;
                        case "PACKER_REJECT_STICK_QTY":
                            changes.OLD_VALUE = origin.PackerRejectStickQty.ToString();
                            changes.NEW_VALUE = data.PackerRejectStickQty.ToString();
                            break;
                        case "DUST_WASTE_GRAM_QTY":
                            changes.OLD_VALUE = origin.DustWasteGramQty.ToString();
                            changes.NEW_VALUE = data.DustWasteGramQty.ToString();
                            break;
                        case "FLOOR_WASTE_GRAM_QTY":
                            changes.OLD_VALUE = origin.FloorWasteGramQty.ToString();
                            changes.NEW_VALUE = data.FloorWasteGramQty.ToString();
                            break;
                        case "DUST_WASTE_STICK_QTY":
                            changes.OLD_VALUE = origin.DustWasteStickQty.ToString();
                            changes.NEW_VALUE = data.DustWasteStickQty.ToString();
                            break;
                        case "FLOOR_WASTE_STICK_QTY":
                            changes.OLD_VALUE = origin.FloorWasteStickQty.ToString();
                            changes.NEW_VALUE = data.FloorWasteStickQty.ToString();
                            break;
                        default: break;
                    }
                    _changesHistoryBll.AddHistory(changes);

                }
                
            }

        }
    }
}
