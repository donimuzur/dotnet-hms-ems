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
    public class GoodsTypeGroupController : BaseController
    {

        private IZaidmExGoodTypeBLL _zaidmExGoodTypeBll;
        private IMasterDataBLL _masterDataBll;
        private IExGroupType _exGroupTypeBll;

        public GoodsTypeGroupController(IZaidmExGoodTypeBLL zaidmExGoodTypeBll, IMasterDataBLL masterData, IPageBLL pageBLL, IExGroupType exGroupTypeBll)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _zaidmExGoodTypeBll = zaidmExGoodTypeBll;
            _masterDataBll = masterData;
            _exGroupTypeBll = exGroupTypeBll;
        }

        private string GetGroupTypeByGroupName(string groupName)
        {
            var db = _exGroupTypeBll.GetGroupTypesByName(groupName);

            string result = db.Aggregate("", (current, exGroupType) => current + (exGroupType.ZAIDM_EX_GOODTYP.EXT_TYP_DESC + ","));
           
            if (result.Length > 0)
                result = result.Substring(0, result.Length - 1);
            
            return result;
        }

        //
        // GET: /ExcisableGoodsTypeGroup/
        public ActionResult Index()
        {

            var goodsTypeGroup = new GoodsTypeGroupViewModel();
            goodsTypeGroup.MainMenu = Enums.MenuList.MasterData;
            goodsTypeGroup.CurrentMenu = PageInfo;

            var dbGroup = _exGroupTypeBll.GetGroupByGroupName();
            foreach (var group in dbGroup)
            {
                var details = new DetailsGoodsTypGroup();
                details.GroupName = group;
                details.GroupTypeName = GetGroupTypeByGroupName(details.GroupName);
                goodsTypeGroup.Details.Add(details);
            }
           
           
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

            var childDetails = _zaidmExGoodTypeBll.GetAll();

            model.Details = Mapper.Map<List<GoodsTypeDetails>>(childDetails).ToList();
           
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GoodsTypeGroupCreateViewModel model)
        {

            if (model.SubmitType == Core.Constans.SubmitType.Cancel)
                return RedirectToAction("Index");
          

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
                    _exGroupTypeBll.SaveGroup(listGroup);

                    return RedirectToAction("Index");
                }


                ModelState.AddModelError("Details", "Choose at least one type");

            }

            InitCreateModel(model);

            return View("Create", model);
        }

        public ActionResult Details(string groupName)
        {
            var model = new GoodsTypeGroupDetailsViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.GroupName = groupName;

            var childDetails = _exGroupTypeBll.GetGroupTypesByName(groupName);

            model.Details = Mapper.Map<List<GoodsTypeDetails>>(childDetails).ToList();

            foreach (var details in model.Details)
            {
                details.IsChecked = true;
            }
            return View(model);
        }

        public ActionResult Edit(string groupName)
        {
            var model = new GoodsTypeGroupEditViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.GroupName = groupName;

            var realChild = _exGroupTypeBll.GetGroupTypesByName(groupName);

            var childDetails = _zaidmExGoodTypeBll.GetAll();

            model.Details = Mapper.Map<List<GoodsTypeDetails>>(childDetails).ToList();

            foreach (var details in model.Details)
            {
                foreach (var exGroupType in realChild)
                {
                    if (details.GoodTypeId == exGroupType.EX_GOODTYP_ID)
                        details.IsChecked = true;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(GoodsTypeGroupEditViewModel model)
        {

            if (model.SubmitType == Core.Constans.SubmitType.Cancel)
                return RedirectToAction("Index");

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
                    _exGroupTypeBll.UpdateGroupByGroupName(listGroup, model.GroupName);

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("Details", "Choose at least one type");
            }

           // InitCreateModel(model);
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            return View("Edit", model);
        }
    }
}