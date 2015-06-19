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
using Sampoerna.EMS.Website.Models.CK5;


namespace Sampoerna.EMS.Website.Controllers
{
    public class CK5Controller : BaseController
    {
        private ICK5BLL _ck5Bll;

        public CK5Controller(IPageBLL pageBLL, ICK5BLL ck5Bll)
            : base(pageBLL, Enums.MenuList.CK5)
        {
            _ck5Bll = ck5Bll;
        }


        private List<CK5Item> GetCk5Items(Enums.CK5Type ck5Type,CK5SearchViewModel filter = null)
        {
            CK5Input input;

            if (filter == null)
            {
                //Get All
                input = new CK5Input {Ck5Type = ck5Type.ToString()};
                return Mapper.Map<List<CK5Item>>(_ck5Bll.GetCK5ByParam(input));
            }
            //getbyparams

            input = Mapper.Map<CK5Input>(filter);
            input.Ck5Type = ck5Type.ToString();
            return Mapper.Map<List<CK5Item>>(_ck5Bll.GetCK5ByParam(input));
        }

        private CK5IndexViewModel CreateInitModel(Enums.MenuList menulist, Enums.CK5Type ck5Type)
        {
            var model = new CK5IndexViewModel();

            model.MainMenu = menulist;
            model.CurrentMenu = PageInfo;
            model.Ck5Type = ck5Type;

            var dbCk5 = _ck5Bll.GetAll();
            model.SearchView.DocumentNumberList = new SelectList(dbCk5, "CK5_NUMBER", "CK5_NUMBER");
            model.SearchView.POAList = GlobalFunctions.GetPoaAll();
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();

            //null ?
            var sourcePlant = dbCk5.Select(c => c.T1001W.ZAIDM_EX_NPPBKC).ToList().Distinct();
            var destinationPlant = dbCk5.Select(c => c.T1001W1.ZAIDM_EX_NPPBKC).ToList().Distinct();

            model.SearchView.NPPBKCOriginList = new SelectList(sourcePlant, "NPPBKC_ID", "NPPBKC_NO");
            model.SearchView.NPPBKCDestinationList = new SelectList(destinationPlant, "NPPBKC_ID", "NPPBKC_NO");

            model.DetailsList = GetCk5Items(ck5Type);
            if (ck5Type == Enums.CK5Type.Domestic)
            {
                model.DetailList2 = GetCk5Items(Enums.CK5Type.Intercompany);
            }
            else if (ck5Type == Enums.CK5Type.PortToImporter)
                model.DetailList2 = GetCk5Items(Enums.CK5Type.ImporterToPlant);

            return model;
        }

        //
        // GET: /CK5/
        public ActionResult Index()
        {
            var model = CreateInitModel(Enums.MenuList.CK5, Enums.CK5Type.Domestic);
            return View(model);
        }

        [HttpPost]
        public PartialViewResult Intercompany(CK5IndexViewModel model)
        {
            //only use by domestic and importer

            Enums.CK5Type ck5Type= Enums.CK5Type.Domestic;

            if (model.Ck5Type == Enums.CK5Type.Domestic)
                ck5Type = Enums.CK5Type.Intercompany;
            else if (model.Ck5Type == Enums.CK5Type.PortToImporter)
                ck5Type = Enums.CK5Type.ImporterToPlant;

            model.DetailList2 = GetCk5Items(ck5Type, model.SearchView);
            return PartialView("_CK5IntercompanyTablePartial", model);
        }

        [HttpPost]
        public PartialViewResult Filter(CK5IndexViewModel model)
        {
            model.DetailsList = GetCk5Items(model.Ck5Type, model.SearchView);
            return PartialView("_CK5TablePartial", model);
        }

        public ActionResult CK5Manual()
        {
            var model = CreateInitModel(Enums.MenuList.CK5, Enums.CK5Type.Manual);
            return View(model);
        }

        public ActionResult CK5Export()
        {
            var model = CreateInitModel(Enums.MenuList.CK5, Enums.CK5Type.Export);
            return View(model);
        }

        public ActionResult CK5DomesticAlcohol()
        {
            var model = CreateInitModel(Enums.MenuList.CK5, Enums.CK5Type.DomesticAlcohol);
            return View(model);
        }

        public ActionResult CK5Completed()
        {
            var model = CreateInitModel(Enums.MenuList.CK5, Enums.CK5Type.Completed);
            return View(model);
        }

        public ActionResult CK5Import()
        {
            var model = CreateInitModel(Enums.MenuList.CK5, Enums.CK5Type.PortToImporter);
            return View("CK5Import",model);
        }
    }
}