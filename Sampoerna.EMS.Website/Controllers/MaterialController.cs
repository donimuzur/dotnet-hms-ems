﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Material;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MaterialController : BaseController
    {
        private IMaterialBLL _materialBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;

        public MaterialController(IPageBLL pageBLL,IMaterialBLL materialBll, IChangesHistoryBLL changesHistoryBll) : base(pageBLL, Enums.MenuList.MasterData){
            _materialBll = materialBll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        private MaterialCreateViewModel InitCreateModel(MaterialCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            
            model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();
            model.BaseUOM = GlobalFunctions.GetUomList();
            return model;
        }

        //
        // GET: /Material/
        public ActionResult Index()
        {
            var model = new MaterialListViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var data = _materialBll.getAll();
            model.Details = AutoMapper.Mapper.Map<List<MaterialDetails>>(data);
            ViewBag.Message = TempData["message"];
            return View("Index", model);
            
        }

        //
        // GET: /Material/Details/5
        public ActionResult Details(string id)
        {

            var model = new MaterialDetailViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var data = _materialBll.getByID(id);
            Mapper.Map(data,model);
            
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MaterialMaster, id));
            InitDetailModel(model);
            return View("Details",model);
        }

        private MaterialEditViewModel InitEditModel(MaterialEditViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();
            model.BaseUOM = GlobalFunctions.GetUomList();
            return model;
        }

        private MaterialDetailViewModel InitDetailModel(MaterialDetailViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();
            model.BaseUOM = GlobalFunctions.GetUomList();
            return model;
        }

        //
        // GET: /Material/Create
        public ActionResult Create()
        {
            var model = new MaterialCreateViewModel();
            InitCreateModel(model);
            return View(model);
        }
        //private void SetChanges(MaterialEditViewModel origin, ZAIDM_EX_MATERIAL data, string userId)
        //{
        //    var changesData = new Dictionary<string, bool>();
        //    changesData.Add("MATERIAL_NUMBER", origin.MaterialNumber.Equals(data.STICKER_CODE));
        //    changesData.Add("PLANT_ID", origin.PlantId.Equals(data.WERKS));
        //    changesData.Add("MATERIAL_DESC", origin.MaterialDesc.Equals(data.MATERIAL_DESC));
        //    changesData.Add("PURCHASING_GROUP", origin.PurchasingGroup.Equals(data.PURCHASING_GROUP));
        //    changesData.Add("MATERIAL_GROUP", origin.MaterialGroup.Equals(data.MATERIAL_GROUP));
        //   // changesData.Add("BASE_UOM", origin.BaseUOM.SelectedValue.Equals(data.BASE_UOM_ID));
        //    changesData.Add("ISSUE_STORANGE_LOC", origin.IssueStorageLoc.Equals(data.ISSUE_STORANGE_LOC));
        //   //changesData.Add("EX_GOODTYP", origin.EXC_GOOD_TYP.Equals(data.EXC_GOOD_TYP));

        //    //changesData.Add("IS_DELETED", origin.IS_DELETED.Equals(data.IS_DELETED));
        //    //changesData.Add("HEADER_FOOTER_FORM_MAP", origin.HEADER_FOOTER_FORM_MAP.Equals(poa.HEADER_FOOTER_FORM_MAP));

        //    foreach (var listChange in changesData)
        //    {
        //        if (!listChange.Value)
        //        {
        //            var changes = new CHANGES_HISTORY
        //            {
        //                FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
        //                FORM_ID = data.STICKER_CODE.ToString(),
        //                FIELD_NAME = listChange.Key,
        //                MODIFIED_BY = userId,
        //                MODIFIED_DATE = DateTime.Now
        //            };
        //            switch (listChange.Key)
        //            {
        //                case "MATERIAL_NUMBER":
        //                    changes.OLD_VALUE = origin.STICKER_CODE;
        //                    changes.NEW_VALUE = data.STICKER_CODE;
        //                    break;
        //                case "PLANT_ID":
        //                    changes.OLD_VALUE = origin.WERKS;
        //                    changes.NEW_VALUE = data.WERKS;
        //                    break;
        //                case "MATERIAL_DESC":
        //                    changes.OLD_VALUE = origin.MATERIAL_DESC;
        //                    changes.NEW_VALUE = data.MATERIAL_DESC;
        //                    break;
        //                case "PURCHASING_GROUP":
        //                    changes.OLD_VALUE = origin.PURCHASING_GROUP;
        //                    changes.NEW_VALUE = data.PURCHASING_GROUP;
        //                    break;
        //                case "MATERIAL_GROUP":
        //                    changes.OLD_VALUE = origin.MATERIAL_GROUP;
        //                    changes.NEW_VALUE = data.MATERIAL_GROUP;
        //                    break;

        //                //case "BASE_UOM":
        //                //    changes.OLD_VALUE = origin.BASE_UOM_ID.ToString();
        //                //    changes.NEW_VALUE = data.BASE_UOM_ID.ToString();
        //                //    break;
        //                case "ISSUE_STORANGE_LOC":
        //                    changes.OLD_VALUE = origin.ISSUE_STORANGE_LOC;
        //                    changes.NEW_VALUE = data.ISSUE_STORANGE_LOC;
        //                    break;
        //                case "EX_GOODTYP":
        //                    changes.OLD_VALUE = origin.EXC_GOOD_TYP.ToString();
        //                    changes.NEW_VALUE = data.EXC_GOOD_TYP.ToString();
        //                    break;
        //                case "IS_DELETED":
        //                    changes.OLD_VALUE = origin.IS_DELETED.HasValue ? origin.IS_DELETED.Value.ToString() : "NULL";
        //                    changes.NEW_VALUE = data.IS_DELETED.HasValue ? data.IS_DELETED.Value.ToString() : "NULL";
        //                    break;
        //            }
        //            _changesHistoryBll.AddHistory(changes);
        //        }
        //    }
        //}

        //
        // POST: /Material/Create
        [HttpPost]
        public ActionResult Create(MaterialCreateViewModel data)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    var model = Mapper.Map<ZAIDM_EX_MATERIAL>(data);
                    model.CREATED_BY = CurrentUser.USER_ID;
                    model.CREATED_DATE = DateTime.Now;
                    MaterialOutput output = _materialBll.Save(model,CurrentUser.USER_ID);

                    TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                    return RedirectToAction("Index");    
                }

                InitCreateModel(data);
                return View(data);
                
            }
            catch(Exception ex)
            {
                InitCreateModel(data);
                return View(data);
                
            }
        }

        //
        // GET: /Material/Edit/5
        public ActionResult Edit(string id)
        {
            var data = _materialBll.getByID(id);
            
            

            if (data.IS_FROM_SAP)
            {
                var model = Mapper.Map<MaterialDetailViewModel>(data);
                model.MainMenu = Enums.MenuList.MasterData;
                model.CurrentMenu = PageInfo;
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id.ToString()));
                model.MaterialNumber = id;
                //InitEditModel(model);

                return View("Details", model);
            }
            else {
                var model = Mapper.Map<MaterialEditViewModel>(data);
                model.MainMenu = Enums.MenuList.MasterData;
                model.CurrentMenu = PageInfo;
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id.ToString()));
                model.MaterialNumber = id;
                InitEditModel(model);

                return View("Edit", model);
            }

            
        }

        //
        // POST: /Material/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, MaterialEditViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    var data = _materialBll.getByID(id);
                    
                    model.ChangedById = CurrentUser.USER_ID;
                    model.ChangedDate = DateTime.Now;
                    //model.ChangesHistoryList =  Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id))
                    if (data == null) {
                        ModelState.AddModelError("Details", "Data Not Found");
                        InitEditModel(model);

                        return View("Edit", model);
                    }

                    //data = Mapper.Map<ZAIDM_EX_MATERIAL>(model);
                    //data.MATERIAL_ID = id;

                    if (!data.IS_FROM_SAP) {
                        data.MATERIAL_DESC = model.MaterialDesc;
                        data.BASE_UOM_ID = model.UomId;
                        data.EXC_GOOD_TYP = model.GoodTypeId;
                        data.ISSUE_STORANGE_LOC = model.IssueStorageLoc;
                        data.MATERIAL_DESC = model.MaterialDesc;
                        data.MATERIAL_GROUP = model.MaterialGroup;
                        data.STICKER_CODE = model.MaterialNumber;
                        data.WERKS = model.PlantId;
                        data.PURCHASING_GROUP = model.PurchasingGroup;
                        //data.CHANGED_BY = CurrentUser.USER_ID;
                        //data.CHANGED_DATE = DateTime.Now;
                    }
                    
                    _materialBll.Save(data,CurrentUser.USER_ID);
                }
                TempData[Constans.SubmitType.Update] = Constans.SubmitMessage.Updated;
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                InitEditModel(model);
                return View(model);
            }
        }

        

        //
        // POST: /Material/Delete/5
        
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                _materialBll.Delete(id, CurrentUser.USER_ID);
                TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
