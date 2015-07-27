using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.LACK1;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK1Controller : BaseController
    {
        private ILACK1BLL _lack1Bll;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;
        private Enums.MenuList _mainMenu;



        public LACK1Controller(IPageBLL pageBll, ILACK1BLL lack1Bll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll)
            : base(pageBll, Enums.MenuList.LACK1)
        {
            _lack1Bll = lack1Bll;
            _monthBll = monthBll;
            _uomBll = uomBll;
            _mainMenu = Enums.MenuList.LACK1;
        }


        #region Index
        //
        // GET: /LACK1/
        public ActionResult Index()
        {
            var data = InitLack1ViewModel(new Lack1IndexViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Type = Enums.LACK1Type.ListByNppbkc,
                
                Details = Mapper.Map<List<NppbkcData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput()))

            });

            return View("Index", data);
        }

        private Lack1IndexViewModel InitLack1ViewModel(Lack1IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
            model.PoaList = GlobalFunctions.GetPoaAll();//ask compare pbck1
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();
            //minus reported on

            return model;
        }

        private List<NppbkcData> GetListByNppbkc(Lack1IndexViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var litsByNppbkc = _lack1Bll.GetAllByParam(new Lack1GetByParamInput());
                return Mapper.Map<List<NppbkcData>>(litsByNppbkc);
            }
            //get by param
            var input = Mapper.Map<Lack1GetByParamInput>(filter);
            var dbData = _lack1Bll.GetAllByParam(input);
            
            return  Mapper.Map<List<NppbkcData>>(dbData);

        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(Lack1IndexViewModel model)
        {
            model.Details = GetListByNppbkc(model);
            return PartialView("_Lack1Table", model);
        }

        #endregion
    }
}