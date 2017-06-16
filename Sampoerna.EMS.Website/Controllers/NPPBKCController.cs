using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;

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
        private SystemReferenceService refService;
        private NppbkcManagementService service;

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
            refService = new SystemReferenceService();
            service = new NppbkcManagementService();
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
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Controller ? true : false)
            };

            //ViewBag.Message = TempData["message"];
            return View("Index", nppbkc);

        }

        public ActionResult Edit(string id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                return RedirectToAction("Detail", new { id });
            }

            var nppbkc = service.Find(id);

            if (nppbkc == null)
            {
                HttpNotFound();
            }
            if (nppbkc.IS_DELETED == true)
            {
                return RedirectToAction("Detail", "NPPBKC", new { id = nppbkc.NPPBKC_ID });
            }
            var model = new NewNPPBKCViewModel.NppbkcModel();
            var detail = this.MapToModel(nppbkc);
            model.Detail = detail;
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.Plant = _plantBll.Get(id);
            var changeHistoryList = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.NPPBKC, id);
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
            return View(model);

        }

        private List<CustomService.Data.CHANGES_HISTORY> SetChanges(NewNPPBKCViewModel.NewNppbckDetails origin, CustomService.Data.MASTER_NPPBKC nppbkc)
        {
            var changeList = new Dictionary<string, string[]>();
            PlaceValues("NPPBKC_ID", origin.VirtualNppbckId, nppbkc.NPPBKC_ID, ref changeList);
            PlaceValues("ADDR1", origin.Address1, nppbkc.ADDR1, ref changeList);
            PlaceValues("ADDR2", origin.Address2, nppbkc.ADDR2, ref changeList);
            PlaceValues("KPPBC_ID", origin.KppbcId, nppbkc.KPPBC_ID, ref changeList);
            PlaceValues("KPPBC_ADDRESS", origin.KppbcAddress, nppbkc.KPPBC_ADDRESS, ref changeList);
            PlaceValues("DGCE_ADDRESS", origin.DgceAddress, nppbkc.DGCE_ADDRESS, ref changeList);
            PlaceValues("CITY", origin.City, nppbkc.CITY, ref changeList);
            PlaceValues("CITY_ALIAS", origin.CityAlias, nppbkc.CITY_ALIAS, ref changeList);
            PlaceValues("REGION", origin.Region, nppbkc.REGION, ref changeList);
            PlaceValues("REGION_DGCE", origin.RegionOfficeOfDGCE, nppbkc.REGION_DGCE, ref changeList);
            PlaceValues("TEXT_TO", origin.TextTo, nppbkc.TEXT_TO, ref changeList);
            PlaceValues("LOCATION", origin.Location, nppbkc.LOCATION, ref changeList);
            PlaceValues("FLAG_FOR_LACK1", origin.FlagForLack1 ? "True" : "False", nppbkc.FLAG_FOR_LACK1 != null ? (nppbkc.FLAG_FOR_LACK1.Value ? "True" : "False") : "False", ref changeList);
            if (nppbkc.START_DATE != null)
            {
                var newVal = (origin.StartDate.HasValue) ? origin.StartDate.Value.ToString("dd MMMM yyyy") : null;
                PlaceValues("START_DATE", newVal, nppbkc.START_DATE.Value.ToString("dd MMMM yyyy"), ref changeList);
            }

            if (nppbkc.END_DATE != null)
            {
                var newVal = (origin.EndDate.HasValue) ? origin.EndDate.Value.ToString("dd MMMM yyyy") : null;
                PlaceValues("END_DATE", newVal, nppbkc.END_DATE.Value.ToString("dd MMMM yyyy"), ref changeList);
            }

            if (nppbkc.VENDORS != null)
            {
                PlaceValues("ACCOUNT_NUMBER", origin.AcountNumber, nppbkc.VENDORS.LIFNR, ref changeList);
            }
            List<CustomService.Data.CHANGES_HISTORY> changeLogs = new List<CustomService.Data.CHANGES_HISTORY>();
            foreach (var changed in changeList)
            {

                var changes = new CustomService.Data.CHANGES_HISTORY();
                changes.FORM_TYPE_ID = (int)Enums.MenuList.NPPBKC;
                changes.FORM_ID = nppbkc.NPPBKC_ID;
                changes.FIELD_NAME = changed.Key;
                changes.MODIFIED_BY = CurrentUser.USER_ID;
                changes.MODIFIED_DATE = DateTime.Now;
                changes.OLD_VALUE = changed.Value[0];
                changes.NEW_VALUE = changed.Value[1];
                changeLogs.Add(changes);
            }
            return changeLogs;
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(NewNPPBKCViewModel.NppbkcModel model)
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
                var nppbkc = service.Find(nppbkcId);
                var origin = this.MapToModel(nppbkc);
                model.Detail.CreateDate = origin.CreateDate;
                model.Detail.CreatedBy = origin.CreatedBy;
                model.Detail.ModifiedDate = origin.ModifiedDate;
                model.Detail.ModifiedBy = origin.ModifiedBy;
                model.Detail.Is_Deleted = origin.Is_Deleted;
                nppbkc = this.MapToEntity(model.Detail);
                var logs = SetChanges(origin, nppbkc);
                //AutoMapper.Mapper.Map(model.Detail, nppbkc);
                service.Save(nppbkc, CurrentUser.USER_ID, logs);

                
                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success
                       );
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;
                return View("Edit", model);
            }

        }
        public ActionResult Detail(string id)
        {
            var nppbkc = service.Find(id);
            if (nppbkc == null)
            {
                HttpNotFound();
            }
            var changeHistoryList = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.NPPBKC, id);

            var model = new NewNPPBKCViewModel.NppbkcModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.Plant = _plantBll.Get(id);
            var detail = this.MapToModel(nppbkc);
            model.Detail = detail;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);

            return View(model);

        }
        public ActionResult Delete(string id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
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
        public ActionResult Create()
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
                {
                    AddMessageInfo("You don't have permission to access", Enums.MessageInfoType.Error
                      );
                    return RedirectToAction("Index");
                }

                var model = new NewNPPBKCViewModel.NppbkcModel(); ;
                model.Detail = new NewNPPBKCViewModel.NewNppbckDetails();
                model.Detail.StartDate = DateTime.Now;
                model.Detail.EndDate = DateTime.Now.AddYears(1);
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

                return View(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                      );
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(NewNPPBKCViewModel.NppbkcModel model)
        {
            try
            {
                var origin = this.MapToModel(new CustomService.Data.MASTER_NPPBKC());
                model.Detail.CreateDate = origin.CreateDate;
                model.Detail.CreatedBy = origin.CreatedBy;
                model.Detail.ModifiedDate = origin.ModifiedDate;
                model.Detail.ModifiedBy = origin.ModifiedBy;
                model.Detail.Is_Deleted = origin.Is_Deleted;
                var nppbkc = this.MapToEntity(model.Detail);
                var logs = SetChanges(origin, nppbkc);
                service.Save(nppbkc, CurrentUser.USER_ID, logs);


                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                       );
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;
                return View("Create", model);
            }
        }

        private void PlaceValues(string key, string oldValue, string newValue, ref Dictionary<string, string[]> map)
        {
            var changed = oldValue != newValue && newValue != null;
            if (changed)
            {
                string[] pair = new string[2];
                pair[0] = oldValue ?? "N/A";
                pair[1] = newValue ?? "N/A";
                map.Add(key, pair);
            }

        }


        #region Helper
        public NewNPPBKCViewModel.NewNppbckDetails MapToModel(CustomService.Data.MASTER_NPPBKC nppbkc)
        {
            try
            {
                return new NewNPPBKCViewModel.NewNppbckDetails()
                {
                    VirtualNppbckId = nppbkc.NPPBKC_ID,
                    Address1 = nppbkc.ADDR1,
                    Address2 = nppbkc.ADDR2,
                    City = nppbkc.CITY,
                    CityAlias = nppbkc.CITY_ALIAS,
                    KppbcId = nppbkc.KPPBC_ID,
                    KppbcAddress = nppbkc.KPPBC_ADDRESS,
                    Region = nppbkc.REGION,
                    RegionOfficeOfDGCE = nppbkc.REGION_DGCE,
                    AcountNumber = (nppbkc.VENDORS != null) ? nppbkc.VENDORS.LIFNR : null,
                    TextTo = nppbkc.TEXT_TO,
                    StartDate = nppbkc.START_DATE,
                    EndDate = nppbkc.END_DATE,
                    CreatedBy = nppbkc.CREATED_BY,
                    CreateDate = nppbkc.CREATED_DATE,
                    ModifiedBy = nppbkc.MODIFIED_BY,
                    ModifiedDate = nppbkc.MODIFIED_DATE,
                    DgceAddress = nppbkc.DGCE_ADDRESS,
                    Location = nppbkc.LOCATION,
                    FlagForLack1 = (nppbkc.FLAG_FOR_LACK1 != null) ? nppbkc.FLAG_FOR_LACK1.Value : false,
                    Is_Deleted = (nppbkc.IS_DELETED != null ) ? ((nppbkc.IS_DELETED.Value) ? "Yes" : "No") : "No"

                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CustomService.Data.MASTER_NPPBKC MapToEntity(NewNPPBKCViewModel.NewNppbckDetails model)
        {
            try
            {
                return new CustomService.Data.MASTER_NPPBKC()
                {
                    NPPBKC_ID = model.VirtualNppbckId,
                    ADDR1 = model.Address1,
                    ADDR2 = model.Address2,
                    KPPBC_ID = model.KppbcId,
                    KPPBC_ADDRESS = model.KppbcAddress,
                    REGION = model.Region,
                    REGION_DGCE = model.RegionOfficeOfDGCE,
                    DGCE_ADDRESS = model.DgceAddress,
                    TEXT_TO = model.TextTo,
                    CITY = model.City,
                    CITY_ALIAS = model.CityAlias,
                    LOCATION = model.Location,
                    IS_DELETED = false,
                    START_DATE = model.StartDate,
                    END_DATE = model.EndDate,
                    FLAG_FOR_LACK1 = model.FlagForLack1,
                    CREATED_BY = model.CreatedBy,
                    CREATED_DATE = model.CreateDate.Value,
                    MODIFIED_BY = model.ModifiedBy,
                    MODIFIED_DATE = model.ModifiedDate

                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

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
            slDocument.MergeWorksheetCells(1, 1, 1, 15);
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
            slDocument.SetCellValue(iRow, 2, "Address1");
            slDocument.SetCellValue(iRow, 3, "Address2");
            slDocument.SetCellValue(iRow, 4, "City");
            slDocument.SetCellValue(iRow, 5, "City Alias");
            slDocument.SetCellValue(iRow, 6, "Region Office of DGCE");
            slDocument.SetCellValue(iRow, 7, "Text To");
            slDocument.SetCellValue(iRow, 8, "KPPBC ID");
            slDocument.SetCellValue(iRow, 9, "Region");
            slDocument.SetCellValue(iRow, 10, "Account Number");
            slDocument.SetCellValue(iRow, 11, "Start Date");
            slDocument.SetCellValue(iRow, 12, "End Date");
            slDocument.SetCellValue(iRow, 13, "Flaging For LACK-1");
            slDocument.SetCellValue(iRow, 14, "Plant");
            slDocument.SetCellValue(iRow, 15, "Deleted");


            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 15, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcel(SLDocument slDocument, List<VirtualNppbckDetails> listData)
        {
            int iRow = 3; //starting row data

            var listPlants = _plantBll.GetAll();

            foreach (var data in listData)
            {
                string plantDesc = "";
                var plant = listPlants.Where(c => c.NPPBKC_ID == data.VirtualNppbckId);
                foreach (var plant1 in plant)
                {
                    plantDesc += plant1.WERKS + "-" + plant1.ORT01 + Environment.NewLine;
                }

                slDocument.SetCellValue(iRow, 1, data.VirtualNppbckId);
                slDocument.SetCellValue(iRow, 2, data.Address1);
                slDocument.SetCellValue(iRow, 3, data.Address2);
                slDocument.SetCellValue(iRow, 4, data.City);
                slDocument.SetCellValue(iRow, 5, data.CityAlias);
                slDocument.SetCellValue(iRow, 6, data.RegionOfficeOfDGCE);
                slDocument.SetCellValue(iRow, 7, data.TextTo);
                slDocument.SetCellValue(iRow, 8, data.KppbcId);
                slDocument.SetCellValue(iRow, 9, data.Region);
                slDocument.SetCellValue(iRow, 10, data.AcountNumber);
                slDocument.SetCellValue(iRow, 11, Utils.ConvertHelper.ConvertDateToStringddMMMyyyy(data.StartDate));
                slDocument.SetCellValue(iRow, 12, Utils.ConvertHelper.ConvertDateToStringddMMMyyyy(data.EndDate));
                slDocument.SetCellValue(iRow, 13, data.FlagForLack1 ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 14, plantDesc);
                slDocument.SetCellValue(iRow, 15, data.Is_Deleted);


                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.SetWrapText(true);

            slDocument.AutoFitColumn(1, 15);
            slDocument.SetCellStyle(3, 1, iRow - 1, 15, valueStyle);

            return slDocument;
        }

        #endregion
    }
}