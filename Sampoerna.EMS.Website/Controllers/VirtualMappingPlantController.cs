using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.VirtualMappingPlant;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Controllers
{
    public class VirtualMappingPlantController : BaseController
    {

        private IVirtualMappingPlantBLL _virtualMappingPlanBll;
        private IMasterDataBLL _masterDataBll;
        private IChangesHistoryBLL _changesHistoryBLL;
        //private List<AutoCompletePlant> _plantList;
        private Enums.MenuList _mainMenu;
       

        public VirtualMappingPlantController(IVirtualMappingPlantBLL vitVirtualMappingPlanBll, IMasterDataBLL masterData, IChangesHistoryBLL changeLogHistoryBLL, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.VirtualMappingPlant)
        {
            _virtualMappingPlanBll = vitVirtualMappingPlanBll;
            _masterDataBll = masterData;
            _changesHistoryBLL = changeLogHistoryBLL;
            //_plantList = _masterDataBll.get;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /VirtualMappingPlant/
        public ActionResult Index()
        {
            var model = new VirtualMappingPlantIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var dbData = _virtualMappingPlanBll.GetAll();
            model.Details = AutoMapper.Mapper.Map<List<VirtualMappingPlantDetail>>(dbData);
            ViewBag.Message = TempData["message"];
            return View("Index", model);
        }

        private VirtualMappingPlantCreateViewModel InitCreateModel(VirtualMappingPlantCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.CompanyNameList = GlobalFunctions.GetCompanyList();
            model.ImportPlanNameList = GlobalFunctions.GetVirtualPlantList();
            model.ExportPlanNameList = GlobalFunctions.GetVirtualPlantList();
            
            return model;
        }

        public ActionResult Create()
        {
            var model = new VirtualMappingPlantCreateViewModel();

            InitCreateModel(model);

            return View("Create", model);
        }
        private void SetChanges(VIRTUAL_PLANT_MAP origin, VirtualMappingPlantEditViewModel data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("COMPANY_ID", origin.COMPANY_ID.Equals(data.CompanyId));
            changesData.Add("EXPORT_PLANT", origin.EXPORT_PLANT_ID.Equals(data.ExportPlantId));
            changesData.Add("IMPORT_PLANT",origin.IMPORT_PLANT_ID.Equals(data.ImportPlantId));
          
            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.VirtualMappingPlant,
                        FORM_ID = data.VirtualMapId.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "COMPANY_ID":
                            changes.OLD_VALUE = origin.COMPANY_ID ;
                            changes.NEW_VALUE = data.CompanyId;
                            break;
                        case "EXPORT_PLANT":
                            changes.OLD_VALUE = origin.EXPORT_PLANT_ID;
                            changes.NEW_VALUE = data.ExportPlantId;
                            break;
                        case "IMPORT_PLANT":
                            changes.OLD_VALUE = origin.IMPORT_PLANT_ID;
                            changes.NEW_VALUE = data.ImportPlantId;
                            break;
                        
                    }
                    _changesHistoryBLL.AddHistory(changes);
                }
            }
        }


        [HttpPost]
        public ActionResult Create(VirtualMappingPlantCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var dbVirtual = new VIRTUAL_PLANT_MAP();

                var dbVirtual = AutoMapper.Mapper.Map<VIRTUAL_PLANT_MAP>(model);
                dbVirtual.CREATED_DATE = DateTime.Now;
                dbVirtual.CREATED_BY = CurrentUser.USER_ID;
                _virtualMappingPlanBll.Save(dbVirtual);
                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                return RedirectToAction("Index");
            }

            InitCreateModel(model);

            return View("Create", model);
        }

        public ActionResult Details(int id)
        {
            var model = new VirtualMappingPlantDetailsViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBLL.GetByFormTypeAndFormId(Enums.MenuList.MasterData, id.ToString()));

            var dbVirtual = _virtualMappingPlanBll.GetByIdIncludeChild(id);
            model.VirtualMapId = dbVirtual.VIRTUAL_PLANT_MAP_ID;
            model.CompanyName = dbVirtual.T001.BUTXT;
            model.ImportPlanName = dbVirtual.T001W.WERKS;
            model.ExportPlanName = dbVirtual.T001W1.WERKS;
            model.ImportPlantDesc = dbVirtual.T001W.WERKS + "-" + dbVirtual.T001W.NAME1;
            model.ExportPlantDesc = dbVirtual.T001W1.WERKS + "-" + dbVirtual.T001W1.NAME1;
            

            //model.IsDeleted = dbVirtual.IS_DELETED.HasValue ? dbVirtual.IS_DELETED.Value : false;
            var changeHistoryList = _changesHistoryBLL.GetByFormTypeId(Enums.MenuList.VirtualMappingPlant);
           
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
         

            return View(model);
        }

        private VirtualMappingPlantEditViewModel InitEditModel(VirtualMappingPlantEditViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.CompanyNameList = GlobalFunctions.GetCompanyList();
            model.ImportPlanNameList = GlobalFunctions.GetVirtualPlantList();
            model.ExportPlanNameList = GlobalFunctions.GetVirtualPlantList();

            return model;
        }

        
        public ActionResult Edit(int id)
        {
            
            var model = new VirtualMappingPlantEditViewModel();
           
            InitEditModel(model);

            var dbVirtual = _virtualMappingPlanBll.GetByIdIncludeChild(id);
           
            if (dbVirtual != null)
            {
                if (dbVirtual.IS_DELETED == true)
                {
                    var modeldetail = new VirtualMappingPlantDetailsViewModel();
                    modeldetail.MainMenu = _mainMenu;
                    modeldetail.CurrentMenu = PageInfo;
                    modeldetail.VirtualMapId = dbVirtual.VIRTUAL_PLANT_MAP_ID;

                    if (!string.IsNullOrEmpty(dbVirtual.COMPANY_ID))
                        modeldetail.CompanyName = dbVirtual.T001.BUTXT;

                    modeldetail.ImportPlanName = dbVirtual.T001W.WERKS;
                    modeldetail.ExportPlanName = dbVirtual.T001W1.WERKS;

                    
                    return View("Details",modeldetail);
                }
                else {
                    model.VirtualMapId = dbVirtual.VIRTUAL_PLANT_MAP_ID;

                    if (!string.IsNullOrEmpty(dbVirtual.COMPANY_ID))
                        model.CompanyId = dbVirtual.COMPANY_ID;

                    model.ImportPlantId = dbVirtual.T001W.WERKS;
                    model.ExportPlantId = dbVirtual.T001W1.WERKS;

                    return View(model);
                }
                
                

                
            }
            else
            {
                //model.VirtualMapId = 0;
                //model.ImportPlantId = 0;
                //model.ExportPlantId = 0;
                //ModelState.AddModelError("Exception", "Data Not Found");
                throw new HttpException(403, "Data not found");
            }
            
        }

        [HttpPost]
        public ActionResult Edit(VirtualMappingPlantEditViewModel model)
        {

            if (ModelState.IsValid)
            {

                var dbVirtual = _virtualMappingPlanBll.GetById(model.VirtualMapId);

                if (dbVirtual == null)
                {
                    ModelState.AddModelError("Details", "Data Not Found");
                    InitEditModel(model);

                    return View("Edit", model);
                }
                var origin = AutoMapper.Mapper.Map<VIRTUAL_PLANT_MAP>(dbVirtual);
                SetChanges(origin, model, CurrentUser.USER_ID);
                dbVirtual.COMPANY_ID = model.CompanyId;
                dbVirtual.IMPORT_PLANT_ID = model.ImportPlantId;
                dbVirtual.EXPORT_PLANT_ID = model.ExportPlantId;

                _virtualMappingPlanBll.Save(dbVirtual);
                TempData[Constans.SubmitType.Update] = Constans.SubmitMessage.Updated;
                return RedirectToAction("Index");
            }

           
            InitEditModel(model);
            return View("Edit", model);
        }

        public ActionResult Delete(int id)
        {
            _virtualMappingPlanBll.Delete(id, CurrentUser.USER_ID);
            TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
            return RedirectToAction("Index");
        }

        //public ActionResult PlantList() { 
        //    return Json(_plantList , JsonRequestBehavior.AllowGet);
        //}
    }
}