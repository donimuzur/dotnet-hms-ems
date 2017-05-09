using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using iTextSharp.text.pdf.qrcode;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.MasterDataApproval;
using System.Configuration;
using Sampoerna.EMS.XMLReader;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MasterApprovalController : BaseController
    {
        //
        // GET: /MasterApproval/
        private Enums.MenuList _mainMenu;
        private IMasterDataAprovalBLL _masterDataAprovalBLL;
        private IBrandRegistrationBLL _brandRegistrationBLL;
        private XmlBrandRegistrationWriter _xmlWriter;
        private IUserBLL _userBLL;
        public MasterApprovalController(IPageBLL pageBll,IMasterDataAprovalBLL masterDataAprovalBLL,IBrandRegistrationBLL brandRegistrationBLL,IUserBLL userBLL) : base(pageBll, Enums.MenuList.MasterDataApproval)
        {
            _mainMenu = Enums.MenuList.MasterData;
            _masterDataAprovalBLL = masterDataAprovalBLL;
            
            _xmlWriter = new XmlBrandRegistrationWriter();
            _userBLL = userBLL;
        }

        public ActionResult Index()
        {
            var model = new MasterDataApprovalIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            var data = _masterDataAprovalBLL.GetList();
            model.Details = Mapper.Map<List<MasterDataApprovalDetailViewModel>>(data);
            model.DocumentStatus = Enums.DocumentStatus.WaitingForMasterApprover;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(MasterDataApprovalIndexViewModel model)
        {
            
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            var data = _masterDataAprovalBLL.GetList(model.DocumentStatus);
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
                GenerateXml(model.Detail);

                AddMessageInfo("Success", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            
            return RedirectToAction("Detail", new { id = model.Detail.APPROVAL_ID });
        }

        private void GenerateXml(MasterDataApprovalDetailViewModel data)
        {
            if (data.PAGE_ID == (int)Enums.MenuList.BrandRegistration)
            {
                var tempId = data.FORM_ID.Split('-');
                var werks = tempId[0];
                var facode = tempId[1];
                var stickerCode = tempId[2];

                var brandXmlDto = _brandRegistrationBLL.GetDataForXml(werks, facode, stickerCode);
                if (brandXmlDto != null)
                {
                    
                        
                        var fileName = ConfigurationManager.AppSettings["PathXmlTemp"] + "BRANDREG" +
                           DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml";
                        var outboundFilePath = ConfigurationManager.AppSettings["CK5PathXml"] + "BRANDREG" +
                           DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml";
                        brandXmlDto.XmlPath = fileName;

                        _xmlWriter.CreateBrandRegXml(brandXmlDto);

                        _xmlWriter.MoveTempToOutbound(fileName, outboundFilePath);
                    


                }




            }


        }

        public ActionResult Reject(MasterDataApprovalItemViewModel model)
        {
            try
            {
                _masterDataAprovalBLL.Reject(CurrentUser.USER_ID, model.Detail.APPROVAL_ID);
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