using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ProductType;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ProductTypeController : BaseController
    {
         private Enums.MenuList _mainMenu;
         private IZaidmExProdTypeBLL _exProdTypeBll;


         public ProductTypeController(IZaidmExProdTypeBLL exProdTypeBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.ProductType)
        {
            _exProdTypeBll = exProdTypeBll;
            _mainMenu = Enums.MenuList.MasterData;
             
        }

        //
        // GET: /ProductType/
        public ActionResult Index()
        {
            var model = new ProductTypeIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.ListProductTypes = Mapper.Map<List<ProductTypeFormViewModel>>(_exProdTypeBll.GetAll());
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer);

            return View("Index", model);
        }

        public ActionResult Edit(string id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }

            var model = new ProductTypeFormViewModel();

            try
            {
                model = Mapper.Map<ProductTypeFormViewModel>(_exProdTypeBll.GetById(id));
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View("Edit", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ProductTypeFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var input = new ProductTypeSaveInput();
                    input.ProductType = Mapper.Map<ZAIDM_EX_PRODTYP>(model);
                    input.UserId = CurrentUser.USER_ID;

                    _exProdTypeBll.UpdateProductType(input);

                    AddMessageInfo("Success update Product Type", Enums.MessageInfoType.Success);
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    AddMessageInfo("Update Failed : " + ex.Message, Enums.MessageInfoType.Error);
                }
            }

            model = new ProductTypeFormViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View("Edit", model);
        }

        public ActionResult Detail(string id)
        {
            var model = new ProductTypeFormViewModel();

            try
            {
                model = Mapper.Map<ProductTypeFormViewModel>(_exProdTypeBll.GetById(id));
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;
            }

            return View("Detail", model);
        }


	}
}