using System.Linq;
using AutoMapper;
using iTextSharp.text.pdf.qrcode;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.MonthClosing;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MonthClosingController : BaseController
    {
        private Enums.MenuList _mainMenu;

        private IMonthClosingBLL _monthClosingBll;
        private IMonthClosingDocBLL _monthClosingDocBll;

        public MonthClosingController(IPageBLL pageBLL, IMonthClosingBLL monthClosingBll, IMonthClosingDocBLL monthClosingDocBll)
            : base(pageBLL, Enums.MenuList.Settings)
        {
            _mainMenu = Enums.MenuList.Settings;
            _monthClosingBll = monthClosingBll;
            _monthClosingDocBll = monthClosingDocBll;
        }

        public ActionResult Index()
        {
            var model = new MonthClosingIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            var closingList = _monthClosingBll.GetList();
            model.MonthClosingList = Mapper.Map<List<MonthClosingDetail>>(closingList);
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;

            return View("Index", model);
        }

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new MonthClosingIndexViewModel
            {
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
                Details = new MonthClosingDetail()
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(MonthClosingIndexViewModel model)
        {
            return View("Create", InitialModel(model));
        }

        private MonthClosingIndexViewModel InitialModel(MonthClosingIndexViewModel model)
        {
            if (model.Details != null) model.Details.ClosingDate = DateTime.Now;

            return (model);
        }

        [HttpPost]
        public ActionResult Create(MonthClosingIndexViewModel model)
        {
            try
            {
                MonthClosingDto data = new MonthClosingDto();

                model.Details.MonthClosingDoc = new List<MonthClosingDocModel>();
                if (model.Details.MonthClosingFiles != null)
                {
                    int counter = 0;
                    foreach (var item in model.Details.MonthClosingFiles)
                    {
                        if (item != null)
                        {
                            var filenamecheck = item.FileName;

                            if (filenamecheck.Contains("\\"))
                            {
                                filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }

                            var attachment = new MonthClosingDocModel()
                            {
                                FILE_NAME = filenamecheck,
                                FILE_PATH = SaveUploadedFile(item, model.Details.ClosingDate.ToString("MMyyyy"), counter),
                                MONTH_FLAG = model.Details.ClosingDate.ToString("MMyyyy")
                            };
                            model.Details.MonthClosingDoc.Add(attachment);
                            counter += 1;
                        }
                    }
                }

                data = Mapper.Map<MonthClosingDto>(model.Details);
                var monthClosingData = _monthClosingBll.Save(data);

                var docData = Mapper.Map<List<MonthClosingDocDto>>(model.Details.MonthClosingDoc);
                var monthClosingDocData = _monthClosingDocBll.Save(docData);

                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        private string SaveUploadedFile(HttpPostedFileBase file, string monthFlag, int counter)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            //initialize folders in case deleted by an test publish profile
            if (!Directory.Exists(Server.MapPath(Constans.MonthClosingDocFolderPath)))
                Directory.CreateDirectory(Server.MapPath(Constans.MonthClosingDocFolderPath));

            sFileName = Constans.MonthClosingDocFolderPath + Path.GetFileName(monthFlag + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + counter + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }

            var closingData = _monthClosingBll.GetById(id.Value);

            if (closingData == null)
            {
                return HttpNotFound();
            }

            var model = new MonthClosingIndexViewModel();

            try
            {
                model.Details = Mapper.Map<MonthClosingDetail>(closingData);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var closingData = _monthClosingBll.GetById(id.Value);

            if (closingData == null)
            {
                return HttpNotFound();
            }

            var model = new MonthClosingIndexViewModel();

            try
            {
                model.Details = Mapper.Map<MonthClosingDetail>(closingData);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}