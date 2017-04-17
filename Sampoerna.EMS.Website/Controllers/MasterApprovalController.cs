using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using iTextSharp.text.pdf.qrcode;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.MasterDataApproval;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MasterApprovalController : BaseController
    {
        //
        // GET: /MasterApproval/
        private Enums.MenuList _mainMenu;
        private IMasterDataAprovalBLL _masterDataAprovalBLL;
        private IUserBLL _userBLL;
        public MasterApprovalController(IPageBLL pageBll,IMasterDataAprovalBLL masterDataAprovalBLL,IUserBLL userBLL) : base(pageBll, Enums.MenuList.MasterDataApproval)
        {
            _mainMenu = Enums.MenuList.MasterData;
            _masterDataAprovalBLL = masterDataAprovalBLL;
            _userBLL = userBLL;
        }

        public ActionResult Index()
        {
            var model = new MasterDataApprovalIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            var data = _masterDataAprovalBLL.GetList();
            model.Details = Mapper.Map<List<MasterDataApprovalDetailViewModel>>(data);
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = new MasterDataApprovalItemViewModel();
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;

            var data = _masterDataAprovalBLL.GetByApprovalId(id);

            model.Detail = Mapper.Map<MasterDataApprovalDetailViewModel>(data);
            model.IsMasterApprover = _userBLL.IsUserMasterApprover(CurrentUser.USER_ID);
            return View(model);
        }

        [HttpPost]
        public ActionResult Approve(MasterDataApprovalItemViewModel model)
        {
            try
            {
                _masterDataAprovalBLL.Approve(CurrentUser.USER_ID, model.Detail.APPROVAL_ID);
                AddMessageInfo("Success", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            
            return RedirectToAction("Detail", new { id = model.Detail.APPROVAL_ID });
        }
    }
}