using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.NPPBKC;

namespace Sampoerna.EMS.Website.Controllers
{
    public class NPPBKCController : BaseController
    {
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IMasterDataBLL _masterDataBll;

        public NPPBKCController(IZaidmExNPPBKCBLL nppbkcBll, IMasterDataBLL masterData, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterData;   
        }
        
        //
        // GET: /NPPBKC/
        public ActionResult Index()
        {
            var nppbkc = new NPPBKCViewModels();
            nppbkc.MainMenu = Enums.MenuList.MasterData;
            nppbkc.CurrentMenu = PageInfo;

            nppbkc.Details = Mapper.Map<List<DetailNppbck>>(_nppbkcBll.GetAll());

            return View("Index", nppbkc);
            
        }
	}
}