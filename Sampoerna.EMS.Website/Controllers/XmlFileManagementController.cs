using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.XmlFileManagement;

namespace Sampoerna.EMS.Website.Controllers
{
    public class XmlFileManagementController : BaseController
    {
        private IXmlFileLogBLL _xmlFileLogBll;

        public XmlFileManagementController(IPageBLL pageBLL, IXmlFileLogBLL xmlFileLogBll)
            : base(pageBLL, Enums.MenuList.Settings)
        {
            _xmlFileLogBll = xmlFileLogBll;
        }

        private List<XmlFileManagementFormViewModel> GetData(XmlFileManagementIndexViewModel filter = null)
        {
            GetXmlLogByParamInput input = new GetXmlLogByParamInput();
            List<XML_LOGSDto> dbData;
            if (filter != null)
            {
                input.DateFrom = filter.DateFrom;
                input.DateTo = filter.DateTo;
            }

            dbData = _xmlFileLogBll.GetXmlLogByParam(input);
            return Mapper.Map<List<XmlFileManagementFormViewModel>>(dbData);
        }

        //private SelectList GetDateListXmlLog(bool isFrom, List<XmlFileManagementFormViewModel> list)
        //{

        //    IEnumerable<SelectItemModel> query;
        //    if (isFrom)
        //        query = from x in list.Where(c => c.TimeStamp.HasValue).OrderBy(c => c.TimeStamp)
        //                select new Models.SelectItemModel()
        //                {
        //                    ValueField = x.TimeStamp,
        //                    TextField = x.TimeStamp.Value.ToString("dd MMM yyyy")
        //                };
        //    else
        //        query = from x in list.Where(c => c.TimeStamp.HasValue).OrderByDescending(c => c.TimeStamp)
        //                select new Models.SelectItemModel()
        //                {
        //                    ValueField = x.TimeStamp,
        //                    TextField = x.TimeStamp.Value.ToString("dd MMM yyyy")
        //                };

        //    return new SelectList(query.DistinctBy(c => c.TextField), "ValueField", "TextField");

        //}

        public ActionResult Index()
        {
            var model = new XmlFileManagementIndexViewModel();
            model.MainMenu = Enums.MenuList.Settings;
            model.CurrentMenu = PageInfo;
            
            model.ListXmlLogs = GetData();

            //model.DateFromList = GetDateListXmlLog(true, model.ListXmlLogs);
            //model.DateToList = GetDateListXmlLog(false, model.ListXmlLogs);

            return View(model);
        }

        [HttpPost]
        public PartialViewResult Filter(XmlFileManagementIndexViewModel model)
        {

            model.ListXmlLogs = GetData(model);
          

            return PartialView("_XmlFileViewIndex", model);
        }

        public ActionResult Detail(long id)
        {
          
            var model = Mapper.Map<XmlFileManagementFormViewModel>(_xmlFileLogBll.GetByIdIncludeTables(id));

            model.MainMenu = Enums.MenuList.Settings;
            model.CurrentMenu = PageInfo;
            if (model.XmlLogStatus == Enums.XmlLogStatus.Error)
                model.IsError = true;
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;

            return View(model);
        }

        public ActionResult ReExecute(long id)
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                    return RedirectToAction("Index");
               

                var input = new UpdateXmlFileInput();

                input.XmlId = id;
                input.SourcePath = ConfigurationManager.AppSettings["XmlErrorPath"];// @"C:\Deploy\InboundMsg\error\";
                input.DestPath = ConfigurationManager.AppSettings["XmlInboundPath"];//@"C:\Deploy\InboundMsg\";

                _xmlFileLogBll.UpdateXmlFile(input);

                AddMessageInfo("Success Move File", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Index");
        }
	}
}