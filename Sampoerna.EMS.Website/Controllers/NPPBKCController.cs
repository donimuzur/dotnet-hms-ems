using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.NPPBKC;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class NPPBKCController : BaseController
    {
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IMasterDataBLL _masterDataBll;
        private ICompanyBLL _companyBll;
        private IZaidmExKPPBCBLL _kppbcBll;
        private IPlantBLL _plantBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;

        public NPPBKCController(IZaidmExNPPBKCBLL nppbkcBll, IChangesHistoryBLL changesHistoryBll, ICompanyBLL companyBll, IMasterDataBLL masterData, IZaidmExKPPBCBLL kppbcBll,
            IPageBLL pageBLL, IPlantBLL plantBll)
            : base(pageBLL, Enums.MenuList.NPPBKC)
        {
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterData;
            _companyBll = companyBll;
            _kppbcBll = kppbcBll;
            _plantBll = plantBll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /NPPBKC/
        public ActionResult Index()
        {
            var nppbkc = new NPPBKCIViewModels
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<VirtualNppbckDetails>>(_nppbkcBll.GetAll()),
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
            };

            //ViewBag.Message = TempData["message"];
            return View("Index", nppbkc);

        }

        public ActionResult Edit(string id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }

            var nppbkc = _nppbkcBll.GetById(id);

            if (nppbkc == null)
            {
                HttpNotFound();
            }
            if (nppbkc.IS_DELETED == true)
            {
                return RedirectToAction("Detail", "NPPBKC", new { id = nppbkc.NPPBKC_ID });
            }
            var model = new NppbkcFormModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.Plant = _plantBll.Get(id);


            

            model.Detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);

            return View(model);

        }
        private void SetChanges(VirtualNppbckDetails origin, ZAIDM_EX_NPPBKC nppbkc)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("REGION_OFFICE_DGCE", origin.RegionOfficeOfDGCE == nppbkc.REGION_DGCE);
            changesData.Add("CITY_ALIAS",origin.CityAlias == nppbkc.CITY_ALIAS);
            changesData.Add("TEXT_TO", origin.TextTo == nppbkc.TEXT_TO);


            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.NPPBKC;
                    changes.FORM_ID = nppbkc.NPPBKC_ID;
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = CurrentUser.USER_ID;
                    changes.MODIFIED_DATE = DateTime.Now;
                    switch (listChange.Key)
                    {
                        case "REGION_OFFICE_DGCE":
                            changes.OLD_VALUE = origin.RegionOfficeOfDGCE;
                            changes.NEW_VALUE = nppbkc.REGION_DGCE;
                            break;

                        case "CITY_ALIAS":
                            changes.OLD_VALUE = origin.CityAlias;
                            changes.NEW_VALUE = nppbkc.CITY_ALIAS;
                            break;
                        case "TEXT_TO":
                            changes.OLD_VALUE = origin.TextTo;
                            changes.NEW_VALUE = nppbkc.TEXT_TO;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);


                }
            }



        }
        [HttpPost]
        public ActionResult Edit(NppbkcFormModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    var Nppbkc = _nppbkcBll.GetById(model.Detail.VirtualNppbckId);
            //    model.MainMenu = Enums.MenuList.MasterData;
            //    model.CurrentMenu = PageInfo;

            //    var detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(Nppbkc);

            //    model.Detail = detail;
            //    return View("Edit", model);
            //}

            try
            {
                var nppbkcId = model.Detail.VirtualNppbckId;
                var nppbkc = _nppbkcBll.GetById(nppbkcId);
                var origin = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);
                AutoMapper.Mapper.Map(model.Detail, nppbkc);
                SetChanges(origin, nppbkc);
                AutoMapper.Mapper.Map(model.Detail, nppbkc);
                _nppbkcBll.Save(nppbkc);


                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                       );
                return RedirectToAction("Index");

            }
            catch(Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );
                return View();
            }

        }
        public ActionResult Detail(string id)
        {
            var nppbkc = _nppbkcBll.GetById(id);
            if (nppbkc == null)
            {
                HttpNotFound();
            }
            var changeHistoryList = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.NPPBKC, id);

            var model = new NppbkcFormModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.Plant = _plantBll.Get(id);
            var detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);
            model.Detail = detail;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);

            return View(model);

        }
        public ActionResult Delete(string id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            try
            {
                _nppbkcBll.Delete(id);
                AddMessageInfo(Constans.SubmitMessage.Deleted, Enums.MessageInfoType.Success
                      );
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                        );
            }
            return RedirectToAction("Index");
        }

        #region export xls

        public void ExportXlsFile()
        {
            string pathFile = "";

            pathFile = CreateXlsFile();

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsFile()
        {
            //get data
            var listData = Mapper.Map<List<VirtualNppbckDetails>>(_nppbkcBll.GetAll());

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master NPPBKC");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcel(slDocument);

            //create data
            slDocument = CreateDataExcel(slDocument, listData);

            var fileName = "MasterData_MasterNppbkc" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "NPPBKC ID");
            slDocument.SetCellValue(iRow, 2, "Address");
            slDocument.SetCellValue(iRow, 3, "City Alias");
            slDocument.SetCellValue(iRow, 4, "Region Office of DGCE");
            slDocument.SetCellValue(iRow, 5, "Text To");
            slDocument.SetCellValue(iRow, 6, "Deleted");


            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcel(SLDocument slDocument, List<VirtualNppbckDetails> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.VirtualNppbckId);
                slDocument.SetCellValue(iRow, 2, data.Address1);
                slDocument.SetCellValue(iRow, 3, data.CityAlias);
                slDocument.SetCellValue(iRow, 4, data.RegionOfficeOfDGCE);
                slDocument.SetCellValue(iRow, 5, data.TextTo);
                slDocument.SetCellValue(iRow, 6, data.Is_Deleted);


                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 6);
            slDocument.SetCellStyle(3, 1, iRow - 1, 6, valueStyle);

            return slDocument;
        }

        #endregion
    }
}