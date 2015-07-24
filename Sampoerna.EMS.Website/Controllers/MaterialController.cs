using System;
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

        public MaterialController(IPageBLL pageBLL,IMaterialBLL materialBll, IChangesHistoryBLL changesHistoryBll) : base(pageBLL, Enums.MenuList.MaterialMaster){
            _materialBll = materialBll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        private MaterialCreateViewModel InitCreateModel(MaterialCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetVirtualPlantListMultiSelect();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();
            model.BaseUOM = GlobalFunctions.GetUomList();
            model.ConversionUomList = GlobalFunctions.GetConversionUomList();
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
        public ActionResult Details(string mn, string p)
        {

            var model = new MaterialDetailViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var data = _materialBll.getByID(mn, p);
            Mapper.Map(data,model);
            
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MaterialMaster, mn+p));
            model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();
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
            model.ConversionUomList = GlobalFunctions.GetConversionUomList();
            return model;
        }

        private MaterialDetailViewModel InitDetailModel(MaterialDetailViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


             
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
        private void SetChanges(MaterialEditViewModel origin, ZAIDM_EX_MATERIAL data)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("MATERIAL_DESC", origin.MaterialDesc.Equals(data.MATERIAL_DESC));
            changesData.Add("PURCHASING_GROUP", origin.PurchasingGroup.Equals(data.PURCHASING_GROUP));
            changesData.Add("MATERIAL_GROUP", origin.MaterialGroup.Equals(data.MATERIAL_GROUP));
            changesData.Add("BASE_UOM", origin.UomId.Equals(data.BASE_UOM_ID));
            changesData.Add("ISSUE_STORANGE_LOC", origin.IssueStorageLoc.Equals(data.ISSUE_STORANGE_LOC));
            changesData.Add("EX_GOODTYP", origin.GoodTypeId.Equals(data.EXC_GOOD_TYP));

            
            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                        FORM_ID = data.STICKER_CODE+data.WERKS,
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = CurrentUser.USER_ID,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                       
                        case "MATERIAL_DESC":
                            changes.OLD_VALUE = origin.MaterialDesc;
                            changes.NEW_VALUE = data.MATERIAL_DESC;
                            break;
                        case "PURCHASING_GROUP":
                            changes.OLD_VALUE = origin.PurchasingGroup;
                            changes.NEW_VALUE = data.PURCHASING_GROUP;
                            break;
                        case "MATERIAL_GROUP":
                            changes.OLD_VALUE = origin.MaterialGroup;
                            changes.NEW_VALUE = data.MATERIAL_GROUP;
                            break;

                        case "BASE_UOM":
                            changes.OLD_VALUE = origin.UomId;
                            changes.NEW_VALUE = data.BASE_UOM_ID;
                            break;
                        case "ISSUE_STORANGE_LOC":
                            changes.OLD_VALUE = origin.IssueStorageLoc;
                            changes.NEW_VALUE = data.ISSUE_STORANGE_LOC;
                            break;
                        case "EX_GOODTYP":
                            changes.OLD_VALUE = origin.GoodTypeId;
                            changes.NEW_VALUE = data.EXC_GOOD_TYP;
                            break;
                        
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        
       //  POST: /Material/Create
        [HttpPost]
        public ActionResult Create(MaterialCreateViewModel data)
        {

            try
            {
                // TODO: Add insert logic here
                //if (ModelState.IsValid)
                //{
                    var plantIds = data.PlantId;
                    foreach (var plant in plantIds)
                    {
                        var model = Mapper.Map<ZAIDM_EX_MATERIAL>(data);
                  

                        model.WERKS = plant;
                        foreach (var uom in model.MATERIAL_UOM)
                        {
                            uom.STICKER_CODE = model.STICKER_CODE;
                            uom.WERKS = model.WERKS;

                        }
                        model.CREATED_BY = CurrentUser.USER_ID;
                        model.CREATED_DATE = DateTime.Now;
                        var  output = _materialBll.Save(model, CurrentUser.USER_ID);
                        if (!output.Success)
                        {
                            TempData[Constans.SubmitType.Save] = output.ErrorMessage;
                        }
                        else
                        {
                            TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                        }
                        
                    }
                return RedirectToAction("Index");    
                //}

                //return RedirectToAction("Create"); 
                
            }
            catch(Exception ex)
            {
                InitCreateModel(data);
                return View(data);
                
            }
        }

        //
        // GET: /Material/Edit/5
        public ActionResult Edit(string mn, string p)
        {
            var data = _materialBll.getByID(mn, p);
            
            

            if (data.IS_FROM_SAP)
            {
             
                return RedirectToAction("Details", new {mn=mn, p=p});
            }
            else {

                var model = Mapper.Map<MaterialEditViewModel>(data);
                model.MainMenu = Enums.MenuList.MasterData;
                model.CurrentMenu = PageInfo;
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, mn+p));
                model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();

                InitEditModel(model);

                return View("Edit", model);
            }

            
        }

        //
        // POST: /Material/Edit/5
        [HttpPost]
        public ActionResult Edit(MaterialEditViewModel model)
        {
            try
            {
                // TODO: Add update logic here
               
                    var data = _materialBll.getByID(model.MaterialNumber, model.PlantId);
                    
                    model.ChangedById = CurrentUser.USER_ID;
                    model.ChangedDate = DateTime.Now;
                    //model.ChangesHistoryList =  Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id))
                    if (data == null) {
                       return RedirectToAction("Index");
                    }

                    foreach (var matUom in model.MaterialUom)
                    {
                        var uom = new MATERIAL_UOM();
                        uom.STICKER_CODE = matUom.MaterialNumber;
                        uom.WERKS = matUom.Plant;
                        uom.UMREN = matUom.Umren;
                        uom.UMREZ = matUom.Umrez;
                        uom.MEINH = matUom.Meinh;

                        _materialBll.SaveUoM(uom);
                    }
                    var origin = AutoMapper.Mapper.Map<MaterialEditViewModel>(data);
                    AutoMapper.Mapper.Map(model, data);
                    data.MODIFIED_BY = CurrentUser.USER_ID;
                    data.MODIFIED_DATE = DateTime.Now;
                    data.CREATED_DATE = origin.CreatedDate;
                    data.CREATED_BY = origin.CreatedById;
                    
                    SetChanges(origin, data);
                    var output = _materialBll.Save(data,CurrentUser.USER_ID);
                    if (!output.Success)
                    {
                        TempData[Constans.SubmitType.Update] = output.ErrorMessage;
                    }
                    else
                    {
                        TempData[Constans.SubmitType.Update] = Constans.SubmitMessage.Updated;
                    }


               
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        

        //
        // POST: /Material/Delete/5
        
        public ActionResult Delete(string mn, string p)
        {
            try
            {
                // TODO: Add delete logic here
                _materialBll.Delete(mn, p, CurrentUser.USER_ID);
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
