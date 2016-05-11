using System;
using System.IO;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using System.Collections.Generic;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.LACK1;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserController : BaseController
    {
        private IUserBLL _bll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;

        public UserController(IUserBLL bll, IChangesHistoryBLL changesHistoryBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.USER)
        {
            _bll = bll;
            _changesHistoryBll = changesHistoryBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /User/
        public ActionResult Index()
        {
            var input = new UserInput();
            List<USER> users = _bll.GetUsers(input);
            var model = new UserViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<UserItem>>(users)
            };
            return View(model);
        }

        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            
            var user = _bll.GetUserById(id);
            var changeHistoryList = _changesHistoryBll.GetByFormTypeId(Enums.MenuList.USER);
            var model = new UserItemViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<UserItem>(user), 
                ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList)
            };

            return View("Detail",model);
        }


        #region export xls

        public void ExportMasterUsers()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterUsers();

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

        private string CreateXlsMasterUsers()
        {
            //get data
            List<USER> users = _bll.GetUsers(new UserInput());
            var listData = Mapper.Map<List<UserItem>>(users);

            var slDocument = new SLDocument();
            
            //title
            slDocument.SetCellValue(1, 1, "Master Users");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterUsers(slDocument);

            //create data
            slDocument = CreateDataExcelMasterUsers(slDocument, listData);

            var fileName = "MasterData_MasterUsers" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);
           
            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterUsers(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "User ID");
            slDocument.SetCellValue(iRow, 2, "First Name");
            slDocument.SetCellValue(iRow, 3, "Last Name");
            slDocument.SetCellValue(iRow, 4, "Phone");
            slDocument.SetCellValue(iRow, 5, "Email");


            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterUsers(SLDocument slDocument, List<UserItem> listData)
        {
            int iRow = 3; //starting row data
         
            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.USER_ID);
                slDocument.SetCellValue(iRow, 2, data.FIRST_NAME);
                slDocument.SetCellValue(iRow, 3, data.LAST_NAME);
                slDocument.SetCellValue(iRow, 4, data.PHONE);
                slDocument.SetCellValue(iRow, 5, data.EMAIL);
                
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 5);
            slDocument.SetCellStyle(3, 1, iRow - 1, 5, valueStyle);

            return slDocument;
        }

        #endregion
    }
}