using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WebForms;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.ReportingData;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.HeaderFooter;
using System.Configuration;

namespace Sampoerna.EMS.Website.Controllers
{
    public class HeaderFooterController : BaseController
    {
        private IHeaderFooterBLL _headerFooterBll;
        private ICompanyBLL _companyBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;

        public HeaderFooterController(IPageBLL pageBLL, IHeaderFooterBLL headerFooterBll, ICompanyBLL companyBll, IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.HeaderFooter)
        {
            _headerFooterBll = headerFooterBll;
            _companyBll = companyBll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        private SelectList GetCompanyList()
        {
            var data = _companyBll.GetMasterData();
            return new SelectList(data, "BUKRS", "BUKRS");
        }

        //
        // GET: /HeaderFooter/
        public ActionResult Index()
        {
            var data = _headerFooterBll.GetAll();
            var model = new HeaderFooterViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
                Details = Mapper.Map<List<HeaderFooterItem>>(data)
            };
            ViewBag.Message = TempData["message"];
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var data = _headerFooterBll.GetDetailsById(id);
            var model = new HeaderFooterItemViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
                Detail = Mapper.Map<HeaderFooterDetailItem>(data),
                ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id.ToString()))
            };
            return View(model);
        }

        public ActionResult InitialCreate(HeaderFooterItemViewModel model)
        {
            model.CompanyList = GetCompanyList();
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;
            return View("Create", model);
        }

        public ActionResult Create()
        {
            return InitialCreate(new HeaderFooterItemViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
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
                var param = Mapper.Map<HeaderFooterDetails>(model.Detail);

                param.HeaderFooterMapList = new List<HeaderFooterMap>();

                var existCompany = _headerFooterBll.GetCompanyId(model.Detail.COMPANY_ID.ToString());
                if (existCompany.MessageExist == "1")
                {
                    AddMessageInfo("Company Code Already Set", Enums.MessageInfoType.Warning);
                    return InitialCreate(model);
                }
                var saveOutput = _headerFooterBll.Save(Mapper.Map<HeaderFooterDetails>(model.Detail), CurrentUser.USER_ID);

               
                if (saveOutput.Success)
                {
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                      );
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
            if (data.IS_DELETED.HasValue && data.IS_DELETED.Value)
            {
                return RedirectToAction("Details", "HeaderFooter", new { id = data.HEADER_FOOTER_ID });
            }
            var model = new HeaderFooterItemViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
                Detail = Mapper.Map<HeaderFooterDetailItem>(data),
                ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id.ToString()))
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
                    AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
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
            model.MainMenu = _mainMenu;
            model.Detail.HeaderFooterMapList = InitialEditHeaderFooterMapList(model.Detail.HeaderFooterMapList);
            return View("Edit", model);
        }

        [HttpPost]
        public JsonResult GetCompanyDetail(string id)
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

        public DataSet PrintPreview(int id, bool isHeaderSet, bool isFooterSet)
        {

            var headerFooterData = Mapper.Map<HeaderFooterItem>(_headerFooterBll.GetById(id));
            headerFooterData.HEADER_IMAGE_PATH = !string.IsNullOrEmpty(headerFooterData.HEADER_IMAGE_PATH)
                ? new Uri(Server.MapPath(headerFooterData.HEADER_IMAGE_PATH)).AbsoluteUri
                : string.Empty;
            headerFooterData.FOOTER_CONTENT = !isFooterSet ? string.Empty : headerFooterData.FOOTER_CONTENT;
            headerFooterData.IsHeaderHide = !isHeaderSet;

            var srcToConvert = new List<HeaderFooterItem> { headerFooterData };

            DataSet ds = new DataSet("HeaderFooter");

            DataTable dt = new DataTable("DataTable1");

            // object of data row 
            DataRow drow;
            // add the column in table to store the image of Byte array type 
            dt.Columns.Add("footer", System.Type.GetType("System.String"));
            drow = dt.NewRow();
            drow[0] = headerFooterData.FOOTER_CONTENT;
            dt.Rows.Add(drow);
            if (isHeaderSet)
            {
                dt = GetImageRow(dt, headerFooterData.HEADER_IMAGE_PATH_BEFOREEDIT);
            }
            ds.Tables.Add(dt);
            return ds;
            //return RedirectToAction("ShowReport", "AspxReportViewer");
        }

        public ActionResult Delete(int id)
        {

            _headerFooterBll.Delete(id, CurrentUser.USER_ID);
            TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
            return RedirectToAction("Index");
        }

        //[EncryptedParameter]
        public ActionResult PrintOut(int id, bool isHeaderSet, bool isFooterSet)
        {
            //DataTable dt = new DataTable();
            ReportClass rpt = new ReportClass();
            string report_path = ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = report_path + "HeaderFooter\\HeaderFooterPreview.rpt";

            var dt = PrintPreview(id, isHeaderSet, isFooterSet);
            rpt.Load();
            rpt.SetDataSource(dt);

            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        private DataTable GetImageRow(DataTable dt, string imagePath)
        {

            try
            {

                FileStream fs;
                BinaryReader br;

                if (System.IO.File.Exists(Server.MapPath(imagePath)))
                {
                    fs = new FileStream(Server.MapPath(imagePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                else
                {
                    // if photo does not exist show the nophoto.jpg file 
                    fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                // initialise the binary reader from file streamobject 
                br = new BinaryReader(fs);
                // define the byte array of filelength 
                byte[] imgbyte = new byte[fs.Length + 1];
                // read the bytes from the binary reader 
                imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
                dt.Columns.Add("image", System.Type.GetType("System.Byte[]"));

                dt.Rows[0]["image"] = imgbyte;


                br.Close();
                // close the binary reader 
                fs.Close();
                // close the file stream 




            }
            catch (Exception ex)
            {
            }
            return dt;
            // Return Datatable After Image Row Insertion

        }


        private List<HeaderFooterMapItem> InitialEditHeaderFooterMapList(List<HeaderFooterMapItem> existingData)
        {
            var formTypeList = existingData.Select(d => d.FORM_TYPE_ID).ToList();
            var enumValues = EnumHelper.GetValues<Enums.FormType>().Where(c => !formTypeList.Contains(c));
            
            var rc = enumValues.Select(enumValue => new HeaderFooterMapItem()
            {
                HEADER_FOOTER_FORM_MAP_ID = 0,
                FORM_TYPE_ID = enumValue,
                FORM_TYPE_DESC = EnumHelper.GetDescription(enumValue),
                IS_HEADER_SET = false,
                IS_FOOTER_SET = false,
                HEADER_FOOTER_ID = 0
            }).ToList();
            existingData.AddRange(rc);
            return existingData;
        }
        
    }
}