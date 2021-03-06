﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.GOODSTYPE;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Controllers
{
    public class GoodsTypeGroupController : BaseController
    {

        private IZaidmExGoodTypeBLL _zaidmExGoodTypeBll;
        private IMasterDataBLL _masterDataBll;
        private IExGroupTypeBLL _exGroupTypeBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;

        public GoodsTypeGroupController(IZaidmExGoodTypeBLL zaidmExGoodTypeBll, IMasterDataBLL masterData, IExGroupTypeBLL exGroupTypeBll, IChangesHistoryBLL changesHistoryBll,
            IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.GoodsTypeGroup)
        {
            _zaidmExGoodTypeBll = zaidmExGoodTypeBll;
            _masterDataBll = masterData;
            _exGroupTypeBll = exGroupTypeBll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        private string GetGroupTypeByGroupName(string groupName)
        {
            //var db = _exGroupTypeBll.GetGroupTypesByName(groupName);

            //string result = db.Aggregate("", (current, exGroupType) => current + (exGroupType.ZAIDM_EX_GOODTYP.EXC_GOOD_TYP + ","));

            //if (result.Length > 0)
            //    result = result.Substring(0, result.Length - 1);

            //return result;
            return string.Empty;
        }

        //
        // GET: /ExcisableGoodsTypeGroup/
        public ActionResult Index()
        {
            var data = _exGroupTypeBll.GetAll();
            var goodsTypeGroup = new GoodsTypeGroupViewModel();
            goodsTypeGroup.MainMenu = _mainMenu;
            goodsTypeGroup.CurrentMenu = PageInfo;
            var detailFromDb = Mapper.Map<List<DetailsGoodsTypGroup>>(data);
            var distinctDetail = new List<DetailsGoodsTypGroup>();
            if (detailFromDb.Count > 0)
            {
                var groupnames = data.GroupBy(x => x.GROUP_NAME).Select(x => x.Key);


                foreach (var dd in groupnames)
                {
                    var detail = new DetailsGoodsTypGroup();
                    detail.GroupName = dd;
                    var concatName = string.Empty;
                    var dataFilterByNames = detailFromDb.Where(x => x.GroupName.Equals(dd)).ToList();
                    foreach (var d in dataFilterByNames)
                    {

                        concatName = string.Empty;
                        var names = _exGroupTypeBll.GetGoodTypeByGroup(d.GoodsTypeId);
                        int nameIndex = 0;
                        foreach (var name in names)
                        {
                            if (nameIndex > 0)
                            {
                                concatName += " , " + name;
                            }
                            else
                            {
                                concatName += name;
                            }
                            nameIndex++;
                        }

                        detail.GoodsTypeId = d.GoodsTypeId;
                        detail.Inactive = d.Inactive;
                    }
                    detail.GroupTypeName = concatName;

                    distinctDetail.Add(detail);
                }


            }

            goodsTypeGroup.Details = distinctDetail;
            goodsTypeGroup.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Controller ? true : false);
            return View("Index", goodsTypeGroup);
        }

        private GoodsTypeGroupCreateViewModel InitCreateModel(GoodsTypeGroupCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return model;
        }

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller )
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new GoodsTypeGroupCreateViewModel();

            InitCreateModel(model);

            var childDetails = _zaidmExGoodTypeBll.GetAll();

            model.Details = Mapper.Map<List<GoodsTypeDetails>>(childDetails).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GoodsTypeGroupCreateViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (_exGroupTypeBll.IsGroupNameExist(model.GroupName))
                {
                    ModelState.AddModelError("GroupName", "Group name already exist");
                    InitCreateModel(model);

                    return View("Create", model);
                }

                var listGroup = new List<EX_GROUP_TYPE_DETAILS>();

                foreach (var detail in model.Details.Where(detail => detail.IsChecked))
                {

                    var detailGroupType = new EX_GROUP_TYPE_DETAILS();
                    detailGroupType.GOODTYPE_ID = detail.GoodTypeId;
                  
                    listGroup.Add(detailGroupType);
                }
                var groupType = new EX_GROUP_TYPE();
                groupType.GROUP_NAME = model.GroupName;
                groupType.EX_GROUP_TYPE_DETAILS = listGroup;  
                if (listGroup.Count > 0)
                {

                    _exGroupTypeBll.Save(groupType);

                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                      );
                    return RedirectToAction("Index");
                }


                ModelState.AddModelError("Details", "Choose at least one type");

            }

            InitCreateModel(model);

            return View("Create", model);
        }

        public ActionResult Details(int id)
        {
            var model = new GoodsTypeGroupEditViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            // model.GroupName = groupName;

            var realChild = _exGroupTypeBll.GetById(id);
            model.GroupName = realChild.GROUP_NAME;
            var childDetails = _zaidmExGoodTypeBll.GetAll();

            model.Details = Mapper.Map<List<GoodsTypeDetails>>(childDetails).ToList();
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.GoodsTypeGroup, id.ToString()));
            foreach (var details in model.Details)
            {
                foreach (var gt in realChild.EX_GROUP_TYPE_DETAILS)
                {
                    if (details.GoodTypeId == gt.GOODTYPE_ID)
                    {
                        details.IsChecked = true;
                    }
                }


            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                return RedirectToAction("Details", new { id });
            }

            var model = new GoodsTypeGroupEditViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            // model.GroupName = groupName;

            var realChild = _exGroupTypeBll.GetById(id);
            model.Id = realChild.EX_GROUP_TYPE_ID;
            model.GroupName = realChild.GROUP_NAME;
            var childDetails = _zaidmExGoodTypeBll.GetAll();

            model.Details = Mapper.Map<List<GoodsTypeDetails>>(childDetails).ToList();

            foreach (var details in model.Details)
            {
                foreach (var gt in realChild.EX_GROUP_TYPE_DETAILS)
                {
                    if (details.GoodTypeId == gt.GOODTYPE_ID)
                    {
                        details.IsChecked = true;
                    }
                }


            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(GoodsTypeGroupEditViewModel model)
        {


            if (ModelState.IsValid)
            {
                List<EX_GROUP_TYPE_DETAILS> newDetails = new List<EX_GROUP_TYPE_DETAILS>();

                foreach (var data in model.Details.Where(x => x.IsChecked)) {
                    var detailGroupType = new EX_GROUP_TYPE_DETAILS();

                    detailGroupType.GOODTYPE_ID = data.GoodTypeId;
                    detailGroupType.EX_GROUP_TYPE_ID = model.Id;
                    newDetails.Add(detailGroupType);
                    
                }

                _exGroupTypeBll.InsertDetail(model.Id, newDetails, CurrentUser.USER_ID);
                //var realChild = _exGroupTypeBll.GetById(model.Id);
                //var listDetail = new List<EX_GROUP_TYPE_DETAILS>(realChild.EX_GROUP_TYPE_DETAILS);

                
                //var listGroup = new List<EX_GROUP_TYPE>();
                //foreach (var detail in model.Details.Where(detail => detail.IsChecked))
                //{
                //    var detailGroupType = new EX_GROUP_TYPE_DETAILS();

                //    detailGroupType.GOODTYPE_ID = detail.GoodTypeId;
                //    detailGroupType.EX_GROUP_TYPE_ID = realChild.EX_GROUP_TYPE_ID;
                //    _exGroupTypeBll.InsertDetail(detailGroupType);

                //}

                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success
                        ); return RedirectToAction("Index");


                ModelState.AddModelError("Details", "Choose at least one type");
            }

            // InitCreateModel(model);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View("Edit", model);
        }
    }
}