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

        public VirtualMappingPlantController(IVirtualMappingPlantBLL vitVirtualMappingPlanBll, IMasterDataBLL masterData, IChangesHistoryBLL changeLogHistoryBLL, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _virtualMappingPlanBll = vitVirtualMappingPlanBll;
            _masterDataBll = masterData;
            _changesHistoryBLL = changeLogHistoryBLL;
            //_plantList = _masterDataBll.get;
        }

        //
        // GET: /VirtualMappingPlant/
        public ActionResult Index()
        {
            var model = new VirtualMappingPlantIndexViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            var dbData = _virtualMappingPlanBll.GetAll();
            model.Details = AutoMapper.Mapper.Map<List<VirtualMappingPlantDetail>>(dbData);
            ViewBag.Message = TempData["message"];
            return View("Index", model);
        }

        private VirtualMappingPlantCreateViewModel InitCreateModel(VirtualMappingPlantCreateViewModel model)
        {
            model.MainMenu = Enums.MenuList.MasterData;
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


        [HttpPost]
        public ActionResult Create(VirtualMappingPlantCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var dbVirtual = new VIRTUAL_PLANT_MAP();

                var dbVirtual = AutoMapper.Mapper.Map<VIRTUAL_PLANT_MAP>(model);

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
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBLL.GetByFormTypeAndFormId(Enums.MenuList.MasterData, id.ToString()));

            var dbVirtual = _virtualMappingPlanBll.GetByIdIncludeChild(id);
            model.VirtualMapId = dbVirtual.VIRTUAL_PLANT_MAP_ID;
            model.CompanyName = dbVirtual.T001.BUTXT;
            model.ImportPlanName = dbVirtual.T001W.WERKS;
            model.ExportPlanName = dbVirtual.T001W1.WERKS;
            model.IsDeleted = dbVirtual.IS_DELETED.HasValue ? dbVirtual.IS_DELETED.Value : false;
            

            return View(model);
        }

        private VirtualMappingPlantEditViewModel InitEditModel(VirtualMappingPlantEditViewModel model)
        {
            model.MainMenu = Enums.MenuList.MasterData;
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