using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public PlantController(IPlantBLL plantBll, IMasterDataBLL masterData,IZaidmExNPPBKCBLL nppbkcBll,IZaidmExGoodTypeBLL goodTypeBll, IPageBLL pageBLL)
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
            var plantT1001W = new PlantViewModel();
            plantT1001W.MainMenu = Enums.MenuList.MasterData;
            plantT1001W.CurrentMenu = PageInfo;

            plantT1001W.Details = _plantBll.GetAll().Select(AutoMapper.Mapper.Map<DetailPlantT1001W>).ToList();

            return View("Index", plantT1001W);
        }

        public ActionResult Edit(long id )
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
            model.RecieveMaterialListItems = new SelectList(_goodTypeBll.GetAll(), "GOODTYPE_ID", "EXT_TYP_DESC", plant.RECEIVED_MATERIAL_TYPE_ID);
            
            model.Detail = detail;
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(PlantFormModel model)
        {

            if (!ModelState.IsValid)
                return View();

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
            model.RecieveMaterialListItems = new SelectList(_goodTypeBll.GetAll(), "GOODTYPE_ID", "EXT_TYP_DESC", plant.RECEIVED_MATERIAL_TYPE_ID);

            model.Detail = detail;
            return View(model);

        }
	}
}