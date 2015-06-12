using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class VirtualMappingPlantController : BaseController
    {

        private IVirtualMappingPlanBLL _virtualMappingPlanBll;

        public VirtualMappingPlantController(IVirtualMappingPlanBLL vitVirtualMappingPlanBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.VitualMappingPlant)
        {
            _virtualMappingPlanBll = vitVirtualMappingPlanBll;
        }

        //
        // GET: /VirtualMappingPlant/
        public ActionResult Index()
        {
            var virtualMappingPlant = new VirtualMappingPlantViewModel();
            virtualMappingPlant.Details = _virtualMappingPlanBll.GetAll().Select(AutoMapper.Mapper.Map<VirtualMappingPlantDetail>).ToList();


            return View("Index", virtualMappingPlant);
        }
    }
}