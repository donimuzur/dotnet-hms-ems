using System.IO;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.POA;
using Sampoerna.EMS.Website.Models.POAMap;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class POAMapController : BaseController
    {
        private IPOAMapBLL _poaMapBLL;
        private IChangesHistoryBLL _changeHistoryBll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poabll;
        private IPlantBLL _plantbll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
       
        public POAMapController(IPageBLL pageBLL, IPOABLL poabll, IPOAMapBLL poaMapBll, IZaidmExNPPBKCBLL nppbkcbll,IPlantBLL plantbll, IChangesHistoryBLL changeHistorybll) 
            : base(pageBLL, Enums.MenuList.POAMap) 
        {
            _poaMapBLL = poaMapBll;
            _changeHistoryBll = changeHistorybll;
            _mainMenu = Enums.MenuList.MasterData;
            _nppbkcbll = nppbkcbll;
            _poabll = poabll;
            _plantbll = plantbll;
        }
        //
        // GET: /POA/
        public ActionResult Index()
        {
            var model = new PoaMapIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.PoaMaps = Mapper.Map<List<POA_MAPDto>>(_poaMapBLL.GetAll());
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false);
            return View("Index", model);
        }

        //
        // GET: /POAMap/Details/5
        public ActionResult Details(int id)
        {
            var existingData = _poaMapBLL.GetById(id);
            var model = new PoaMapDetailViewModel
            {
                PoaMap = Mapper.Map<POA_MAPDto>(existingData),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            return View("Detail", model);
        }

        //
        // GET: /POAMap/Create
        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new PoaMapDetailViewModel
            {
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
                NppbckIds = GlobalFunctions.GetNppbkcAll(_nppbkcbll),
                Plants = GlobalFunctions.GetPlantAll(),
                POAs = GlobalFunctions.GetPoaAll(_poabll)
            };
            return View("Create",model);
        }

        //
        // POST: /POAMap/Create
        [HttpPost]
        public ActionResult Create(PoaMapDetailViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var existingData = _poaMapBLL.GetByNppbckId(model.PoaMap.NPPBKC_ID, model.PoaMap.WERKS, model.PoaMap.POA_ID);
                if (existingData != null)
                {
                    AddMessageInfo("data already exist", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Create");
                }
                var data = Mapper.Map<POA_MAP>(model.PoaMap);
                data.CREATED_BY = CurrentUser.USER_ID;
                data.CREATED_DATE = DateTime.Now;
                _poaMapBLL.Save(data);

                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                     );
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );
              
               
                return View(model);
            }
        }

        //
        // GET: /POAMap/Edit/5
        public ActionResult Edit(int id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Details", new { id });
            }

            var existingData = _poaMapBLL.GetById(id);
            var model = new PoaMapDetailViewModel
            {
                PoaMap = Mapper.Map<POA_MAPDto>(existingData),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            model.NppbckIds = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.Plants = GlobalFunctions.GetPlantAll();
            model.POAs = GlobalFunctions.GetPoaAll(_poabll);
            return View("Edit", model);
        }

        
        public ActionResult Delete(int id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            try
            {
                
                _poaMapBLL.Delete(id);

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

        //
        // POST: /POAMap/Create
        [HttpPost]
        public ActionResult Edit(PoaMapDetailViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var existingData = _poaMapBLL.GetByNppbckId(model.PoaMap.NPPBKC_ID, model.PoaMap.WERKS, model.PoaMap.POA_ID);
                if (existingData != null)
                {
                    AddMessageInfo("data already exist", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Create");
                }
                var data = Mapper.Map<POA_MAP>(model.PoaMap);
                data.POA_MAP_ID = model.PoaMap.POA_MAP_ID;
                data.CREATED_BY = CurrentUser.USER_ID;
                data.CREATED_DATE = DateTime.Now;
                _poaMapBLL.Save(data);

                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success
                     );
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );


                return View(model);
            }
        }

        [HttpPost]
        public JsonResult GetPlantOfNppbck(string nppbkcId)
        {
            //var data = _nppbkcbll.GetById(nppbkcId).T001W;
            var data = _plantbll.GetPlantByNppbkc(nppbkcId);
            if (data == null)
            {
                return null;
            }
            return Json(new SelectList(data, "WERKS", "NAME1"));
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
            var listData = Mapper.Map<List<POA_MAPDto>>(_poaMapBLL.GetAll());

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master POA Map");
            slDocument.MergeWorksheetCells(1, 1, 1, 3);
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

            var fileName = "MasterData_MasterPoaMap" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "NPPBKC ID");
            slDocument.SetCellValue(iRow, 2, "PLANT");
            slDocument.SetCellValue(iRow, 3, "POA");
            

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 3, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcel(SLDocument slDocument, List<POA_MAPDto> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.NPPBKC_ID);
                slDocument.SetCellValue(iRow, 2, data.WERKS + Constans.DelimeterSelectItem + data.PLANT_NAME);
                slDocument.SetCellValue(iRow, 3, data.POA_ID + Constans.DelimeterSelectItem + data.POA_NAME);
                
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 3);
            slDocument.SetCellStyle(3, 1, iRow - 1, 3, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
