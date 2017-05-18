using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using iTextSharp.text.pdf.qrcode;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.MasterDataApproval;
using System.Configuration;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
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
        private IWorkflowHistoryBLL _workflowHistoryBLL;
        private IPOABLL _poaBLL;
        private XmlBrandRegistrationWriter _xmlWriter;
        private IUserBLL _userBLL;
        //private IPageBLL _pageBll;
        public MasterApprovalController(IPageBLL pageBll,IWorkflowHistoryBLL workflowHistoryBLL,IMasterDataAprovalBLL masterDataAprovalBLL,IPOABLL poaBLL,IBrandRegistrationBLL brandRegistrationBLL,IUserBLL userBLL) : base(pageBll, Enums.MenuList.MasterDataApproval)
        {
            _mainMenu = Enums.MenuList.MasterData;
            _masterDataAprovalBLL = masterDataAprovalBLL;
            
            _xmlWriter = new XmlBrandRegistrationWriter();
            _userBLL = userBLL;
            _poaBLL = poaBLL;
            //_pageBll = pageBll;
            _workflowHistoryBLL = workflowHistoryBLL;
            _brandRegistrationBLL = brandRegistrationBLL;
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
            var workflowHistory = _workflowHistoryBLL.GetByFormTypeAndFormId(new GetByFormTypeAndFormIdInput()
            {
                FormId = data.APPROVAL_ID,
                FormType = Enums.FormType.MasterDataApproval
            });
            
            model.WorkflowHistory = new List<WorkflowHistoryViewModel>();
            model.WorkflowHistory.Add(new WorkflowHistoryViewModel()
            {
                ACTION = EnumHelper.GetDescription(Enums.ActionType.Created),
                ACTION_BY = data.CREATED_BY,
                ACTION_DATE = data.CREATED_DATE,
                FORM_TYPE_ID = Enums.FormType.MasterDataApproval,
                FORM_NUMBER = data.FORM_ID,
                Role = EnumHelper.GetDescription(_poaBLL.GetUserRole(data.CREATED_BY)),
                USERNAME = data.CREATED_BY
                
            });
            model.WorkflowHistory.AddRange(Mapper.Map<List<WorkflowHistoryViewModel>>(workflowHistory));
            model.Detail = Mapper.Map<MasterDataApprovalDetailViewModel>(data);
            model.IsMasterApprover = _userBLL.IsUserMasterApprover(CurrentUser.USER_ID);
            return View(model);
        }

        [HttpPost]
        public ActionResult Approve(MasterDataApprovalItemViewModel model)
        {
            try
            {
                model.Detail.APPROVED_BY = CurrentUser.USER_ID;
                
                UpdateWorkflowHistory(model.Detail, Enums.ActionType.Approve);
                _masterDataAprovalBLL.Approve(CurrentUser.USER_ID, model.Detail.APPROVAL_ID);

                var isNeedSend = (bool.TrueString.ToLower() == ConfigurationManager.AppSettings.Get("SendXmlMasterData").ToLower());
                if(isNeedSend) GenerateXml(model.Detail);

                AddMessageInfo("Success", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            
            return RedirectToAction("Detail", new { id = model.Detail.APPROVAL_ID });
        }

        private void GenerateXml(MasterDataApprovalDetailViewModel dataModel)
        {
            var data  = _masterDataAprovalBLL.GetByApprovalId(dataModel.APPROVAL_ID);
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

        [HttpPost]
        public ActionResult Reject(MasterDataApprovalItemViewModel modelView)
        {
            try
            {
                

                var data = _masterDataAprovalBLL.GetByApprovalId(modelView.Detail.APPROVAL_ID);

                var mappedData = Mapper.Map<MasterDataApprovalDetailViewModel>(data);
                mappedData.APPROVED_BY = CurrentUser.USER_ID;
                UpdateWorkflowHistory(mappedData, Enums.ActionType.Reject,modelView.RejectComment);
                _masterDataAprovalBLL.Reject(CurrentUser.USER_ID, data.APPROVAL_ID);
                
                AddMessageInfo("Success", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return RedirectToAction("Detail", new { id = modelView.Detail.APPROVAL_ID });
        }


        private void UpdateWorkflowHistory(MasterDataApprovalDetailViewModel data,Enums.ActionType action,string comment = "")
        {
            WorkflowHistoryDto workflowDto = new WorkflowHistoryDto();
            workflowDto.FORM_ID = data.APPROVAL_ID;
            workflowDto.ACTION = action;
            workflowDto.ACTION_DATE = DateTime.Now;
            workflowDto.FORM_NUMBER = data.FORM_ID;
            workflowDto.FORM_TYPE_ID = Enums.FormType.MasterDataApproval;

            if (action == Enums.ActionType.Created)
            {
                workflowDto.ROLE = _poaBLL.GetUserRole(data.CREATED_BY);
                workflowDto.ACTION_BY = data.CREATED_BY;
            }
            else
            {
                if (action == Enums.ActionType.Reject) workflowDto.COMMENT = comment;
                workflowDto.ROLE = _poaBLL.GetUserRole(data.APPROVED_BY);
                workflowDto.ACTION_BY = data.APPROVED_BY;
            }

            _workflowHistoryBLL.AddHistory(workflowDto);
        }


        

        
    }
}