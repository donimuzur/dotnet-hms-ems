using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.PBCK4;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK4Controller : BaseController
    {
        private IPOABLL _poaBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IPBCK4BLL _pbck4Bll;

        public PBCK4Controller(IPageBLL pageBLL, IPOABLL poabll, IZaidmExNPPBKCBLL nppbkcBll, IPBCK4BLL pbck4Bll)
            : base(pageBLL, Enums.MenuList.PBCK4)
        {
            _poaBll = poabll;
            _nppbkcBll = nppbkcBll;
            _pbck4Bll = pbck4Bll;

        }

        //
        // GET: /PBCK4/
        public ActionResult Index()
        {

            Pbck4IndexViewModel model;
            try
            {
                model = CreateInitModelView();


            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck4IndexViewModel();
            }

            return View(model);
        }

        private Pbck4IndexViewModel CreateInitModelView()
        {
            var model = new Pbck4IndexViewModel();

            model.MainMenu = Enums.MenuList.PBCK4;
            model.CurrentMenu = PageInfo;
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;

            //var listPbck4 = _ck5Bll.GetAll();
            //model.SearchView.DocumentNumberList = new SelectList(listCk5Dto, "SUBMISSION_NUMBER", "SUBMISSION_NUMBER");

            model.SearchView.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.SearchView.PlantIdList = GlobalFunctions.GetPlantAll();

            model.SearchView.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();

            
            //list table
            model.DetailsList = GetPbck4Items();

            return model;
        }

        private List<Pbck4Item> GetPbck4Items(Pbck4SearchViewModel filter = null)
        {
            Pbck4GetByParamInput input;
            List<Pbck4Dto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Pbck4GetByParamInput();
                
                dbData = _pbck4Bll.GetPbck4ByParam(input);
                return Mapper.Map<List<Pbck4Item>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<Pbck4GetByParamInput>(filter);

            dbData = _pbck4Bll.GetPbck4ByParam(input);
            return Mapper.Map<List<Pbck4Item>>(dbData);
        }

        [HttpPost]
        public PartialViewResult Filter(Pbck4IndexViewModel model)
        {
            model.DetailsList = GetPbck4Items(model.SearchView);
            return PartialView("_Pbck4OpenListDocuments", model);
        }
	}
}