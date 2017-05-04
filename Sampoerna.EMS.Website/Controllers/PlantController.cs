using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.PlantReceiveMaterial;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PlantController : BaseController
    {
        private IPlantBLL _plantBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private Enums.MenuList _mainMenu;
        private IChangesHistoryBLL _changesHistoryBll;
        
        public PlantController(IPlantBLL plantBll, IZaidmExNPPBKCBLL nppbkcBll, IZaidmExGoodTypeBLL goodTypeBll, IChangesHistoryBLL changesHistoryBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterPlant)
        {
            _plantBll = plantBll;
            _nppbkcBll = nppbkcBll;
            _goodTypeBll = goodTypeBll;
            _mainMenu = Enums.MenuList.MasterData;
            _changesHistoryBll = changesHistoryBll;
        }

        //
        // GET: /Plant/
        public ActionResult Index()
        {
            var plant = new PlantViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<DetailPlantT1001W>>(_plantBll.GetAllPlant()),
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
            };
            ViewBag.Message = TempData["message"];
            return View("Index", plant);

        }

        public ActionResult Edit(string id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }
            
            var plant = _plantBll.GetId(id);
            //NPPBKC Import for the dropdown dikosongkan default value
            
            plant.NPPBKC_IMPORT_ID = !string.IsNullOrEmpty(plant.NPPBKC_IMPORT_ID) ? plant.NPPBKC_IMPORT_ID : string.Empty;
            if (plant == null)
            {
                return HttpNotFound();
            }
            
            var detail = Mapper.Map<DetailPlantT1001W>(plant);

            var model = new PlantFormModel
            {
              
                Detail = detail
                
            };
            
            return InitialEdit(model);
        }

        public ActionResult InitialEdit(PlantFormModel model)
        {
            var dataNppbkc = _nppbkcBll.GetAll().Where(x => x.IS_DELETED != true).ToList();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.Nppbkc = new SelectList(dataNppbkc, "NPPBKC_ID", "NPPBKC_ID", model.Detail.NPPBKC_ID);
            model.NppbkcImport = new SelectList(dataNppbkc, "NPPBKC_ID", "NPPBKC_ID", model.Detail.NPPBKC_IMPORT_ID);
            model.IsMainPlantExist = IsMainPlantAlreadyExist(model.Detail.NPPBKC_ID, model.Detail.IsMainPlant,
                model.Detail.Werks);
            model.Detail.ReceiveMaterials = GetPlantReceiveMaterial(model.Detail);
            return View("Edit", model);
        }

        [HttpPost]
        public JsonResult ShowMainPlant(string nppbck1, bool? isMainPlant)
        {
            var checkIfExist = _plantBll.GetT001W(nppbck1, isMainPlant);
            var IsMainPlantExist = checkIfExist != null;
            return Json(IsMainPlantExist);
        }

        [HttpPost]
        public ActionResult Edit(PlantFormModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors);
                var errorMessage = errors.Aggregate("", (current, error) => current + (error.ErrorMessage + "\n"));
                AddMessageInfo(errorMessage, Enums.MessageInfoType.Error);
                      
                return InitialEdit(model);
            }
            var isAlreadyExistMainPlant = IsMainPlantAlreadyExist(model.Detail.NPPBKC_ID, model.Detail.IsMainPlant,
                model.Detail.Werks);
            if (isAlreadyExistMainPlant)
            {
                AddMessageInfo("Main Plant Already Set", Enums.MessageInfoType.Warning);
                return InitialEdit(model);
            }

            if (model.Detail.NPPBKC_ID == model.Detail.NPPBKC_IMPORT_ID) {
                AddMessageInfo("NPPBKC domestic cannot be the same as NPPBKC Import", Enums.MessageInfoType.Warning);
                return InitialEdit(model);
            }
            try
            {
               
                var receiveMaterial = model.Detail.ReceiveMaterials.Where(c => c.IsChecked).ToList();
                model.Detail.ReceiveMaterials = receiveMaterial;
                var t1001w = Mapper.Map<Plant>(model.Detail);
                if (t1001w.PLANT_RECEIVE_MATERIAL != null)
                {
                    var tempRecieveMaterial = t1001w.PLANT_RECEIVE_MATERIAL;
                    foreach (var rm in tempRecieveMaterial)
                    {
                        rm.ZAIDM_EX_GOODTYP = _goodTypeBll.GetById(rm.EXC_GOOD_TYP);
                    }
                    t1001w.PLANT_RECEIVE_MATERIAL = tempRecieveMaterial;
                }
                
                _plantBll.save(t1001w, CurrentUser.USER_ID);
                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {

                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
              
                return InitialEdit(model);
            }
        }
        public ActionResult Detail(string id)
        {
            var plant = _plantBll.GetId(id);

            if (plant == null)
            {
                return HttpNotFound();
            }

            var detail = Mapper.Map<DetailPlantT1001W>(plant);

            var model = new PlantFormModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_ID", plant.NPPBKC_ID),
                NppbkcImport = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_ID", plant.NPPBKC_IMPORT_ID),
                Detail = detail
            };

            model.Detail.IsNo = !model.Detail.IsMainPlant;
            model.Detail.IsYes = model.Detail.IsMainPlant;
            model.Detail.ReceiveMaterials = GetPlantReceiveMaterial(model.Detail);
            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MasterPlant, id));
            
            return View(model);

        }

        private List<PlantReceiveMaterialItemModel> GetPlantReceiveMaterial(DetailPlantT1001W plant)
        {
            var goodTypes = _goodTypeBll.GetAll();

            var planReceives = new List<PlantReceiveMaterialItemModel>();
            
                var recieve = _plantBll.GetReceiveMaterials(plant.Werks);
                foreach (var goodType in goodTypes)
                {
                    var planReceive = new PlantReceiveMaterialItemModel();
                    planReceive.EXC_GOOD_TYP = goodType.EXC_GOOD_TYP;
                    planReceive.PLANT_ID = plant.Werks;
                    planReceive.EXT_TYP_DESC = goodType.EXT_TYP_DESC;
                    planReceive.IsChecked = false;
                    if(recieve.Any(x => x.EXC_GOOD_TYP.Equals(goodType.EXC_GOOD_TYP)))
                    {
                        planReceive.IsChecked = true;
                    }
                    planReceives.Add(planReceive);
                }
            
            return planReceives;
        }

        private bool IsMainPlantAlreadyExist(string nppbkcid, bool IsMainPlant, string plantId)
        {
            if (!IsMainPlant)
                return false;
            var checkIfExist = _plantBll.GetT001W(nppbkcid, IsMainPlant);
            if (checkIfExist == null)
                return false;
            if (checkIfExist.WERKS != plantId)
                return true;
            return false;
           

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
            var listData = Mapper.Map<List<DetailPlantT1001W>>(_plantBll.GetAllPlant());

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Plant");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
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

            var fileName = "MasterData_MasterPlant" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Plant ID");
            slDocument.SetCellValue(iRow, 2, "NPPBKC NO");
            slDocument.SetCellValue(iRow, 3, "Plant Description");
            slDocument.SetCellValue(iRow, 4, "Plant Address");
            slDocument.SetCellValue(iRow, 5, "Plant City");
            slDocument.SetCellValue(iRow, 6, "Skeptis");
            slDocument.SetCellValue(iRow, 7, "Main Plant");
            slDocument.SetCellValue(iRow, 8, "Receive Material");
            slDocument.SetCellValue(iRow, 9, "Phone");
            slDocument.SetCellValue(iRow, 10, "NPPBKC Import");
            slDocument.SetCellValue(iRow, 11, "Deletion");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 11, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcel(SLDocument slDocument, List<DetailPlantT1001W> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                data.ReceiveMaterials = GetPlantReceiveMaterial(data);

                var receiveMaterial = "";
                foreach (var plantReceiveMaterialItemModel in data.ReceiveMaterials)
                {
                    if (plantReceiveMaterialItemModel.IsChecked)
                        receiveMaterial += plantReceiveMaterialItemModel.EXT_TYP_DESC + Environment.NewLine;
                }
                slDocument.SetCellValue(iRow, 1, data.Werks);
                slDocument.SetCellValue(iRow, 2, data.NPPBKC_ID);
                slDocument.SetCellValue(iRow, 3, data.PlantDescription);
                slDocument.SetCellValue(iRow, 4, data.Address);
                slDocument.SetCellValue(iRow, 5, data.Ort01);
                slDocument.SetCellValue(iRow, 6, data.Skeptis);
                slDocument.SetCellValue(iRow, 7, data.IsMainPlant ? "Yes" : "No");

                slDocument.SetCellValue(iRow, 8, receiveMaterial);
                
                slDocument.SetCellValue(iRow, 9, data.Phone);
                slDocument.SetCellValue(iRow, 10, data.NPPBKC_IMPORT_ID);
                slDocument.SetCellValue(iRow, 11, data.IsDeletedString);
                

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.SetWrapText(true);

            slDocument.AutoFitColumn(1, 11);
            slDocument.SetCellStyle(3, 1, iRow - 1, 11, valueStyle);

            return slDocument;
        }

        #endregion
    }
}