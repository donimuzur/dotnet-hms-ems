using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.Reversal;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ReversalController : BaseController
    {
        private IReversalBLL _reversalBll;
        private Enums.MenuList _mainMenu;

        public ReversalController(IPageBLL pageBll, IReversalBLL reversalBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _reversalBll = reversalBll;
            _mainMenu = Enums.MenuList.CK4C;
        }

        #region Index

        public ActionResult Index()
        {
            var data = InitIndexViewModel(new ReversalIndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.Reversal,
                ProductionDate = DateTime.Today.ToString("dd MMM yyyy"),
                IsShowNewButton = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
            });

            return View("Index", data);
        }

        private ReversalIndexViewModel InitIndexViewModel(
            ReversalIndexViewModel model)
        {
            model.PlantWerksList = GlobalFunctions.GetPlantAll();

            //model.Detail = GetOpenDocument(model);

            return model;
        }

        #endregion
    }
}