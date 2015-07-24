using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
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


        
        public LACK1Controller(IPageBLL pageBll, ILACK1BLL lack1Bll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll ) 
            : base(pageBll, Enums.MenuList.LACK1)
        {
            _lack1Bll = lack1Bll;
            _monthBll = monthBll;
            _uomBll = uomBll;
            _mainMenu = Enums.MenuList.LACK1;
        }

        private List<LACK1Item> GetListByNppbkc(LACK1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var lack1Data = _lack1Bll.GetListByNpbkcParam(new Lack1GetListByNppbkcParam());
                return Mapper.Map<List<LACK1Item>>(lack1Data);
            }
            //getparams
            var input = Mapper.Map<Lack1GetListByNppbkcParam>(filter);
            var dbData = _lack1Bll.GetListByNpbkcParam(input);
            return Mapper.Map<List<LACK1Item>>(dbData);
        }

        #region Index
        //
        // GET: /LACK1/
        public ActionResult Index()
        {
            var data =  InitLack1ViewModel(new LACK1ViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                SearchInput =
                {
                    Lack1Type = Enums.LACK1Type.ListByNppbkc
                }

            });
           
            return View("Index", data);
        }

        private LACK1ViewModel InitLack1ViewModel(LACK1ViewModel model)
        {
            model.SearchInput.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
            model.SearchInput.PoaList = GlobalFunctions.GetPoaAll();//ask compare pbck1
            model.SearchInput.PlantIdList = GlobalFunctions.GetPlantAll();
            model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();
            //minus reported on

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(LACK1ViewModel model)
        {
            model.Details = GetListByNppbkc(model.SearchInput);
            return PartialView("_Lack1Table", model);
        }
        #endregion 
	}
}