
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.XmlLog;

namespace Sampoerna.EMS.Website.Controllers
{
    public class XmlLogController : BaseController
    {
        private INlogBLL _nLogBll;
        private IMonthBLL _monthBll;

        public XmlLogController(INlogBLL nLogBll, IPageBLL pageBLL, IMonthBLL monthBll)
            : base(pageBLL, Enums.MenuList.Settings)
        {
            _nLogBll = nLogBll;
            _monthBll = monthBll;
        }

        private SelectList GetListMont(List<NlogDto> listData)
        {
            var data = listData.DistinctBy(c => c.Timestamp != null ? c.Timestamp.Value.Month : 0).ToList();
            var listMonth = new List<MONTH>();
            foreach (var nlogDto in data)
            {
                if (!nlogDto.Timestamp.HasValue) continue;
                var month = _monthBll.GetMonth(nlogDto.Timestamp.Value.Month);
                if (month != null)
                    listMonth.Add(month);
            }

            return new SelectList(listMonth, "MONTH_ID", "MONTH_NAME_ENG");

        }

        private XmlLogIndexViewModel SetListIndex(XmlLogIndexViewModel model)
        {
            var listData = _nLogBll.GetAllData();
            var result = listData.DistinctBy(c=>c.FileName).ToList();
            model.FileNameList = new SelectList(result, "FileName", "FileName");
            //model.MonthList = new SelectList(listData.Select(c => c.Timestamp != null ? c.Timestamp.Value.Month : 0).Distinct().ToList(), "FileName", "FileName");
            //model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.MonthList = GetListMont(listData);

            return model;
        }
        //
        // GET: /XmlLog/
        public ActionResult Index()
        {
            var model = new XmlLogIndexViewModel();
            model.MainMenu = Enums.MenuList.Settings;
            model.CurrentMenu = PageInfo;
            var dataXml = _nLogBll.GetNlogByParam(new NlogGetByParamInput());
            model.ListXmlLogs = Mapper.Map<List<XmlLogFormViewModel>>(dataXml);
            foreach (var xmlLogFormViewModel in model.ListXmlLogs)
            {
                if (xmlLogFormViewModel.Logger.Length > 30)
                    xmlLogFormViewModel.Logger = xmlLogFormViewModel.Logger.Substring(0, 30) + "...";

                if (xmlLogFormViewModel.Message.Length > 30)
                    xmlLogFormViewModel.Message = xmlLogFormViewModel.Message.Substring(0, 30) + "...";
            }


            model = SetListIndex(model);

            return View(model);
        }

        public ActionResult Detail(long id)
        {
            var model = Mapper.Map<XmlLogFormViewModel>(_nLogBll.GetById(id));

            model.MainMenu = Enums.MenuList.Settings;
            model.CurrentMenu = PageInfo;

            return View(model);
        }

        [HttpPost]
        public PartialViewResult Filter(XmlLogIndexViewModel model)
        {
            var input = new NlogGetByParamInput()
            {
                FileName = model.FileName,
                Month = model.Month
            };

            var dataXml = _nLogBll.GetNlogByParam(input);

            model.ListXmlLogs = Mapper.Map<List<XmlLogFormViewModel>>(dataXml);

            foreach (var xmlLogFormViewModel in model.ListXmlLogs)
            {
                if (xmlLogFormViewModel.Logger.Length > 30)
                    xmlLogFormViewModel.Logger = xmlLogFormViewModel.Logger.Substring(0, 30) + "...";

                if (xmlLogFormViewModel.Message.Length > 30)
                    xmlLogFormViewModel.Message = xmlLogFormViewModel.Message.Substring(0, 30) + "...";
            }

            return PartialView("_XmlLogViewIndex", model);
        }

        public ActionResult BackupXml(XmlLogIndexViewModel model)
        {

            //var input = new NlogGetByParamInput()
            //{
            //    FileName = model.FileName,
            //    Month = model.Month
            //};

            //_nLogBll.DeleteDataByParam(input);

            //AddMessageInfo("Success backup data", Enums.MessageInfoType.Success);
            
            //return RedirectToAction("Index");

            try
            {
                string folderPath = @"D:\Temp\EMS\zipFolder\\";
                string zipName = "XmlLogZip" + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + CurrentUser.USER_ID + ".zip";

                var input = new BackupXmlLogInput();
                input.FolderPath = folderPath;
                input.FileZipName = folderPath + zipName;

                input.FileName = model.FileName;
                input.Month = model.Month;

                _nLogBll.BackupXmlLog(input);

                // Read bytes from disk
                //byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Directories/hello/sample.zip"));
                byte[] fileBytes = System.IO.File.ReadAllBytes(folderPath + zipName);

                // Return bytes as stream for download
                return File(fileBytes, "application/zip", zipName);
            }
            catch (Exception ex)
            {

                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                return RedirectToAction("Index");
            }
          

        }

      
	}
}