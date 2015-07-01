using System.Collections.Generic;
using System.Linq;
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
            IZaidmExPOABLL poaBll = MvcApplication.GetInstance<ZaidmExPOABLL>();
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

        public static SelectList GetKppBcCityList()
        {
            IZaidmExNPPBKCBLL nppbkcbll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
            var nppbkcList = nppbkcbll.GetAll();
                
            //var selectItemSource = Mapper.Map<List<SelectItemModel>>(nppbkcList);
            return new SelectList(nppbkcList, "KPPBC_ID", "City");
        }

        public static SelectList GetGoodTypeGroupList()
        {
            IZaidmExGoodTypeBLL goodTypeBll = MvcApplication.GetInstance<ZaidmExGoodTypeBLL>();
            var goodTypes = goodTypeBll.GetAll();
            return new SelectList(goodTypes, "GOODTYPE_ID", "EXT_TYP_DESC");
        }

        public static SelectList GetExciseSettlementList()
        {
            IMasterDataBLL masterDataBll = MvcApplication.GetInstance<MasterDataBLL>();
            var exciseSettlements = masterDataBll.GetAllExciseExSettlements();
            return new SelectList(exciseSettlements, "EX_SETTLEMENT_ID", "EX_SETTLEMENT_NAME");
        }

        public static SelectList GetExciseStatusList()
        {
            IMasterDataBLL masterDataBll = MvcApplication.GetInstance<MasterDataBLL>();
            var exciseStatus = masterDataBll.GetAllExciseStatus();
            return new SelectList(exciseStatus, "EX_STATUS_ID", "EX_STATUS_NAME");
        }

        public static SelectList GetRequestTypeList()
        {
            IMasterDataBLL masterDataBll = MvcApplication.GetInstance<MasterDataBLL>();
            var requestType = masterDataBll.GetAllRequestTypes();
            return new SelectList(requestType, "REQUEST_TYPE_ID", "REQUEST_TYPE_NAME");
        }

        public static SelectList GetSourcePlantList()
        {
            IMasterDataBLL masterDataBll = MvcApplication.GetInstance<MasterDataBLL>();
            var sourcePlant = masterDataBll.GetAllSourcePlants();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(sourcePlant);
            return new SelectList(selectItemSource, "ValueField", "TextField");

            //return new SelectList(sourcePlant, "NPPBCK_ID", "NAME1");
        }

        public static SelectList GetCarriageMethodList()
        {
            IMasterDataBLL masterDataBll = MvcApplication.GetInstance<MasterDataBLL>();
            var carriageMethod = masterDataBll.GetAllCarriageMethods();
            return new SelectList(carriageMethod, "CARRIAGE_METHOD_ID", "CARRIAGE_METHOD_NAME");
        }

        public static SelectList GetSupplierPortList()
        {
            ISupplierPortBLL supplierPortBll = MvcApplication.GetInstance<SupplierPortBLL>();
            var data = supplierPortBll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
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
            return new SelectList(data, "UOM_ID", "UOM_NAME");
        }

        public static SelectList GetPbck1CompletedList()
        {
            IPBCK1BLL pbck1 = MvcApplication.GetInstance<PBCK1BLL>();
            var input = new PBCK1Input();
            var data = pbck1.GetPBCK1ByParam(input);
            return new SelectList(data, "PBCK1_ID", "NUMBER");
        }
        
        public static ZAIDM_EX_NPPBKC GetNppbkcById(long id)
        {
            IZaidmExNPPBKCBLL nppbkcbll = MvcApplication.GetInstance<ZaidmExNPPBKCBLL>();
            return nppbkcbll.GetById(id);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
        
        
        public static SelectList GetCompanyList()
        {
            ICompanyBLL companyBll = MvcApplication.GetInstance<CompanyBLL>();
            var data = companyBll.GetAllData();
            return new SelectList(data, "COMPANY_ID", "BUKRSTXT");
        }

        public static SelectList GetVirtualPlantList()
        {
            IPlantBLL plantBll = MvcApplication.GetInstance<PlantBLL>();
            var data = plantBll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModelVirtualPlant>>(data);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
    }
}