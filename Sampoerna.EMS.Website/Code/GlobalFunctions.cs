using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Antlr.Runtime;
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
        
        public static SelectList GetCreatorList()
        {
            IUserBLL userBll = MvcApplication.GetInstance<UserBLL>();
            var users = userBll.GetUsers(new UserInput());
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(users);
            return new SelectList(selectItemSource, "ValueField", "TextField");
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
    }
}