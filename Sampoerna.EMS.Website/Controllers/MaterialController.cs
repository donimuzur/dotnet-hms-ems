using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Material;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MaterialController : BaseController
    {
        private IMaterialBLL _materialBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IUnitOfMeasurementBLL _unitOfMeasurementBll;
        public MaterialController(IPageBLL pageBLL, IUnitOfMeasurementBLL unitOfMeasurementBll, IZaidmExGoodTypeBLL goodTypeBll, IMaterialBLL materialBll, IChangesHistoryBLL changesHistoryBll) : base(pageBLL, Enums.MenuList.MaterialMaster){
            _materialBll = materialBll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
            _goodTypeBll = goodTypeBll;
            _unitOfMeasurementBll = unitOfMeasurementBll;
        }

        private MaterialCreateViewModel InitCreateModel(MaterialCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetVirtualPlantListMultiSelect();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.BaseUOM = GlobalFunctions.GetUomList(_unitOfMeasurementBll);
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
            //Mapper.Map(data,model);
            model = Mapper.Map<MaterialDetailViewModel>(data);
            
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MaterialMaster, mn+p));
            model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();
            model = InitDetailModel(model);

            if (model.IsDeleted.HasValue && model.IsDeleted.Value)
            {
                model.IsAllowDelete = false;
            }
            else
            {
                if (model.IsFromSap.HasValue && model.IsFromSap.Value)
                {
                    model.IsAllowDelete = false;
                }
                else
                {
                    model.IsAllowDelete = true;
                }
            }

            return View("Details",model);
        }
         

        private MaterialEditViewModel InitEditModel(MaterialEditViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.BaseUOM = GlobalFunctions.GetUomList(_unitOfMeasurementBll);
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
                        if (model.MATERIAL_UOM != null)
                        {
                            foreach (var uom in model.MATERIAL_UOM)
                            {
                                uom.STICKER_CODE = model.STICKER_CODE;
                                uom.WERKS = model.WERKS;
                                uom.MEINH = HttpUtility.UrlDecode(uom.MEINH);
                            }
                        }
                        model.CREATED_BY = CurrentUser.USER_ID;
                        model.CREATED_DATE = DateTime.Now;
                        var  output = _materialBll.Save(model, CurrentUser.USER_ID);
                        if (!output.Success)
                        {
                            AddMessageInfo(output.ErrorMessage, Enums.MessageInfoType.Error
                                );

                        }
                        else
                        {
                            AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                       );
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
               
                var dataexist = _materialBll.getByID(model.MaterialNumber, model.PlantId);
                    
                    
                if (dataexist == null)
                {
                    return RedirectToAction("Index");
                }

                if (model.MaterialUom != null)
                {
                    foreach (var matUom in model.MaterialUom)
                    {
                        var uom = new MATERIAL_UOM();
                        uom.STICKER_CODE = model.MaterialNumber;
                        uom.WERKS = model.PlantId;
                        uom.UMREN = matUom.Umren;
                        uom.UMREZ = matUom.Umrez;
                        uom.MEINH = HttpUtility.UrlDecode(matUom.Meinh);

                        _materialBll.SaveUoM(uom, CurrentUser.USER_ID);
                    }
                }

                var data = AutoMapper.Mapper.Map<ZAIDM_EX_MATERIAL>(model);
                    
                var output = _materialBll.Save(data, CurrentUser.USER_ID);
                if (!output.Success)
                {
                    AddMessageInfo(output.ErrorMessage, Enums.MessageInfoType.Error
                        );

                }
                else
                {
                    AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
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
            catch(Exception ex)
            {
                return RedirectToAction("Detail", new { mn = mn, p=p});
            }
        }

        [HttpPost]
        public JsonResult RemoveMaterialUom(int materialUomId, string materialnumber, string plant)
        {
            return Json(_materialBll.DeleteMaterialUom(materialUomId, CurrentUser.USER_ID, materialnumber, plant));
        }
    }
}
