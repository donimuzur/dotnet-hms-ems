using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.PBCK7;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK7Controller : BaseController
    {
        private IPBCK7BLL _pbck7Bll;
        private IBACK1BLL _back1Bll;
        private Enums.MenuList _mainMenu;
        public PBCK7Controller(IPageBLL pageBll, IPBCK7BLL pbck7Bll, IBACK1BLL back1Bll)
            : base(pageBll, Enums.MenuList.PBCK7)
        {
            _pbck7Bll = pbck7Bll;
            _back1Bll = back1Bll;
            _mainMenu = Enums.MenuList.PBCK7;
        }

        #region Index
        //
        // GET: /PBCK7/
        public ActionResult Index()
        {
            var data = InitPbck7ViewModel(new Pbck7IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck7Type = Enums.Pbck7Type.Pbck7List,

                Detail = Mapper.Map<List<DataListIndexPbck7>>(_pbck7Bll.GetAllByParam(new Pbck7Input()))
            });

            return View("Index",data);
        }
        #endregion

        private Pbck7IndexViewModel InitPbck7ViewModel(Pbck7IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll();
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GlobalFunctions.GetPoaAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;

        }

        [HttpPost]
        public PartialViewResult FilterListPbck7IndexViewResult(Pbck7IndexViewModel model)
        {
            var input = Mapper.Map<Pbck7Input>(model);

            var dbData = _pbck7Bll.GetAllByParam(input);

            var result = Mapper.Map<List<DataListIndexPbck7>>(dbData);

            var viewModel = new Pbck7IndexViewModel();
            viewModel.Detail = result;
            
            return PartialView("_Pbck7TableIndex",viewModel);
        }

        [HttpPost]
        public JsonResult PoaAndPlantListPartial(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(nppbkcId);
            var model = new Pbck7IndexViewModel() { PoaList = listPoa, PlantList = listPlant };
            return Json(model);
        }
    }
}