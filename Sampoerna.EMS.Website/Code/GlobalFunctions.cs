using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Code
{
    public class GlobalFunctions
    {
        public static SelectList GetPoaAll()
        {
            POABLL poaBll = MvcApplication.GetInstance<POABLL>();
            var poaList =  poaBll.GetAll();
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
        
        public static SelectList GetCreatorList(object selectedValue=null)
        {
            IUserBLL userBll = MvcApplication.GetInstance<UserBLL>();
            var users = userBll.GetUsers(new UserInput());
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(users);
            return new SelectList(selectItemSource, "ValueField", "TextField", selectedValue );
        }

        public static SelectList GetNppbkcAll()
        {
            IZaidmExNPPBKCBLL nppbkcbll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
            var nppbkcList = nppbkcbll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(nppbkcList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static ZAIDM_EX_NPPBKC GetNppbkcById(string id)
        {
            IZaidmExNPPBKCBLL nppbkcbll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
            return nppbkcbll.GetById(id);
        }

        public static SelectList GetSupplierPortList()
        {
            ISupplierPortBLL supplierPortBll = MvcApplication.GetInstance<SupplierPortBLL>();
            var data = supplierPortBll.GetAll();
            return new SelectList(data, "SUPPLIER_PORT_ID", "PORT_NAME");
        }

        public static SelectList GetMonthList()
        {
            IMonthBLL monthBll = MvcApplication.GetInstance<MonthBLL>();
            var data = monthBll.GetAll();
            return new SelectList(data, "MONTH_ID", "MONTH_NAME_ENG");
        }

        public static SelectList GetSupplierPlantList()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetGoodTypeList()
        {
            IZaidmExGoodTypeBLL goodTypeBll = MvcApplication.GetInstance<ZaidmExGoodTypeBLL>();
            var data = goodTypeBll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        public static SelectList GetUomList()
        {
            IUnitOfMeasurementBLL uomBll = MvcApplication.GetInstance<UnitOfMeasurementBLL>();
            var data = uomBll.GetAll();
            return new SelectList(data, "UOM_ID", "UOM_DESC");
        }

        public static SelectList GetCompanyList()
        {
            ICompanyBLL companyBll = MvcApplication.GetInstance<CompanyBLL>();
            var data = companyBll.GetAllData();
            return new SelectList(data, "BUKRS", "BUTXT");
        }

        public static SelectList GetVirtualPlantList()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModelVirtualPlant>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
        public static MultiSelectList GetVirtualPlantListMultiSelect()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetAll();
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
            var data = masterBll.GetDataPersonalization();
            return new SelectList(data, "PER_CODE", "PER_DESC");
        }

        public static SelectList GetProductCodeList()
        {
            IZaidmExProdTypeBLL productBll = MvcApplication.GetInstance<ZaidmExProdTypeBLL>();
            var data = productBll.GetAll();
            //var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(data, "PROD_CODE", "PRODUCT_TYPE");
        }

        public static SelectList GetSeriesCodeList()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetAllDataSeries();
            return new SelectList(data, "SERIES_CODE", "SERIES_VALUE");
        }

        public static SelectList GetMarketCodeList()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetAllDataMarket();
            return new SelectList(data, "MARKET_ID", "MARKET_DESC");
        }

        public static SelectList GetCountryList()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetAllDataCountry();
            return new SelectList(data);
        }

        public static SelectList GetCurrencyList()
        {
            IMasterDataBLL masterBll = MvcApplication.GetInstance<MasterDataBLL>();
            var data = masterBll.GetAllDataCurrency();
            return new SelectList(data);
        }

        public static SelectList GetStickerCodeList()
        {
            IMaterialBLL materialBll = MvcApplication.GetInstance<MaterialBLL>();
            var data = materialBll.getAll();
            return new SelectList(data, "STICKER_CODE", "STICKER_CODE");
        }

        public static SelectList GetConversionUomList()
        {
            IUnitOfMeasurementBLL uomBll = MvcApplication.GetInstance<UnitOfMeasurementBLL>();
            var data = uomBll.GetAll();
            return new SelectList(data, "UOM_ID", "UOM_ID");
        }

     }
}