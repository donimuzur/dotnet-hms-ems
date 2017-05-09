using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf.qrcode;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.Material;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Utils;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MaterialController : BaseController
    {
        private IMaterialBLL _materialBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IUnitOfMeasurementBLL _unitOfMeasurementBll;
        public MaterialController(IPageBLL pageBLL, IUnitOfMeasurementBLL unitOfMeasurementBll, IZaidmExGoodTypeBLL goodTypeBll, IMaterialBLL materialBll, IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.MaterialMaster)
        {
            _materialBll = materialBll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
            _goodTypeBll = goodTypeBll;
            _unitOfMeasurementBll = unitOfMeasurementBll;
        }

        private MaterialCreateViewModel InitCreateModel(MaterialCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetVirtualPlantListMultiSelect();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.BaseUOM = GlobalFunctions.GetUomList(_unitOfMeasurementBll);
            model.CurrencyList = GlobalFunctions.GetCurrencyList();
            model.ConversionUomList = GlobalFunctions.GetConversionUomList();
            return model;
        }

        //
        // GET: /Material/
        public ActionResult Index()
        {
            var model = new MaterialListViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.GoodType = EnumHelper.GetDescription(Enums.GoodsType.HasilTembakau);

            var data = _materialBll.getAllMaterial(model.GoodType);
            model.Details = AutoMapper.Mapper.Map<List<MaterialDetails>>(data);
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false);
            ViewBag.Message = TempData["message"];
            return View("Index", model);

        }

        //
        // GET: /Material/Details/5
        public ActionResult Details(string mn, string p)
        {

            var model = new MaterialDetailViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var data = _materialBll.getByID(mn, p);
            //Mapper.Map(data,model);
            model = Mapper.Map<MaterialDetailViewModel>(data);

            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MaterialMaster, mn + p));
            model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();
            model.HjeStr = model.Hje == null ? string.Empty : model.Hje.ToString();
            model.TariffStr = model.Tariff == null ? string.Empty : model.Tariff.ToString();


            model = InitDetailModel(model);

            if (model.IsDeleted.HasValue && model.IsDeleted.Value)
            {
                model.IsAllowDelete = false;
            }
            else
            {
                if (model.IsFromSap.HasValue && model.IsFromSap.Value)
                {
                    model.IsAllowDelete = false;
                }
                else
                {
                    model.IsAllowDelete = true;
                }
            }

            return View("Details", model);
        }


        private MaterialEditViewModel InitEditModel(MaterialEditViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.BaseUOM = GlobalFunctions.GetUomList(_unitOfMeasurementBll);
            model.ConversionUomList = GlobalFunctions.GetConversionUomList();
            model.CurrencyList = GlobalFunctions.GetCurrencyList();
            return model;
        }

        private MaterialDetailViewModel InitDetailModel(MaterialDetailViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;



            return model;
        }

        //
        // GET: /Material/Create
        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new MaterialCreateViewModel();
            InitCreateModel(model);
            return View(model);
        }



        //  POST: /Material/Create
        [HttpPost]
        public ActionResult Create(MaterialCreateViewModel data)
        {

            try
            {
                // TODO: Add insert logic here
                //if (ModelState.IsValid)
                //{


                var plantIds = data.PlantId;
                foreach (var plant in plantIds)
                {
                    var model = Mapper.Map<MaterialDto>(data);


                    model.WERKS = plant;
                    if (model.MATERIAL_UOM != null)
                    {
                        foreach (var uom in model.MATERIAL_UOM)
                        {
                            uom.STICKER_CODE = model.STICKER_CODE;
                            uom.WERKS = model.WERKS;
                            uom.MEINH = HttpUtility.UrlDecode(uom.MEINH);
                        }
                    }
                    model.CREATED_BY = CurrentUser.USER_ID;
                    model.CREATED_DATE = DateTime.Now;
                    model.HJE = data.HjeStr == null ? 0 : Convert.ToDecimal(data.HjeStr);
                    model.TARIFF = data.TariffStr == null ? 0 :  Convert.ToDecimal(data.TariffStr);
                    

                    var output = _materialBll.Save(model, CurrentUser.USER_ID);
                    if (!output.Success)
                    {
                        AddMessageInfo(output.ErrorMessage, Enums.MessageInfoType.Error
                            );

                    }
                    else
                    {
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                   );
                    }

                }
                return RedirectToAction("Index");
                //}

                //return RedirectToAction("Create"); 

            }
            catch (Exception ex)
            {
                InitCreateModel(data);
                return View(data);

            }
        }

        //
        // GET: /Material/Edit/5
        public ActionResult Edit(string mn, string p)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Details", new { mn, p });
            }

            var data = _materialBll.getByID(mn, p);



            //if (data.IS_FROM_SAP)
            //{

            //    return RedirectToAction("Details", new {mn=mn, p=p});
            //}
            //else {

            var model = Mapper.Map<MaterialEditViewModel>(data);

            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, mn + p));
            model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();
            model.HjeStr = model.Hje == null ? string.Empty : model.Hje.ToString();
            model.TariffStr = model.Tariff == null ? string.Empty : model.Tariff.ToString();

            InitEditModel(model);

            return View("Edit", model);
            //}


        }

        //
        // POST: /Material/Edit/5
        [HttpPost]
        public ActionResult Edit(MaterialEditViewModel model)
        {
            try
            {
                // TODO: Add update logic here

                var dataexist = _materialBll.getByID(model.MaterialNumber, model.PlantId);


                if (dataexist == null)
                {
                    return RedirectToAction("Index");
                }

                if (model.MaterialUom != null)
                {
                    foreach (var matUom in model.MaterialUom)
                    {
                        var uom = new MATERIAL_UOM();
                        uom.STICKER_CODE = model.MaterialNumber;
                        uom.WERKS = model.PlantId;
                        uom.UMREN = matUom.Umren;
                        uom.UMREZ = matUom.Umrez;
                        uom.MEINH = HttpUtility.UrlDecode(matUom.Meinh);

                        _materialBll.SaveUoM(uom, CurrentUser.USER_ID);
                    }
                }

                var data = AutoMapper.Mapper.Map<MaterialDto>(model);
                data.HJE = model.HjeStr == null ? 0 : Convert.ToDecimal(model.HjeStr);
                data.TARIFF = model.TariffStr == null ? 0 : Convert.ToDecimal(model.TariffStr);


                var output = _materialBll.Save(data, CurrentUser.USER_ID);
                if (!output.Success)
                {
                    AddMessageInfo(output.ErrorMessage, Enums.MessageInfoType.Error);
                    model.MainMenu = Enums.MenuList.MasterData;
                    model.CurrentMenu = PageInfo;
                    model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, model.MaterialNumber + model.PlantId));
                    model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();
                    model.HjeStr = model.Hje == null ? string.Empty : model.Hje.ToString();
                    model.TariffStr = model.Tariff == null ? string.Empty : model.Tariff.ToString();

                    InitEditModel(model);
                    return View(model);
                }
                
                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
                



                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }



        //
        // POST: /Material/Delete/5

        public ActionResult Delete(string mn, string p)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            try
            {
                // TODO: Add delete logic here
                _materialBll.Delete(mn, p, CurrentUser.USER_ID);
                TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Detail", new { mn = mn, p = p });
            }
        }

        [HttpPost]
        public JsonResult RemoveMaterialUom(int materialUomId, string materialnumber, string plant)
        {
            return Json(_materialBll.DeleteMaterialUom(materialUomId, CurrentUser.USER_ID, materialnumber, plant));
        }

        [HttpPost]
        public PartialViewResult FilterMaterialIndex(MaterialListViewModel model)
        {
            var data = _materialBll.getAllMaterial(model.GoodType);
            model.Details = AutoMapper.Mapper.Map<List<MaterialDetails>>(data);
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false);

            return PartialView("_MaterialList", model);
        }

        #region export xls

        public void ExportXlsFile(MaterialListViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsFile(model);

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

        private string CreateXlsFile(MaterialListViewModel model)
        {
            //var data = _materialBll.getByID(mn, p);
            ////Mapper.Map(data,model);
            //model = Mapper.Map<MaterialDetailViewModel>(data);

            var data = _materialBll.getAll();
            if(model.GoodType != null) data = data.Where(c=>c.EXC_GOOD_TYP == model.GoodType).ToList();
            //get data
            var listData = Mapper.Map<List<MaterialDetailViewModel>>(data);

            var slDocument = new SLDocument();

            string goodTypeName = "";
            if (listData.Count > 0)
                goodTypeName = "-" + listData[0].GoodTypeName;
             
                
            //create filter
            slDocument.SetCellValue(1, 1, "Excisable Goods Type");
            slDocument.SetCellValue(1, 2, ": " + model.GoodType + goodTypeName);

            //title
            slDocument.SetCellValue(2, 1, "Material Master");
            slDocument.MergeWorksheetCells(2, 1, 2, 15);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(2, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcel(slDocument);

            //create data
            slDocument = CreateDataExcel(slDocument, listData);

            var fileName = "MasterData_MaterialMaster" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument)
        {
            int iRow = 3;

            slDocument.SetCellValue(iRow, 1, "Plant");
            slDocument.SetCellValue(iRow, 2, "Material Number");
            slDocument.SetCellValue(iRow, 3, "Material Group");
            slDocument.SetCellValue(iRow, 4, "Material Desc");
            slDocument.SetCellValue(iRow, 5, "Purchasing Group");
            slDocument.SetCellValue(iRow, 6, "Base UOM (SAP)");
            slDocument.SetCellValue(iRow, 7, "Excisable Good Type");
    
            slDocument.SetCellValue(iRow, 8, "Issue Storage Loc");
            slDocument.SetCellValue(iRow, 9, "Tariff");
            slDocument.SetCellValue(iRow, 10, "Tariff Currency");
            slDocument.SetCellValue(iRow, 11, "HJE");
            slDocument.SetCellValue(iRow, 12, "HJE Currency");
            slDocument.SetCellValue(iRow, 13, "Converted UOM - Conversion");
            
          
            slDocument.SetCellValue(iRow, 14, "Plant Deletion");
            slDocument.SetCellValue(iRow, 15, "Client Deletion");
            


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

        private SLDocument CreateDataExcel(SLDocument slDocument, List<MaterialDetailViewModel> listData)
        {
            int iRow = 4; //starting row data

            foreach (var data in listData)
            {
                string materialUom = "";
                foreach (var materialUomDetailse in data.MaterialUom)
                {
                    materialUom += materialUomDetailse.Meinh + " - " + materialUomDetailse.UmrenStr + Environment.NewLine;
                }
                slDocument.SetCellValue(iRow, 1, data.PlantName);
                slDocument.SetCellValue(iRow, 2, data.MaterialNumber);
                slDocument.SetCellValue(iRow, 3, data.MaterialGroup);
                slDocument.SetCellValue(iRow, 4, data.MaterialDesc);
                slDocument.SetCellValue(iRow, 5, data.PurchasingGroup);
                slDocument.SetCellValue(iRow, 6, data.UomName);
                slDocument.SetCellValue(iRow, 7, data.GoodTypeName);
                slDocument.SetCellValue(iRow, 8, data.IssueStorageLoc);
                if (data.Tariff == null) slDocument.SetCellValue(iRow, 9, string.Empty);
                else slDocument.SetCellValue(iRow, 9, data.Tariff.Value.ToString("N2"));
                slDocument.SetCellValue(iRow, 10, data.Tariff_Curr);
                if (data.Hje == null) slDocument.SetCellValue(iRow, 11, string.Empty);
                else slDocument.SetCellValue(iRow, 11, data.Hje.Value.ToString("N2"));
                slDocument.SetCellValue(iRow, 12, data.Hje_Curr);
                slDocument.SetCellValue(iRow, 13, materialUom);
                slDocument.SetCellValue(iRow, 14, data.IsPlantDelete ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 15, data.IsClientDelete ? "Yes" : "No");
                
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
            slDocument.SetCellStyle(4, 1, iRow - 1, 15, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
