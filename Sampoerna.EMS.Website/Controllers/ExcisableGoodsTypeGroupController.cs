using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.GOODSTYPE;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ExcisableGoodsTypeGroupController : BaseController
    {

        private IZaidmExGoodTypeBLL _goodsTypeGroupBLL;
        private IMasterDataBLL _masterDataBll;

        public ExcisableGoodsTypeGroupController(IZaidmExGoodTypeBLL goodsTypeGroupBLL, IMasterDataBLL masterData, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _goodsTypeGroupBLL = goodsTypeGroupBLL;
            _masterDataBll = masterData;   
        }

        private string GetGroupTypeByGroupName(string groupName)
        {
            var db = _goodsTypeGroupBLL.GetGroupTypesByName(groupName);

            string result = db.Aggregate("", (current, exGroupType) => current + (exGroupType.ZAIDM_EX_GOODTYP.EXT_TYP_DESC + ","));
            if (result.Length > 0)
                result = result.Substring(0, result.Length - 1);
            return result;

            //db.Aggregate("", (current, exGroupType) => current + (exGroupType.ZAIDM_EX_GOODTYP.EXT_TYP_DESC + ","));
        }

        //
        // GET: /ExcisableGoodsTypeGroup/
        public ActionResult Index()
        {

            var goodsTypeGroup = new GoodsTypeGroupViewModel();
            goodsTypeGroup.MainMenu = Enums.MenuList.MasterData;
            goodsTypeGroup.CurrentMenu = PageInfo;
        
            var dbGroup = _goodsTypeGroupBLL.GetAllGroup().Select(type => type.GROUP_NAME).Distinct();
            foreach (var group in dbGroup)
            {
                var details = new DetailsGoodsTypGroup();
                details.GroupName = group;
                details.GroupTypeName = GetGroupTypeByGroupName(details.GroupName);
                goodsTypeGroup.Details.Add(details);
            }
           
            //goodsTypeGroup.Details = Mapper.Map<List<DetailsGoodsTypGroup>>(dbGroup);
           
            return View("Index", goodsTypeGroup);
        }

        private GoodsTypeGroupCreateViewModel InitCreateModel(GoodsTypeGroupCreateViewModel model)
        {
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            return model;
        }
        public ActionResult Create()
        {
            var model = new GoodsTypeGroupCreateViewModel();

            InitCreateModel(model);

            var childDetails = _goodsTypeGroupBLL.GetAllChildName();

            model.Details = Mapper.Map<List<GoodsTypeDetails>>(childDetails).ToList();
           
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GoodsTypeGroupCreateViewModel model)
        {

            if (ModelState.IsValid)
            {

                var listGroup = new List<EX_GROUP_TYPE>();
               
                foreach (var detail in model.Details.Where(detail => detail.IsChecked))
                {
                   
                    var groupType = new EX_GROUP_TYPE();
                    groupType.GROUP_NAME = model.GroupName;
                    groupType.EX_GOODTYP_ID = detail.GoodTypeId;
                    listGroup.Add(groupType);
                }

                if (listGroup.Count > 0)
                {
                    _goodsTypeGroupBLL.SaveGroup(listGroup);

                    return RedirectToAction("Index");
                }
            }

            InitCreateModel(model);

            return View("Create", model);
        }
    }
}