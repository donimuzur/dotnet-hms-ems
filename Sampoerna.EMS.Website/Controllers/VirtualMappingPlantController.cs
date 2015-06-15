using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class VirtualMappingPlantController : BaseController
    {

        private IVirtualMappingPlanBLL _virtualMappingPlanBll;
        private IMasterDataBLL _masterDataBll;

        public VirtualMappingPlantController(IVirtualMappingPlanBLL vitVirtualMappingPlanBll, IMasterDataBLL masterData, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _virtualMappingPlanBll = vitVirtualMappingPlanBll;
            _masterDataBll = masterData;
        }

        //
        // GET: /VirtualMappingPlant/
        public ActionResult Index()
        {
            var virtualMappingPlant = new VirtualMappingPlantViewModel();
            virtualMappingPlant.MainMenu = Enums.MenuList.MasterData;
            virtualMappingPlant.CurrentMenu = PageInfo;
            //DropDown
            virtualMappingPlant.CompanyList = new SelectList(_masterDataBll.GetDataCompany().Select(x => new SelectListItem() { Text = x, Value = x })
                 .ToList(), "Value", "Text");
            virtualMappingPlant.Details= _virtualMappingPlanBll.GetAll().Select(AutoMapper.Mapper.Map<SaveVirtualMappingPlantOutput>).ToList();

            
            return View("Index", virtualMappingPlant);
        }
    }
}