using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.Excel;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.PLANT;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PlantController : BaseController
    {
        private IPlantBLL _plantBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;

        public PlantController(IPlantBLL plantBll, IMasterDataBLL masterData, IZaidmExNPPBKCBLL nppbkcBll, IZaidmExGoodTypeBLL goodTypeBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _plantBll = plantBll;
            _nppbkcBll = nppbkcBll;
            _goodTypeBll = goodTypeBll;
        }

        //
        // GET: /Plant/
        public ActionResult Index()
        {
            var plant = new PlantViewModel
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
                Details = _plantBll.GetAll()
            };

            ViewBag.Message = TempData["message"];
            return View("Index", plant);

        }

        public ActionResult Edit(long id)
        {
            var plant = _plantBll.GetId(id);

            if (plant == null)
            {
                return HttpNotFound();
            }

            var detail = AutoMapper.Mapper.Map<DetailPlantT1001W>(plant);

            var model = new PlantFormModel
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
                Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_NO", plant.NPPBCK_ID),
                PlantIdListItems = new SelectList(_plantBll.GetAll(), "PLANT_ID", "WERKS", plant.PLANT_ID),
                Detail = detail
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(PlantFormModel model)
        {

            if (!ModelState.IsValid)
            {
                var plant = _plantBll.GetId(model.Detail.PlantId);
                model.MainMenu = Enums.MenuList.MasterData;
                model.CurrentMenu = PageInfo;

                var detail = AutoMapper.Mapper.Map<DetailPlantT1001W>(plant);
                model.Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_NO", plant.NPPBCK_ID);
                model.PlantIdListItems = new SelectList(_plantBll.GetAll(), "PLANT_ID", "WERKS", plant.PLANT_ID);
                //model.RecieveMaterialListItems = new SelectList(_goodTypeBll.GetAll(), "GOODTYPE_ID", "EXT_TYP_DESC", plant.RECEIVED_MATERIAL_TYPE_ID);

                model.Detail = detail;
                return View("Edit", model);
            }
            try
            {
                var plantId = model.Detail.PlantId;
                var plant = _plantBll.GetId(plantId);
                AutoMapper.Mapper.Map(model.Detail, plant);

                _plantBll.save(plant);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }


        }
        public ActionResult Detail(int id)
        {
            var plant = _plantBll.GetId(id);

            if (plant == null)
            {
                return HttpNotFound();
            }
            var model = new PlantFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            var detail = AutoMapper.Mapper.Map<DetailPlantT1001W>(plant);
            model.Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_NO", plant.NPPBCK_ID);
            model.PlantIdListItems = new SelectList(_plantBll.GetAll(), "PLANT_ID", "WERKS", plant.PLANT_ID);
            //model.RecieveMaterialListItems = new SelectList(_goodTypeBll.GetAll(), "GOODTYPE_ID", "EXT_TYP_DESC", plant.RECEIVED_MATERIAL_TYPE_ID);

            model.Detail = detail;
            model.Detail.IsNo = !model.Detail.IsMainPlant;
            model.Detail.IsYes = model.Detail.IsMainPlant;
            return View(model);

        }
    }
}