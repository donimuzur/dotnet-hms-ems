using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Math;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Code
{
    public class GlobalFunctions
    {

        public static SelectList GetPoaAll(IPOABLL poabll)
        {
            IPOABLL poaBll = poabll;
            var poaList = poaBll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(poaList);
            return new SelectList(selectItemSource, "ValueField", "TextField");

        }

        public static SelectList GetPoaByNppbkcId(string nppbkcId)
        {
            IZaidmExPOAMapBLL poaMapBll = MvcApplication.GetInstance<ZaidmExPOAMapBLL>();
            var poaList = poaMapBll.GetPOAByNPPBKCID(nppbkcId);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(poaList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetCreatorList(object selectedValue = null)
        {
            IUserBLL userBll = MvcApplication.GetInstance<UserBLL>();
            var users = userBll.GetUsers(new UserInput());
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(users);
            return new SelectList(selectItemSource, "ValueField", "TextField", selectedValue);
        }

        public static SelectList GetNppbkcAll(IZaidmExNPPBKCBLL nppbkcBll)
        {
            IZaidmExNPPBKCBLL nppbkcbll = nppbkcBll;
            var nppbkcList = nppbkcbll.GetAll().Where(x => x.IS_DELETED != true).OrderBy(x=> x.NPPBKC_ID);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(nppbkcList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
        public static MultiSelectList GetNppbkcMultiSelectList()
        {
            IZaidmExNPPBKCBLL nppbkcbll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
            var nppbkcList = nppbkcbll.GetAll().Where(x => x.IS_DELETED != true);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(nppbkcList);
            return new MultiSelectList(selectItemSource, "ValueField", "TextField");
        }
        public static SelectList GetNppbkcByFlagDeletionList(bool isDeleted)
        {
            IZaidmExNPPBKCBLL nppbkcbll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
            var nppbkcList = nppbkcbll.GetByFlagDeletion(isDeleted);
            return new SelectList(nppbkcList, "NPPBKC_ID", "NPPBKC_ID");
        }

        public static ZAIDM_EX_NPPBKC GetNppbkcById(string id)
        {
            IZaidmExNPPBKCBLL nppbkcbll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
            return nppbkcbll.GetById(id);
        }

        public static SelectList GetSupplierPortList(ISupplierPortBLL supplierPortbll)
        {
            ISupplierPortBLL supplierPortBll = supplierPortbll;
            var data = supplierPortBll.GetAll();
            return new SelectList(data, "SUPPLIER_PORT_ID", "PORT_NAME");
        }

        public static SelectList GetMonthList(IMonthBLL monthbll)
        {
            IMonthBLL monthBll = monthbll;
            var data = monthBll.GetAll();
            return new SelectList(data, "MONTH_ID", "MONTH_NAME_ENG");
        }

        public static SelectList GetSupplierPlantList()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetAll();
            return new SelectList(data, "WERKS", "DROPDOWNTEXTFIELD");
        }

        public static SelectList GetGoodTypeList(IZaidmExGoodTypeBLL goodTypeBll)
        {
            var data = goodTypeBll.GetAll().Where(x => x.IS_DELETED != true);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetUomList(IUnitOfMeasurementBLL uomBll)
        {
            var data = uomBll.GetAll().Where(x => x.IS_DELETED != true && x.IS_EMS == true);
            return new SelectList(data, "UOM_ID", "UOM_DESC");
        }

        public static SelectList GetCk5AllowedUomList(IUnitOfMeasurementBLL uomBll)
        {
            var data = uomBll.GetCK5ConvertedUoms();
            return new SelectList(data, "UOM_ID", "UOM_DESC");
        }

        public static SelectList GetCompanyList(ICompanyBLL companyBll)
        {
            var data = companyBll.GetAllData().Where(x => x.IS_DELETED != true);
            return new SelectList(data, "BUKRS", "BUTXT");
        }

        public static SelectList GetVirtualPlantList()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetAllPlant().Where(x => x.IS_DELETED != true);
            var selectItemSource = Mapper.Map<List<SelectItemModelVirtualPlant>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
        public static MultiSelectList GetVirtualPlantListMultiSelect()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetAllPlant().Where(x => x.IS_DELETED != true);
            var selectItemSource = Mapper.Map<List<SelectItemModelVirtualPlant>>(data);
            return new MultiSelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetBrandList()
        {
            IBrandRegistrationBLL brandBLL = MvcApplication.GetInstance<BrandRegistrationBLL>();
            var data = brandBLL.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetPersonalizationCodeList()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetDataPersonalization().Where(x => x.IS_DELETED != true);
            var selectList = from s in data
                             select new SelectListItem
                             {
                                 Value = s.PER_CODE,
                                 Text = s.PER_CODE + "-" + s.PER_DESC
                             };
            return new SelectList(selectList, "Value", "Text");
        }

        public static SelectList GetProductCodeList(IZaidmExProdTypeBLL productBll)
        {
            var data = productBll.GetAll().Where(x => x.IS_DELETED != true);
            var selectList = from s in data
                             select new SelectListItem
                             {
                                 Value = s.PROD_CODE,
                                 Text = s.PROD_CODE + "-" + s.PRODUCT_TYPE + " [" + s.PRODUCT_ALIAS + "]"
                             };
            return new SelectList(selectList, "Value", "Text");
        }

        public static SelectList GetSeriesCodeList(IMasterDataBLL _masterDataBll)
        {
            var data = _masterDataBll.GetAllDataSeries().Where(x => x.IS_DELETED != true);
            var selectList = from s in data
                             select new SelectListItem
                                        {
                                            Value = s.SERIES_CODE,
                                            Text = s.SERIES_CODE + "-" + s.SERIES_VALUE
                                        };
            return new SelectList(selectList, "Value", "Text");
        }

        public static SelectList GetMarketCodeList(IMasterDataBLL masterBll)
        {
            var data = masterBll.GetAllDataMarket().Where(x => x.IS_DELETED != true);
            var selectList = from s in data
                             select new SelectListItem
                             {
                                 Value = s.MARKET_ID,
                                 Text = s.MARKET_ID + "-" + s.MARKET_DESC
                             };
            return new SelectList(selectList, "Value", "Text");
        }

        public static SelectList GetCountryList()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetAllDataCountry();
            return new SelectList(data, "COUNTRY_CODE", "COUNTRY_CODE");
        }

        public static SelectList GetCurrencyList()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetAllDataCurrency();
            return new SelectList(data, "CURRENCY_ID", "CURRENCY_ID");
        }

        public static SelectList GetStickerCodeList()
        {
            IMaterialBLL materialBll = MvcApplication.GetInstance<MaterialBLL>();
            var data = materialBll.GetByFlagDeletion(false);
            return new SelectList(data, "STICKER_CODE", "STICKER_CODE");
        }

        public static SelectList GetCutFillerCodeList(string plant = "")
        {
            IMaterialBLL materialBll = MvcApplication.GetInstance<MaterialBLL>();
            var data = materialBll.GetByFlagDeletion(false, plant);
            return new SelectList(data, "STICKER_CODE", "STICKER_CODE");
        }

        public static SelectList GetConversionUomList()
        {
            IUnitOfMeasurementBLL uomBll = MvcApplication.GetInstance<UnitOfMeasurementBLL>();
            var data = uomBll.GetAll().Where(x => x.IS_DELETED != true);
            var selectList = from s in data
                             select new SelectListItem
                             {
                                 Value = s.UOM_ID,
                                 Text = s.UOM_ID + "-" + s.UOM_DESC
                             };
            return new SelectList(selectList, "Value", "Text");
        }

        //public static SelectList GetKppBcCityList()
        //{
        //    IZaidmExNPPBKCBLL nppbkcBll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
        //    var data = nppbkcBll.GetAll().Where(x => x.IS_DELETED != true);
        //    return new SelectList(data, "KPPBC_ID", "CITY");
        //}

        public static SelectList GetGoodTypeGroupList()
        {
            ExGroupTypeBLL goodTypeBll = MvcApplication.GetInstance<ExGroupTypeBLL>();
            var goodTypes = goodTypeBll.GetAll().Where(x => x.Inactive = false).ToList();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(goodTypes);
            return new SelectList(selectItemSource, "ValueField", "TextField");
            //return new SelectList(goodTypes, "EXT_TYP_DESC", "EXT_TYP_DESC");
        }

        public static SelectList GetGoodTypeGroupListByDescValue()
        {
            IZaidmExGoodTypeBLL goodTypeBll = MvcApplication.GetInstance<ZaidmExGoodTypeBLL>();
            var goodTypes = goodTypeBll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(goodTypes);
            // return new SelectList(selectItemSource, "ValueField", "TextField");
            return new SelectList(goodTypes, "EXT_TYP_DESC", "EXT_TYP_DESC");
        }

        //public static SelectList GetSourcePlantList()
        //{
        //    IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
        //    var plant = plantBll.GetAll();
        //    var selectItemSource = Mapper.Map<List<SelectItemModel>>(plant);
        //    return new SelectList(selectItemSource, "ValueField", "TextField");

        //    //return new SelectList(sourcePlant, "NPPBCK_ID", "NAME1");
        //}

        public static SelectList GetPbck1CompletedList()
        {
            IPBCK1BLL pbck1 = MvcApplication.GetInstance<PBCK1BLL>();
            var input = new Pbck1GetCompletedDocumentByParamInput();
            //var data = pbck1.GetAllByParam(input);
            var data = pbck1.GetCompletedDocumentByParam(input);
            return new SelectList(data, "Pbck1Id", "Pbck1Number");
        }

        public static SelectList GetPlantImportList()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            IZaidmExNPPBKCBLL nppbkcBLL = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();

            var nppbkcList = nppbkcBLL.GetAll().Where(x=> x.IS_DELETED != true).Select(x=> x.NPPBKC_ID).ToList();
            List<T001W> plantIdList;
            plantIdList = plantBll.GetAllPlant();
            plantIdList =
                plantIdList.Where(
                    x => x.IS_DELETED != true && x.NPPBKC_IMPORT_ID != null && nppbkcList.Contains(x.NPPBKC_IMPORT_ID))
                    .OrderBy(x => x.WERKS)
                    .ToList();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(plantIdList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetPlantAll()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            List<T001W> plantIdList;
            plantIdList = plantBll.GetAllPlant();
            plantIdList =
                plantIdList.Where(
                    x => x.IS_DELETED != true && x.ZAIDM_EX_NPPBKC != null && x.ZAIDM_EX_NPPBKC.IS_DELETED != true)
                    .OrderBy(x => x.WERKS)
                    .ToList();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(plantIdList);
            return new SelectList(selectItemSource, "ValueField", "TextField");

        }

        public static SelectList GetBroleList()
        {
            IUserAuthorizationBLL userAuthorizationBll = MvcApplication.GetInstance<UserAuthorizationBLL>();
            var data = userAuthorizationBll.GetAllBRole();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static List<PageDto> GetModuleList()
        {
            IPageBLL pageBll = MvcApplication.GetInstance<PageBLL>();
            var data = pageBll.GetParentPages();
            var result = Mapper.Map<List<PageDto>>(data);
            return result;
        }

        public static SelectList GetPlantByNppbkcId(IPlantBLL plantBll, string nppbkcId)
        {
            var plantList = plantBll.GetCompositeListByNppbkcId(nppbkcId);
            return new SelectList(plantList, "WERKS", "DROPDOWNTEXTFIELD");

        }

        public static SelectList GetUsers()
        {
            IUserBLL userBll = MvcApplication.GetInstance<UserBLL>();
            var users = userBll.GetUsers();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(users);
            return new SelectList(selectItemSource, "ValueField", "TextField");

        }

        public static SelectList GetActiveSupplierPlantList()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetActivePlant();
            return new SelectList(data, "WERKS", "DROPDOWNTEXTFIELD");
        }

        public static SelectList GetAuthorizedNppbkc(List<NppbkcPlantDto> listNppbkc)
        {
            var selectItemSource = listNppbkc.Select(x => x.NppbckId);
            return new SelectList(selectItemSource);
        }
        public static SelectList GetAuthorizedPlant(List<NppbkcPlantDto> listNppbkc, string NppbckId)
        {
            var plants = new List<PlantDto>();

            if (NppbckId == null)
            {
                var items = listNppbkc.ToList();
                foreach (var item in items)
                {

                    plants.AddRange(item.Plants);
                }
                var selectItemSource = Mapper.Map<List<SelectItemModel>>(plants);
                return new SelectList(selectItemSource, "ValueField", "TextField");
            }
            else
            {
                var items = listNppbkc.Where(x => x.NppbckId == NppbckId).ToList();
                foreach (var item in items)
                {

                    plants.AddRange(item.Plants);
                }
                var selectItemSource = Mapper.Map<List<SelectItemModel>>(plants);
                return new SelectList(selectItemSource, "ValueField", "TextField");
            }


        }
        public static SelectList GetYearList()
        {
            var selectItemSource = new List<SelectItemModel>();


            for (int i = 3; i > 0; i--)
            {
                var item = new SelectItemModel();

                item.TextField = (DateTime.Now.Year - i).ToString();
                item.ValueField = (DateTime.Now.Year - i).ToString();
                selectItemSource.Add(item);
            }
            for (int i = 0; i < 5; i++)
            {
                var item = new SelectItemModel();

                item.TextField = (DateTime.Now.Year + i).ToString();
                item.ValueField = (DateTime.Now.Year + i).ToString();
                selectItemSource.Add(item);
            }

            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
        public static SelectList GetYearList(ICK5BLL ck5Bll)
        {
            var yearList = ck5Bll.GetAllYearsByGiDate();
            var selectItemSource = new List<SelectItemModel>();

            foreach (var year in yearList)
            {
                var item = new SelectItemModel();
                item.TextField = year.ToString();
                item.ValueField = year.ToString();
                selectItemSource.Add(item);
            }
            //for (int i = 3; i > 0; i--)
            //{
            //    var item = new SelectItemModel();

            //    item.TextField = (DateTime.Now.Year - i).ToString();
            //    item.ValueField = (DateTime.Now.Year - i).ToString();
            //    selectItemSource.Add(item);
            //}
            //for (int i = 0; i < 3; i++)
            //{
            //    var item = new SelectItemModel();

            //    item.TextField = (DateTime.Now.Year + i).ToString();
            //    item.ValueField = (DateTime.Now.Year + i).ToString();
            //    selectItemSource.Add(item);
            //}

            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetPbck1CompletedListByPlant(string plantId)
        {
            IPBCK1BLL pbck1 = MvcApplication.GetInstance<PBCK1BLL>();

            var data = pbck1.GetPbck1CompletedDocumentByPlant(plantId);
            return new SelectList(data, "Pbck1Id", "Pbck1Number");
        }

        public static SelectList GetPlantByCompany(string companyId,bool isReverse = false)
        {
            IT001KBLL t001Kbll = MvcApplication.GetInstance<T001KBLL>();
            var plantList = t001Kbll.GetPlantByCompany(companyId,isReverse);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(plantList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetPlantByCompanyId(string companyId)
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            List<T001W> plantIdList;
            plantIdList = plantBll.GetAllPlant();
            plantIdList =
                plantIdList.Where(
                    x => x.IS_DELETED != true && x.ZAIDM_EX_NPPBKC != null && x.ZAIDM_EX_NPPBKC.IS_DELETED != true
                    && x.T001K != null).Where(x => x.T001K.BUKRS == companyId)
                    .OrderBy(x => x.WERKS)
                    .ToList();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(plantIdList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }


        public static SelectList GetCompanyListFilter(ICompanyBLL companyBll)
        {
            var data = companyBll.GetAllData().Where(x => x.IS_DELETED != true);
            return new SelectList(data, "BUTXT", "BUTXT");
        }

        public static SelectList GetPlantByNppbkcImport(bool isNppbkcImport)
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            List<T001W> plantIdList;
            plantIdList = plantBll.GetAllPlant();
            plantIdList =
                plantIdList.Where(
                    x => x.IS_DELETED != true && x.ZAIDM_EX_NPPBKC != null && x.ZAIDM_EX_NPPBKC.IS_DELETED != true)
                    .OrderBy(x => x.WERKS)
                    .ToList();

            if(isNppbkcImport)
                plantIdList =
                    plantIdList.Where(
                        x => x.IS_DELETED != true && x.ZAIDM_EX_NPPBKC != null && x.ZAIDM_EX_NPPBKC.IS_DELETED != true && x.NPPBKC_IMPORT_ID != null)
                        .OrderBy(x => x.WERKS)
                        .ToList();

            var selectItemSource = Mapper.Map<List<SelectItemModel>>(plantIdList);
            return new SelectList(selectItemSource, "ValueField", "TextField");

        }

        public static SelectList GetFaCodeByPlant(string plantId)
        {
            IBrandRegistrationBLL brandBll = MvcApplication.GetInstance<BrandRegistrationBLL>();
            var brandCe = brandBll.GetBrandCeBylant(plantId);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(brandCe);
            return new SelectList(selectItemSource, "ValueField", "TextField");

        }

        public static SelectList GetUomStickGram(IUnitOfMeasurementBLL uomBll)
        {
            var data = uomBll.GetAll().Where(x => x.IS_DELETED != true && x.IS_EMS == true && (x.UOM_ID == "G" || x.UOM_ID == "Btg"));
            return new SelectList(data, "UOM_ID", "UOM_DESC");
        }


        public static SelectList GetCk5RefPortToImporter(ICK5BLL ck5Bll)
        {
            var data = ck5Bll.GetAllCompletedPortToImporter();
            return  new SelectList(data,"CK5_ID","SUBMISSION_NUMBER");
        }

        public static SelectList GetCountryListCodeAndName()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetAllDataCountry();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
            //return new SelectList(data, "COUNTRY_CODE", "COUNTRY_CODE");
        }


        public static SelectList GetExternalSupplierList(Enums.CK5Type ck5Type)
        {
            ICK5BLL ck5Bll = MvcApplication.GetInstance<CK5BLL>();

            var data = ck5Bll.GetExternalSupplier(ck5Type);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);

            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
    }

}

