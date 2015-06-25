using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.GOODSTYPE;
using Sampoerna.EMS.Website.Models.VirtualMappingPlant;

namespace Sampoerna.EMS.Website.Controllers
{
    public class VirtualMappingPlantController : BaseController
    {

        private IVirtualMappingPlantBLL _virtualMappingPlanBll;
        private IMasterDataBLL _masterDataBll;

        public VirtualMappingPlantController(IVirtualMappingPlantBLL vitVirtualMappingPlanBll, IMasterDataBLL masterData, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _virtualMappingPlanBll = vitVirtualMappingPlanBll;
            _masterDataBll = masterData;
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

                return RedirectToAction("Index");
            }

            InitCreateModel(model);

            return View("Create", model);
        }

        public ActionResult Details(long id)
        {
            var model = new VirtualMappingPlantDetailsViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;


            var dbVirtual = _virtualMappingPlanBll.GetByIdIncludeChild(id);
            model.CompanyName = dbVirtual.T1001.BUKRSTXT;
            model.ImportPlanName = dbVirtual.T1001W.WERKS;
            model.ExportPlanName = dbVirtual.T1001W1.WERKS;
            

            return View(model);
        }

        
        public ActionResult Edit(int id)
        {
            var model = new VirtualMappingPlantEditViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            var dbVirtual = _virtualMappingPlanBll.GetByIdIncludeChild(id);

            model.VirtualMapId = dbVirtual.VIRTUAL_PLANT_MAP_ID;

            if (dbVirtual.COMPANY_ID.HasValue)
                model.CompanyId = dbVirtual.COMPANY_ID.Value;

            model.ImportPlantId = dbVirtual.T1001W.PLANT_ID;
            model.ExportPlantId = dbVirtual.T1001W1.PLANT_ID;

            model.CompanyNameList = GlobalFunctions.GetCompanyList();
            model.ImportPlanNameList = GlobalFunctions.GetVirtualPlantList();
            model.ExportPlanNameList = GlobalFunctions.GetVirtualPlantList();


            return View(model);
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
                    model.MainMenu = Enums.MenuList.MasterData;
                    model.CurrentMenu = PageInfo;

                    model.CompanyNameList = GlobalFunctions.GetCompanyList();
                    model.ImportPlanNameList = GlobalFunctions.GetVirtualPlantList();
                    model.ExportPlanNameList = GlobalFunctions.GetVirtualPlantList();

                    return View("Edit", model);
                }


                dbVirtual.COMPANY_ID = model.CompanyId;
                dbVirtual.IMPORT_PLANT_ID = model.ImportPlantId;
                dbVirtual.EXPORT_PLANT_ID = model.ExportPlantId;

                _virtualMappingPlanBll.Save(dbVirtual);

                return RedirectToAction("Index");
            }

            // InitCreateModel(model);
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.CompanyNameList = GlobalFunctions.GetCompanyList();
            model.ImportPlanNameList = GlobalFunctions.GetVirtualPlantList();
            model.ExportPlanNameList = GlobalFunctions.GetVirtualPlantList();

            return View("Edit", model);
        }
    }
}