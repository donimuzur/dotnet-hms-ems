using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Reporting.WebForms;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.ReportingData;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.HeaderFooter;

namespace Sampoerna.EMS.Website.Controllers
{
    public class HeaderFooterController : BaseController
    {
        private IHeaderFooterBLL _headerFooterBll;
        private ICompanyBLL _companyBll;
        private IChangesHistoryBLL _changesHistoryBll;

        public HeaderFooterController(IPageBLL pageBLL, IHeaderFooterBLL headerFooterBll, ICompanyBLL companyBll, IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.HeaderFooter)
        {
            _headerFooterBll = headerFooterBll;
            _companyBll = companyBll;
            _changesHistoryBll = changesHistoryBll;
        }
        
        private SelectList GetCompanyList()
        {
            var data = _companyBll.GetMasterData();
            return new SelectList(data, "COMPANY_ID", "BUKRS");
        }

        //
        // GET: /HeaderFooter/
        public ActionResult Index()
        {
            var data = _headerFooterBll.GetAll();
            var model = new HeaderFooterViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = Enums.MenuList.MasterData,
                Details = Mapper.Map<List<HeaderFooterItem>>(data)
            };
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var data = _headerFooterBll.GetDetailsById(id);
            var model = new HeaderFooterItemViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = Enums.MenuList.MasterData,
                Detail = Mapper.Map<HeaderFooterDetailItem>(data),
                ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id))
            };
            return View(model);
        }

        public ActionResult InitialCreate(HeaderFooterItemViewModel model)
        {
            model.CompanyList = GetCompanyList();
            model.CurrentMenu = PageInfo;
            model.MainMenu = Enums.MenuList.MasterData;
            return View("Create", model);
        }

        public ActionResult Create()
        {
            return InitialCreate(new HeaderFooterItemViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = Enums.MenuList.MasterData,
                Detail = new HeaderFooterDetailItem() { HeaderFooterMapList = InitialHeaderFooterMapList() }
            });
        }
        
        [HttpPost]
        public ActionResult Create(HeaderFooterItemViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                //do save
                model.Detail.FOOTER_CONTENT = model.Detail.FOOTER_CONTENT.Replace(Environment.NewLine, "<br />");
                
                //do upload image header
                string imageHeaderUrl = SaveUploadedFile(model.HeaderImageFile, model.Detail.COMPANY_ID.Value.ToString(),
                    model.Detail.COMPANY_CODE);
                
                model.Detail.HEADER_IMAGE_PATH = imageHeaderUrl;
                
                var saveOutput = _headerFooterBll.Save(Mapper.Map<HeaderFooterDetails>(model.Detail), CurrentUser.USER_ID);

                if (saveOutput.Success)
                {
                    return RedirectToAction("Index");
                }

                //Set ErrorMessage
                model.ErrorMessage = saveOutput.ErrorCode + "\n\r" + saveOutput.ErrorMessage;
            }

            return InitialCreate(model);
        }
        
        public ActionResult Edit(int id)
        {
            var data = _headerFooterBll.GetDetailsById(id);
            var model = new HeaderFooterItemViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = Enums.MenuList.MasterData,
                Detail = Mapper.Map<HeaderFooterDetailItem>(data),
                ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id))
            };
            return InitialEdit(model);
        }

        [HttpPost]
        public ActionResult Edit(HeaderFooterItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                //do save
                model.Detail.FOOTER_CONTENT = model.Detail.FOOTER_CONTENT.Replace(Environment.NewLine, "<br />");

                //do upload image header
                //delete first if there is already have header image
                string imageHeaderUrl;
                if (model.HeaderImageFile != null)
                {
                    DeleteUploadedFile(model.Detail.HEADER_IMAGE_PATH_BEFOREEDIT);
                    imageHeaderUrl = SaveUploadedFile(model.HeaderImageFile, model.Detail.COMPANY_ID.Value.ToString(),
                    model.Detail.COMPANY_CODE);
                }
                else
                {
                    imageHeaderUrl = model.Detail.HEADER_IMAGE_PATH_BEFOREEDIT;
                }

                model.Detail.HEADER_IMAGE_PATH = imageHeaderUrl;

                var saveOutput = _headerFooterBll.Save(Mapper.Map<HeaderFooterDetails>(model.Detail), CurrentUser.USER_ID);

                if (saveOutput.Success)
                {
                    return RedirectToAction("Index");
                }

                //Set ErrorMessage
                model.ErrorMessage = saveOutput.ErrorCode + "\n\r" + saveOutput.ErrorMessage;
            }
            model.Detail.FOOTER_CONTENT = model.Detail.FOOTER_CONTENT.Replace("<br />", Environment.NewLine);
            return InitialEdit(model);
        }

        public ActionResult InitialEdit(HeaderFooterItemViewModel model)
        {
            model.CompanyList = GetCompanyList();
            model.CurrentMenu = PageInfo;
            model.MainMenu = Enums.MenuList.MasterData;
            return View("Edit", model);
        }
        
        [HttpPost]
        public JsonResult GetCompanyDetail(long id)
        {
            var data = _companyBll.GetById(id);
            return Json(data);
        }

        private List<HeaderFooterMapItem> InitialHeaderFooterMapList()
        {
            var enumValues = EnumHelper.GetValues<Enums.FormType>();
            var rc = enumValues.Select(enumValue => new HeaderFooterMapItem()
            {
                HEADER_FOOTER_FORM_MAP_ID = 0,
                FORM_TYPE_ID = enumValue,
                FORM_TYPE_DESC = EnumHelper.GetDescription(enumValue),
                IS_HEADER_SET = false,
                IS_FOOTER_SET = false,
                HEADER_FOOTER_ID = 0
            }).ToList();
            return rc;
        }

        /// <summary>
        /// logic to save Image Header
        /// </summary>
        /// <param name="file"></param>
        /// <param name="companyid"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        private string SaveUploadedFile(HttpPostedFileBase file, string companyid, string companycode)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            //initialize folders in case deleted by an test publish profile
            if (!Directory.Exists(Server.MapPath(Constans.MasterDataHeaderFooterFolder)))
                Directory.CreateDirectory(Server.MapPath(Constans.MasterDataHeaderFooterFolder));

            sFileName = Constans.MasterDataHeaderFooterFolder + Path.GetFileName(companyid + companycode + "_header" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        private void DeleteUploadedFile(string sFilePath)
        {
            string path = Server.MapPath(sFilePath);
            //do delete first
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public ActionResult PrintPreview(int id, bool isHeaderSet, bool isFooterSet)
        {
            
            var headerFooterData = Mapper.Map<HeaderFooterItem>(_headerFooterBll.GetById(id));
            headerFooterData.HEADER_IMAGE_PATH = !string.IsNullOrEmpty(headerFooterData.HEADER_IMAGE_PATH)
                ? new Uri(Server.MapPath(headerFooterData.HEADER_IMAGE_PATH)).AbsoluteUri
                : string.Empty;
            headerFooterData.FOOTER_CONTENT = !isFooterSet ? string.Empty : headerFooterData.FOOTER_CONTENT;
            headerFooterData.IsHeaderHide = !isHeaderSet;

            var srcToConvert = new List<HeaderFooterItem> { headerFooterData };
            var headerFooterDataTable =
                DataTableHelper.ConvertToDataTable(srcToConvert.ToArray(), new dsHeaderFooter.HeaderFooterDataTable());
            var rptDataSources = new List<ReportDataSource>
            {
                new ReportDataSource("ds_headerfooter", headerFooterDataTable)
            };
            
            //set session for reporting
            Session[Constans.SessionKey.ReportPath] = "Reports/HeaderFooterPreview.rdlc";
            Session[Constans.SessionKey.ReportDataSources] = rptDataSources;
            return RedirectToAction("ShowReport", "AspxReportViewer");
        }

        public ActionResult Delete(int id)
        {
            _headerFooterBll.Delete(id, CurrentUser.USER_ID);
            return RedirectToAction("Details", id);
        }

    }
}