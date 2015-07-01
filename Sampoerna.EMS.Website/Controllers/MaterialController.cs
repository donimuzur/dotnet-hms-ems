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

        public MaterialController(IPageBLL pageBLL,IMaterialBLL materialBll, IChangesHistoryBLL changesHistoryBll) : base(pageBLL, Enums.MenuList.MaterialMaster){
            _materialBll = materialBll;
            _changesHistoryBll = changesHistoryBll;
        }

        private MaterialCreateViewModel InitCreateModel(MaterialCreateViewModel model)
        {
            model.MainMenu = Enums.MenuList.MaterialMaster;
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
            model.MainMenu = Enums.MenuList.MaterialMaster;
            model.CurrentMenu = PageInfo;

            var data = _materialBll.getAll();
            model.Details = AutoMapper.Mapper.Map<List<MaterialDetails>>(data);

            return View("Index", model);
            
        }

        //
        // GET: /Material/Details/5
        public ActionResult Details(long id)
        {

            var model = new MaterialDetailViewModel();
            model.MainMenu = Enums.MenuList.MaterialMaster;
            model.CurrentMenu = PageInfo;

            var data = _materialBll.getByID(id);
            Mapper.Map(data,model);
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MaterialMaster, id));

            return View("Details",model);
        }

        private MaterialEditViewModel InitEditModel(MaterialEditViewModel model)
        {
            model.MainMenu = Enums.MenuList.MaterialMaster;
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
                    MaterialOutput output = _materialBll.Save(model);
                    return RedirectToAction("Index");    
                }

                InitCreateModel(data);
                return View(data);
                
            }
            catch
            {
                InitCreateModel(data);
                return View(data);
                
            }
        }

        //
        // GET: /Material/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _materialBll.getByID(id);
            
            var model = Mapper.Map<MaterialEditViewModel>(data);
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id));
            model.MaterialId = id;
            InitEditModel(model);
            return View(model);
        }

        //
        // POST: /Material/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, MaterialEditViewModel model)
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

                    if (!data.IS_FROM_SAP.Value) {
                        data.MATERIAL_DESC = model.MaterialDesc;
                        data.BASE_UOM = model.UomId;
                        data.EX_GOODTYP = model.GoodTypeId;
                        data.ISSUE_STORANGE_LOC = model.IssueStorageLoc;
                        data.MATERIAL_DESC = model.MaterialDesc;
                        data.MATERIAL_GROUP = model.MaterialGroup;
                        data.MATERIAL_NUMBER = model.MaterialNumber;
                        data.PLANT_ID = model.PlantId;
                        data.PURCHASING_GROUP = model.PurchasingGroup;
                        
                    }
                    
                    _materialBll.Save(data);
                }
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
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
