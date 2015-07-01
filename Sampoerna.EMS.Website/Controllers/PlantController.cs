using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.PlantReceiveMaterial;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PlantController : BaseController
    {
        private IPlantBLL _plantBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private Enums.MenuList _mainMenu;
        private IChangesHistoryBLL _changesHistoryBll;

        public PlantController(IPlantBLL plantBll, IZaidmExNPPBKCBLL nppbkcBll, IZaidmExGoodTypeBLL goodTypeBll, IChangesHistoryBLL changesHistoryBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterPlant)
        {
            _plantBll = plantBll;
            _nppbkcBll = nppbkcBll;
            _goodTypeBll = goodTypeBll;
            _mainMenu = Enums.MenuList.MasterData;
            _changesHistoryBll = changesHistoryBll;
        }

        //
        // GET: /Plant/
        public ActionResult Index()
        {
            var plant = new PlantViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<DetailPlantT1001W>>(_plantBll.GetAll())
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
            
            var detail = Mapper.Map<DetailPlantT1001W>(plant);

            var model = new PlantFormModel
            {
                Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_NO", plant.NPPBCK_ID),
                Detail = detail
            };
            return InitialEdit(model);
        }

        public ActionResult InitialEdit(PlantFormModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_NO", model.Detail.NPPBCK_ID);
            model.Detail.ReceiveMaterials = GetPlantReceiveMaterial(model.Detail);
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(PlantFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return InitialEdit(model);
            }
            try
            {
                var receiveMaterial = model.Detail.ReceiveMaterials.Where(c => c.IsChecked).ToList();
                model.Detail.ReceiveMaterials = receiveMaterial;
                var t1001w = Mapper.Map<Plant>(model.Detail);
                _plantBll.save(t1001w, CurrentUser.USER_ID);

                return RedirectToAction("Index");
            }
            catch
            {
                return InitialEdit(model);
            }
        }
        public ActionResult Detail(int id)
        {
            var plant = _plantBll.GetId(id);

            if (plant == null)
            {
                return HttpNotFound();
            }

            var detail = Mapper.Map<DetailPlantT1001W>(plant);

            var model = new PlantFormModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_NO", plant.NPPBCK_ID),
                Detail = detail
            };

            model.Detail.IsNo = !model.Detail.IsMainPlant;
            model.Detail.IsYes = model.Detail.IsMainPlant;
            model.Detail.ReceiveMaterials = GetPlantReceiveMaterial(model.Detail);
            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MasterPlant, id));
            
            return View(model);

        }

        private List<PlantReceiveMaterialItemModel> GetPlantReceiveMaterial(DetailPlantT1001W plant)
        {
            var goodTypes = _goodTypeBll.GetAll();
            var rc = (from x in goodTypes
                select new PlantReceiveMaterialItemModel()
                {
                    PLANT_ID = plant.PlantId,
                    PLANT_MATERIAL_ID = 0,
                    EXC_GOOD_TYP = x.EXC_GOOD_TYP, 
                    GOODTYPE_ID = x.GOODTYPE_ID,
                    EXT_TYP_DESC = x.EXT_TYP_DESC, 
                    IsChecked = false
                }).ToList();
            if (plant.ReceiveMaterials != null && plant.ReceiveMaterials.Count > 0)
            {
                foreach (var plantReceiveMaterialItemModel in rc)
                {
                    var isFound = plant.ReceiveMaterials.FirstOrDefault(c => c.GOODTYPE_ID == plantReceiveMaterialItemModel.GOODTYPE_ID);
                    if (isFound != null)
                    {
                        plantReceiveMaterialItemModel.IsChecked = true;
                    }
                }
            }
            return rc;
        }
        
    }
}