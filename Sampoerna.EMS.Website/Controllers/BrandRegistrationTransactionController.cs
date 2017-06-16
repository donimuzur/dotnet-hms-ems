using Microsoft.Ajax.Utilities;
using AutoMapper;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.Market;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.CustomService.Services.BrandRegistrationTransaction;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP;
using Sampoerna.EMS.Website.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.GeneralModel;
using System.Net;
using System.Web;
using Sampoerna.EMS.Website.Utility;
using System.Globalization;
using Sampoerna.EMS.Utils;
using static Sampoerna.EMS.Core.Enums;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.FileUpload;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.Website.Models.ProductDevUpload;
using Sampoerna.EMS.Website.Models.PLANT;

namespace Sampoerna.EMS.Website.Controllers
{
    public class BrandRegistrationTransactionController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        private ProductDevelopmentService productDevelopmentService;
        //private BrandRegistrationService brandRegistrationService;
        //private PenetapanSKEPService penetapanSKEPService;
        private NppbkcManagementService nppbkcService;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBLL;
        private ICompanyBLL _companyBll;
        public BrandRegistrationTransactionController(IPageBLL pageBLL, IZaidmExNPPBKCBLL nppbkcbll, ICompanyBLL companyBll, IChangesHistoryBLL changesHistoryBll, IWorkflowHistoryBLL workflowHistoryBLL) : base(pageBLL, Enums.MenuList.BrandRegistrationTransaction)
        {
            this.mainMenu = Enums.MenuList.BrandRegistrationTransaction;
            this.refService = new SystemReferenceService();
            this.productDevelopmentService = new ProductDevelopmentService();
            //this.brandRegistrationService = new BrandRegistrationService();
            //this.penetapanSKEPService = new PenetapanSKEPService();
            this.nppbkcService = new NppbkcManagementService();
            this._nppbkcbll = nppbkcbll;
            this._changesHistoryBll = changesHistoryBll;
            this._workflowHistoryBLL = workflowHistoryBLL;
            this._companyBll = companyBll;
        }

        #region Product Development

        #region Local Helpers Product
        private ProductDevelopmentViewModel GeneratePropertiesProduct(ProductDevelopmentViewModel source, bool update)
        {
            var companyList = productDevelopmentService.GetCompanies().Select(item => new CompanyModel()
            {
                Id = item.BUKRS,
                Name = item.BUTXT
            });

            var brandList = productDevelopmentService.GetBrand().Select(item => new BrandRegistrationDetail()
            {
                PlantName = item.WERKS,
                FaCode = item.FA_CODE
            });

            var marketList = productDevelopmentService.GetMarket().Select(item => new MarketModel()
            {
                Market_Id = item.MARKET_ID,
                Market_Desc = item.MARKET_DESC
            });

            var data = source;
          
            if (!update || data == null)
            {
                data = new ProductDevelopmentViewModel();
            }

            var plantList = productDevelopmentService.FindPlantNonImport("1616");
            var selectPlantList = from s in plantList
                                  select new SelectListItem
                                  {
                                      Value = s.NAME1,
                                      Text = s.NAME1
                                  };
            var namePlantList = new SelectList(selectPlantList.GroupBy(p => p.Value).Select(g => g.First()), "Value", "Text");
            data.PlantList = namePlantList;
                            
            data.MainMenu = mainMenu;
            data.CurrentMenu = PageInfo;
            data.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;

            data.CompanyList = GenericHelpers<CompanyModel>.GenerateList(companyList, item => item.Id, item => item.Name);            
            data.BrandList = GenericHelpers<BrandRegistrationDetail>.GenerateList(brandList, item => item.PlantName, item => item.FaCode);
            data.MarketList = GenericHelpers<MarketModel>.GenerateList(marketList, item => item.Market_Id, item => item.Market_Desc);

            data.ShowActionOptions = data.IsNotViewer;
            data.EditMode = false;
            data.EnableFormInput = true;
            data.ViewModel.IsCreator = false;
            data.IsCreated = false;

            var infoUser = productDevelopmentService.FindUserDetail(CurrentUser.USER_ID);
            data.ViewModel.Creator = new UserModel();
            data.ViewModel.Creator.FirstName = infoUser.FIRST_NAME;
            data.ViewModel.Creator.LastName = infoUser.LAST_NAME;
            data.ViewModel.Creator.Email = infoUser.EMAIL;

            var materialListOld = productDevelopmentService.GetAllMaterial();
            var selectMaterialListOld = from s in materialListOld
                                        select new SelectListItem
                                        {
                                            Value = s.STICKER_CODE,
                                            Text = s.STICKER_CODE
                                        };
            var nameMaterialListOld = new SelectList(selectMaterialListOld.GroupBy(p => p.Value).Select(g => g.FirstOrDefault()), "Value", "Text");
            data.MaterialListOld = nameMaterialListOld;


            var materialListNew = productDevelopmentService.GetAllMaterialUsed();
            var selectMaterialListNew = from s in materialListNew
                                     select new SelectListItem
                                     {
                                         Value = s.STICKER_CODE,
                                         Text = s.STICKER_CODE
                                     };
            var nameMaterialListNew = new SelectList(selectMaterialListNew.GroupBy(p => p.Value).Select(g => g.FirstOrDefault()), "Value", "Text");
            data.MaterialListNew = nameMaterialListNew;

            var limit = refService.GetUploadFileSizeLimit();
            data.FileUploadLimit = (limit != null) ? Convert.ToDecimal(limit.REFF_VALUE) : 0;         

            return data;
        }


        public ActionResult GetSupportingDocumentsProduct(string company)
        {
            var formId = (long)Enums.FormList.ProdDev;
            var docs = refService.GetSupportingDocuments(formId, company);
            return PartialView("_SupportingDocumentProduct", docs.Select(x => MapSupportingDocumentModelProduct(x)));
        }


        public PartialViewResult GetDetailItem(long detailID)
        {           
            var detail = productDevelopmentService.FindProductDevDetail(detailID);
            
            var data = GeneratePropertiesProduct(null, false);
            var changeHistoryList = this._changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductDevelopment, detailID.ToString());
            data.SupportingDocuments = productDevelopmentService.GetSupportDoc(detailID, (long)Enums.FormList.ProdDev, detail.BUKRS).Select(x => MapSupportingDocumentModelProductView(x)).ToList();
            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
            data.WorkflowHistory = GetWorkflowHistoryProduct(detailID);
            
            data.DetailModel = Mapper.Map<ProductDevDetailModel>(detail);

            return PartialView("_DetailModalProduct", data);
        }

        public PartialViewResult GetDetailItemLock(long detailID)
        {
            var detail = productDevelopmentService.FindProductDevDetail(detailID);

            var data = GeneratePropertiesProduct(null, false);
            var changeHistoryList = this._changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductDevelopment, detailID.ToString());
            data.SupportingDocuments = productDevelopmentService.GetSupportDoc(detailID, (long)Enums.FormList.ProdDev, detail.BUKRS).Select(x => MapSupportingDocumentModelProductView(x)).ToList();
            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
            data.WorkflowHistory = GetWorkflowHistoryProduct(detailID);

            data.DetailModel = Mapper.Map<ProductDevDetailModel>(detail);

            return PartialView("_DetailModalLock", data);
        }
        #endregion

        #region Index
        public ActionResult IndexProductDevelopment()
        {           
            var PDSummaryModel = new PDSummaryReportViewModel();
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.User || IsCreatorPRD(CurrentUser.USER_ID))
                {
                    var users = refService.GetAllUser();
                    var documents = GetProductListOpen("", "", false, true, null);
                    PDSummaryModel = new PDSummaryReportViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        Filter = new ProductDevelopmentFilterModel(),
                        CreatorList = GlobalFunctions.GetCreatorList(),
                        PoaList = GetPoaList(refService.GetAllPOA()),
                        IsNotViewer = (IsCreatorPRD(CurrentUser.USER_ID)),//CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.User||
                        IsExciser = productDevelopmentService.IsAdminExciser(CurrentUser.USER_ID),
                        ProductOpenDoc = documents
                    };                 
                }
                else
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return View(PDSummaryModel);
        }
            #endregion

        #region Create
        public ActionResult CreateProductDevelopment()
        {

            if (!IsCreatorPRD(CurrentUser.USER_ID))
            {                
                return RedirectToAction("Unauthorized", "Error");
            }

            var data = GeneratePropertiesProduct(null, false);

            return View(data);
        }
       
        [HttpPost]
        public JsonResult CreateProduct(ProductDevelopmentViewModel model)
        {
            var formData = Request;
            var modelObj = JObject.Parse(formData["model"]);

            try
            {
                var viewModel = JsonConvert.DeserializeObject<ProductDevelopmentModel>(modelObj.ToString());
               
              
                PRODUCT_DEVELOPMENT entity = new PRODUCT_DEVELOPMENT()
                {
                   CREATED_DATE = DateTime.Now,
                   CREATED_BY = CurrentUser.USER_ID,
                   LASTMODIFIED_DATE = DateTime.Now,
                   LASTMODIFIED_BY = CurrentUser.USER_ID,               
                   PD_NO = viewModel.PD_NO,
                   NEXT_ACTION = viewModel.Next_Action
                };
               
                return Json(productDevelopmentService.CreateProduct(entity));
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        [HttpPost]
        public JsonResult CreateProductDetail(ProductDevelopmentViewModel model, long pdID)
        {
            var formData = Request;
            JArray itemArray = JArray.Parse(formData["model"]);

            try
            {
                var draftStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;              
                var count = productDevelopmentService.GetLastRecordPDDetail();

                Int64 result;
                if (count != null)
                {
                    if (count.PD_DETAIL_ID == 0)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = count.PD_DETAIL_ID;
                    }
                }
                else
                {
                    result = 0;
                }

                long tempCounter = result;
                foreach (var item in itemArray)
                {
                    tempCounter = tempCounter  + 1;
                    var reqNo = tempCounter.ToString("D10");
                    
                    long productID = pdID;

                    string brandReqNo = ((JObject)item).GetValue("brandReqNoItem").ToString();
                    string company = ((JObject)item).GetValue("companyItem").ToString();
                    string faCodeOld = ((JObject)item).GetValue("faCodeOldItem").ToString();                
                    string faCodeOldDesc = ((JObject)item).GetValue("faCodeOldDescItem").ToString();
                    string faCodeNew = ((JObject)item).GetValue("faCodeNewItem").ToString();
                    string faCodeNewDesc = ((JObject)item).GetValue("faCodeNewDescItem").ToString();
                    string hlCode = ((JObject)item).GetValue("hlCodeItem").ToString();
                    string market = ((JObject)item).GetValue("marketItem").ToString();
                    string plant = ((JObject)item).GetValue("plantItem").ToString();
                    bool IsImport = Convert.ToBoolean (((JObject)item).GetValue("isImport"));

                    PRODUCT_DEVELOPMENT_DETAIL detail = new PRODUCT_DEVELOPMENT_DETAIL();
                    detail.PD_ID = productID;
                    detail.REQUEST_NO = String.Format("{0}/{1}", reqNo.ToString(), brandReqNo);
                    detail.BUKRS = company;
                    detail.FA_CODE_OLD = faCodeOld;
                    detail.FA_CODE_OLD_DESCR = faCodeOldDesc;
                    detail.FA_CODE_NEW = faCodeNew;
                    detail.FA_CODE_NEW_DESCR = faCodeNewDesc;
                    detail.HL_CODE = hlCode;
                    detail.MARKET_ID = market;
                    detail.WERKS = plant;
                    detail.IS_IMPORT = IsImport;
                    detail.STATUS_APPROVAL = draftStatus;
                    detail.LASTMODIFIED_BY = CurrentUser.USER_ID;
                    detail.LASTMODIFIED_DATE = DateTime.Now;

                    var detailID = productDevelopmentService.CreateProductDetail(detail, (int)Enums.MenuList.ProductDevelopment, (int)Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID);

                    productDevelopmentService.GetUpdateUsedMaterial(faCodeNew, plant, true, CurrentUser.USER_ID);

                    var allItemUpload = productDevelopmentService.GetItemUploadAll();

                    var iterator = 0;
                    foreach (var itemFiles in allItemUpload)
                    {
                        iterator++;

                        if (itemFiles.ITEM_ID == iterator)
                        {
                            var fileid = itemFiles.FILE_ID;
                            productDevelopmentService.UpdateItemUpload(Convert.ToInt64(fileid), Convert.ToInt64(detailID), true);
                        }
                    }

                }
                AddMessageInfo("Data Saved Successfully.", Enums.MessageInfoType.Success);
                return Json("Detail Product Saved Successfully.");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Problem in Saving Data.", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }

        }

        #endregion

        #region Edit

        private void ExecuteEditActionProduct(ProductDevDetailModel model, ReferenceKeys.ApprovalStatus statusApproval, Enums.ActionType actionType)
        {
            try
            {
                var comment = (model.RevisionData != null) ? model.RevisionData.Comment : null;
                var updated = productDevelopmentService.ChangeStatus(model.PD_DETAIL_ID, statusApproval, (int)Enums.MenuList.ProductDevelopment, (int)actionType, (int)CurrentUser.UserRole, CurrentUser.USER_ID, comment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult EditProductDevelopment(string id)
        {
            var model = new ProductDevelopmentViewModel()
            {
                MainMenu = mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                IsExciser = productDevelopmentService.IsAdminExciser(CurrentUser.USER_ID),                
            };

            var data = GeneratePropertiesProduct(null, false);
            var obj = productDevelopmentService.FindProductDevelopment(Convert.ToInt64(id));
            var objDetail = productDevelopmentService.GetProductDetailByProductID(obj.PD_ID);
            data.ViewModel = Mapper.Map<ProductDevelopmentModel>(obj);
            data.ListProductDevDetail = new List<ProductDevDetailModel>();
            data.ListProductDevDetail = Mapper.Map<List<ProductDevDetailModel>>(objDetail);
         
            var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
            var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;

            //if (obj.CREATED_BY != CurrentUser.USER_ID)
            //{
            //    AddMessageInfo("Operation not allowed!. You are not the creator of this entry", Enums.MessageInfoType.Error);

            //    RedirectToAction("IndexProductDevelopment");
            //}


            //   data.ViewModel.IsCreator = true;
            data.ViewModel.IsCreator = CurrentUser.USER_ID == obj.CREATED_BY;
            foreach (var detail in objDetail)
            {
                var changeHistoryList = this._changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductDevelopment, detail.PD_DETAIL_ID.ToString());
                data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                data.WorkflowHistory = GetWorkflowHistoryProduct(detail.PD_DETAIL_ID);
                data.SupportingDocuments = productDevelopmentService.GetSupportDoc(detail.PD_DETAIL_ID, (long)Enums.FormList.ProdDev, detail.BUKRS).Select(x => MapSupportingDocumentModelProductView(x)).ToList();             
            }
            data.IsExciser = productDevelopmentService.IsAdminExciser(CurrentUser.USER_ID);
            data.EnableFormInput = true;
            data.EditMode = true;
            data.IsCreated = true;
            return View("EditProductDevelopment", data);
        }   

        [HttpPost]
        public JsonResult EditProduct(ProductDevelopmentViewModel model, long PD_ID)
        {
            var formData = Request;
            var modelObj = JObject.Parse(formData["model"]);

            try
            {
                var product = JsonConvert.DeserializeObject<ProductDevelopmentModel>(modelObj.ToString());

                productDevelopmentService.EditProduct(PD_ID, product.Next_Action, CurrentUser.USER_ID);

                AddMessageInfo("Data Updated Successfully.", Enums.MessageInfoType.Success);
                return Json("Update Successfully.");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        //[HttpPost]
        //public JsonResult EditProductDetail (ProductDevelopmentViewModel model, long PD_ID)
        //{
        //    var formData = Request;
        //    JArray itemArray = JArray.Parse(formData["model"]);

        //    try
        //    {
        //        var data = productDevelopmentService.FindProductDevelopment(PD_ID);
        //        var parameters = new Dictionary<string, string>();
        //        var sender = refService.GetUserEmail(CurrentUser.USER_ID);
        //        var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminApprover), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);

        //        var index = 0;
        //        var maildetail = "";
        //        var appStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;

        //        foreach (var item in itemArray)
        //        {
        //            index++;
        //            long PD_DetailID = Convert.ToInt64(((JObject)item).GetValue("ProductDetailID"));
        //            ProductDevDetailModel modelDetail = new ProductDevDetailModel();
        //            modelDetail.PD_DETAIL_ID = PD_DetailID;





        //            PRODUCT_DEVELOPMENT_DETAIL detail = new PRODUCT_DEVELOPMENT_DETAIL();
        //            detail.LASTMODIFIED_BY = CurrentUser.USER_ID;
        //            detail.LASTMODIFIED_DATE = DateTime.Now;
        //            detail.STATUS_APPROVAL = appStatus;

        //         //   productDevelopmentService.EditProductDetail(detail, (int)Enums.MenuList.ProductDevelopment, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
        //            ExecuteEditActionProduct(modelDetail, ReferenceKeys.ApprovalStatus.AwaitingAdminApproval, Enums.ActionType.WaitingForApproval);
        //        }

        //        parameters.Add("pd_no", data.PD_NO);
        //        parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy"));
        //        //parameters.Add("submitter", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
        //        parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
        //        parameters.Add("detail", maildetail);
        //        parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_VALUE);
        //        parameters.Add("url_detail_product", Url.Action("DetailProduct", "BrandRegistrationTransactionController", new { id = data.PD_ID }, this.Request.Url.Scheme));
        //        parameters.Add("url_approve_product", Url.Action("ApproveProduct", "BrandRegistrationTransactionController", new { id = data.PD_ID }, this.Request.Url.Scheme));

        //        long mailcontentId = 0;
        //        mailcontentId = (int)ReferenceKeys.EmailContent.ProductDevelopmentApprovalRequest;

        //        var mailContent = refService.GetMailContent(mailcontentId, parameters);
        //        var reff = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
        //        var sendToId = reff.REFF_VALUE;
        //        var sendTo = refService.GetUserEmail(sendToId);


        //        return Json("Detail Product Saved Successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
        //        Console.WriteLine(ex.StackTrace);
        //        return Json(false);
        //    }
        //}

        [HttpPost]
        public JsonResult EditDetailTable(ProductDevelopmentViewModel model, long PD_DetailID)
        {
            var formData = Request;
            var modelObj = JObject.Parse(formData["model"]);
            try
            {
                var appStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                var detail = JsonConvert.DeserializeObject<ProductDevDetailModel>(modelObj.ToString());
               
                productDevelopmentService.EditProductDetail(appStatus, PD_DetailID, detail.Fa_Code_Old, detail.Fa_Code_New, detail.Fa_Code_Old_Desc, detail.Fa_Code_New_Desc, detail.Hl_Code, detail.Market_Id,(int)Enums.MenuList.ProductDevelopment, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);

                return Json("Detail Product Updated.");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        [HttpPost]
        public JsonResult SubmitDetail(ProductDevDetailViewModel model, long PD_ID)
        {
            var formData = Request;
            JArray itemArray = JArray.Parse(formData["model"]);

            try
            {         
                var data = productDevelopmentService.FindProductDevelopment(PD_ID);
                var parameters = new Dictionary<string, string>();
                var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.CreatorPRD), data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME);                                          
           
                var index = 0;
                var maildetail = "";
                foreach (var item in itemArray)
                {
                    index++;
                    long PD_DetailID= Convert.ToInt64(((JObject)item).GetValue("ProductDetailID"));
                    ProductDevDetailModel modelDetail = new ProductDevDetailModel();
                    modelDetail.PD_DETAIL_ID = PD_DetailID;

                    var detail = productDevelopmentService.FindProductDevDetail(PD_DetailID);

                    maildetail += "<tr>";
                    maildetail += "<td colspan='3'>&nbsp;<b>Detail Product " + index + "</b></td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Request No</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.REQUEST_NO + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Fa Code Old & Description</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.FA_CODE_OLD + " - " + detail.FA_CODE_OLD_DESCR + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Fa Code New & Description</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.FA_CODE_NEW + " - " + detail.FA_CODE_NEW_DESCR + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Hl Code</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.HL_CODE + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Market</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.ZAIDM_EX_MARKET.MARKET_DESC + "</td>";
                    maildetail += "</tr>";

                    ExecuteApprovalActionProduct(modelDetail, ReferenceKeys.ApprovalStatus.AwaitingExciseApproval, Enums.ActionType.Submit);                                       
                }

                parameters.Add("pd_no", data.PD_NO );
                parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy"));
                parameters.Add("submitter", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                parameters.Add("detail", maildetail);
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingExciseApproval).REFF_VALUE);
                parameters.Add("url_detail_product", Url.Action("DetailProduct", "BrandRegistrationTransaction", new { id = data.PD_ID }, this.Request.Url.Scheme));
                parameters.Add("url_approve_product", Url.Action("ApproveProduct", "BrandRegistrationTransaction", new { id = data.PD_ID }, this.Request.Url.Scheme));

                long mailcontentId = 0;
                mailcontentId = (int)ReferenceKeys.EmailContent.ProductDevelopmentApprovalRequest;
                
                var mailContent = refService.GetMailContent(mailcontentId, parameters);
                var reff = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
                var sendToId = reff.REFF_VALUE;
                var sendTo = refService.GetUserEmail(sendToId);
                   
                SendMailApprovalActionProduct(ReferenceKeys.ApprovalStatus.AwaitingExciseApproval, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);

               


                AddMessageInfo("Submitted Successfully.", Enums.MessageInfoType.Success);
                return Json("Item Product Submitted.");

                
            }
            catch (Exception ex)
            {
                AddMessageInfo("Problem in Submission or Sending Email!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        [HttpPost]
        public JsonResult CancelSubmission (long PD_ID)
        {
            try
            {
                var obj = productDevelopmentService.FindProductDevelopment(PD_ID);
                var objDetail = productDevelopmentService.GetProductDetailByProductID(obj.PD_ID);
                var cancelStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Canceled).REFF_ID;

                foreach (var detail in objDetail)
                {
                    productDevelopmentService.GetUpdateUsedMaterial(detail.FA_CODE_NEW, detail.WERKS, false, CurrentUser.USER_ID);
                    productDevelopmentService.EditDetailStatusApproval(cancelStatus, detail.PD_DETAIL_ID, (int)Enums.MenuList.ProductDevelopment, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                }
                
                AddMessageInfo("Form Submission Cancelled.", Enums.MessageInfoType.Success);
                return Json("Form Submission Cancelled.");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Problem in Cancel Submission", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }

        }

        #endregion

        #region Detail
        public ActionResult DetailProduct(long id)
        {
            var data = GeneratePropertiesProduct(null, false);
            var obj = productDevelopmentService.FindProductDevelopment(Convert.ToInt64(id));
            var objDetail = productDevelopmentService.GetProductDetailByProductID(obj.PD_ID);
            data.ViewModel = Mapper.Map<ProductDevelopmentModel>(obj);
            data.ListProductDevDetail = Mapper.Map<List<ProductDevDetailModel>>(objDetail);

            foreach (var detail in objDetail)
            {
                var changeHistoryList = this._changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductDevelopment, detail.PD_DETAIL_ID.ToString());
                data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                data.WorkflowHistory = GetWorkflowHistoryProduct(detail.PD_DETAIL_ID);
                data.SupportingDocuments = productDevelopmentService.GetSupportDoc(detail.PD_DETAIL_ID, (long)Enums.FormList.ProdDev, detail.BUKRS).Select(x => MapSupportingDocumentModelProductView(x)).ToList();
            }

            data.EnableFormInput = false;
            data.EditMode = true;
       
            return View(data);
        }
        #endregion

        #region Approve & Revise
      
        private void ExecuteApprovalActionProduct(ProductDevDetailModel model, ReferenceKeys.ApprovalStatus statusApproval, Enums.ActionType actionType)
        {
            try
            {
                var comment = (model.RevisionData != null) ? model.RevisionData.Comment : null;
                var updated = productDevelopmentService.ChangeStatus(model.PD_DETAIL_ID, statusApproval, (int)Enums.MenuList.ProductDevelopment, (int)actionType, (int)CurrentUser.UserRole, CurrentUser.USER_ID, comment);
            }
            catch (Exception ex)
            {
                throw ex;
            }                
        }

        private void SendMailApprovalActionProduct(ReferenceKeys.ApprovalStatus statusApproval, string email, string subject, string sender, string display, string sendTo)
        {
            try
            {
                List<string> mailAddresses = new List<string>();
                if (statusApproval == ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                {
                    //var approvers = refService.GetAdminApprovers().ToList();
                    //foreach (var appr in approvers)
                    //{
                    //    var _email = refService.GetUserEmail(appr.REFF_VALUE.Trim());
                    //    if (!string.IsNullOrEmpty(_email))
                    //    {
                    //        mailAddresses.Add(_email);
                    //    }
                    //}
                    var exciser = productDevelopmentService.GetAdminExciser().ToList();
                    foreach (var exc in exciser)
                    {
                        var _email = refService.GetUserEmail(exc.USER_ID);
                        if (!string.IsNullOrEmpty(_email) && _email != sender)
                        {
                            mailAddresses.Add(_email);
                        }
                    }
                }
                else
                {                  
                    mailAddresses.Add(sendTo);
                }

                AddMessageInfo("Success for Submission.", Enums.MessageInfoType.Success);

                bool mailStatus = ItpiMailer.Instance.SendEmail(mailAddresses.ToArray(), null, null, null, subject, email, true, sender, display);
                if (!mailStatus)
                {
                    AddMessageInfo("Send email failed!", Enums.MessageInfoType.Warning);
                }
                else
                {
                    AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
                }
              
            }
            catch (Exception ex)
            {
                throw ex;
            }

        
        }

        public ActionResult ApproveProduct(long id)
        {
            try
            {
                var data = GeneratePropertiesProduct(null, false);
                var obj = productDevelopmentService.FindProductDevelopment(Convert.ToInt64(id));
                var objDetail = productDevelopmentService.GetProductDetailByProductID(obj.PD_ID);
                data.ViewModel = Mapper.Map<ProductDevelopmentModel>(obj);
                data.ListProductDevDetail = Mapper.Map<List<ProductDevDetailModel>>(objDetail);

                foreach (var detail in objDetail)
                {
                    var changeHistoryList = this._changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductDevelopment, detail.PD_DETAIL_ID.ToString());
                    data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                    data.WorkflowHistory = GetWorkflowHistoryProduct(detail.PD_DETAIL_ID);
                    data.SupportingDocuments = productDevelopmentService.GetSupportDoc(detail.PD_DETAIL_ID, (long)Enums.FormList.ProdDev, detail.BUKRS).Select(x => MapSupportingDocumentModelProductView(x)).ToList();
                }

                data.IsExciser = productDevelopmentService.IsAdminExciser(CurrentUser.USER_ID);
                data.EnableFormInput = false;
                data.EditMode = true;
                data.DetailModel.RevisionData = new WorkflowHistory()
                {
                    FormID = Convert.ToInt64(id),
                    FormTypeID = (int)Enums.MenuList.ProductDevelopment,
                    Action = (int)Enums.ActionType.Reject,
                    ActionBy = CurrentUser.USER_ID,
                    Role = (int)CurrentUser.UserRole
                };
                //  data.ViewDetailModel.RevisionData.Comment = "Rejection";
                return View("ApproveProduct",data);
                //data.ViewModel.IsCreator = true;
                //data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;
                //if (data.ViewModel.IsApproved)
                //{
                //    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);
                //    RedirectToAction("Index");
                //}
                //data.ApproveConfirm = new ConfirmDialogModel()
                //{
                //    Action = new ConfirmDialogModel.Button()
                //    {
                //        Id = "ApproveButtonConfirm",
                //        CssClass = "btn btn-success",
                //        Label = "Approve"
                //    },
                //    CssClass = " approve-modal product",
                //    Id = "ProductDevelopmentApproveModal",
                //    Message = String.Format("You are going to approve Product Development data. Are you sure?", data.ViewModel.PD_ID),
                //    Title = "Approve Confirmation",
                //    ModalLabel = "ApproveModalLabel"

                //};
                //data.ViewModel.RevisionData = new WorkflowHistory()
                //{
                //    FormID = Convert.ToInt64(id),
                //    FormTypeID = (int)Enums.MenuList.BrandRegistrationTransaction,
                //    Action = (int)Enums.ActionType.Reject,
                //    ActionBy = CurrentUser.USER_ID,
                //    Role = (int)CurrentUser.UserRole
                //};
              
            }
            catch (Exception ex)
            {
                return RedirectToAction("IndexProductDevelopment");
            }
        }

        [HttpPost]
        public JsonResult ApproveDetail(ProductDevDetailViewModel model, long PD_ID)
        {
            var formData = Request;
            JArray itemArray = JArray.Parse(formData["model"]);

            try
            {
                var data = productDevelopmentService.FindProductDevelopment(PD_ID);              
                var parameters = new Dictionary<string, string>();
                var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.POAExcise), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);

                var index = 0;
                var maildetail = "";
                foreach (var item in itemArray)
                {
                    index++;
                    long PD_DetailID = Convert.ToInt64(((JObject)item).GetValue("ProductDetailID"));
                    ProductDevDetailModel modelDetail = new ProductDevDetailModel();
                    modelDetail.PD_DETAIL_ID = PD_DetailID;

                    var detail = productDevelopmentService.FindProductDevDetail(PD_DetailID);

                    maildetail += "<tr>";
                    maildetail += "<td colspan='3'>&nbsp;<b>Detail Product " + index + "</b></td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Request No</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.REQUEST_NO + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Fa Code Old & Description</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.FA_CODE_OLD + " - " + detail.FA_CODE_OLD_DESCR + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Fa Code New & Description</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.FA_CODE_NEW + " - " + detail.FA_CODE_NEW_DESCR + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Hl Code</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.HL_CODE  + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Market</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.ZAIDM_EX_MARKET.MARKET_DESC + "</td>";
                    maildetail += "</tr>";

                    ExecuteApprovalActionProduct(modelDetail, ReferenceKeys.ApprovalStatus.Completed, Enums.ActionType.Approve);

                    if (data.NEXT_ACTION == (long)Enums.ProductDevelopmentAction.MapSKEP)
                    {
                        var getFa = productDevelopmentService.GetFaCodeLatestSKEP(detail.FA_CODE_OLD);

                        ZAIDM_EX_BRAND dataBrand = new ZAIDM_EX_BRAND();

                        dataBrand.FA_CODE = detail.FA_CODE_NEW;
                        dataBrand.BRAND_CE = detail.FA_CODE_NEW_DESCR;
                        dataBrand.SKEP_DATE = getFa.SKEP_DATE;

                        dataBrand.WERKS = getFa.WERKS;
                        dataBrand.STICKER_CODE = getFa.STICKER_CODE;
                        dataBrand.PER_CODE = getFa.PER_CODE;
                        dataBrand.SKEP_NO = getFa.SKEP_NO;
                        dataBrand.PROD_CODE = getFa.PROD_CODE;
                        dataBrand.SERIES_CODE = getFa.SERIES_CODE;
                        dataBrand.BRAND_CONTENT = getFa.BRAND_CONTENT;
                        dataBrand.MARKET_ID = getFa.MARKET_ID;
                        dataBrand.COUNTRY = getFa.COUNTRY;
                        dataBrand.HJE_IDR = getFa.HJE_IDR;
                        dataBrand.HJE_CURR = getFa.HJE_CURR;
                        dataBrand.TARIFF = getFa.TARIFF;
                        dataBrand.TARIF_CURR = getFa.TARIF_CURR;
                        dataBrand.COLOUR = getFa.COLOUR;
                        dataBrand.EXC_GOOD_TYP = getFa.EXC_GOOD_TYP;
                        dataBrand.CUT_FILLER_CODE = getFa.CUT_FILLER_CODE;
                        dataBrand.PRINTING_PRICE = getFa.PRINTING_PRICE;
                        dataBrand.CONVERSION = getFa.CONVERSION;
                        dataBrand.START_DATE = getFa.START_DATE;
                        dataBrand.END_DATE = getFa.END_DATE;
                        dataBrand.STATUS = getFa.STATUS;
                        dataBrand.IS_FROM_SAP = getFa.IS_FROM_SAP;
                        dataBrand.IS_DELETED = getFa.IS_DELETED;
                        dataBrand.CREATED_DATE = DateTime.Now;
                        dataBrand.CREATED_BY = data.CREATED_BY;
                        dataBrand.MODIFIED_DATE = DateTime.Now;
                        dataBrand.MODIFIED_BY = data.CREATED_BY;
                        dataBrand.PER_CODE_DESC = getFa.PER_CODE_DESC;
                        dataBrand.BAHAN_KEMASAN = getFa.BAHAN_KEMASAN;
                        dataBrand.PACKED_ADJUSTED = getFa.PACKED_ADJUSTED;

                        productDevelopmentService.CreateBrand(dataBrand);
                    }
                }

                parameters.Add("pd_no", data.PD_NO);
                parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy"));
                parameters.Add("detail", maildetail);
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_VALUE);
                parameters.Add("url_detail_product", Url.Action("DetailProduct", "BrandRegistrationTransaction", new { id = data.PD_ID }, this.Request.Url.Scheme));

                long mailcontentId = 0;
                mailcontentId = (int)ReferenceKeys.EmailContent.ProductDevelopmentApproved;

                var mailContent = refService.GetMailContent(mailcontentId, parameters);
                var reff = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
                var sendToId = reff.REFF_VALUE;
                var sendTo = data.CREATOR.EMAIL;

                SendMailApprovalActionProduct(ReferenceKeys.ApprovalStatus.Completed, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);
              
                AddMessageInfo("Approved Successfully.", Enums.MessageInfoType.Success);
                return Json("Item Product Approved.");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Problem in Approving or Sending Email!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        [HttpPost]
        public JsonResult ReviseProduct(ProductDevDetailModel model, long PD_ID, string comment)
        {
            var formData = Request;
            JArray itemArray = JArray.Parse(formData["model"]);

            try
            {
                var data = productDevelopmentService.FindProductDevelopment(PD_ID);
                var parameters = new Dictionary<string, string>();                
                var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.POAExcise), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);
            
                var index = 0;
                var maildetail = "";

                foreach (var item in itemArray)
                {
                    index++;
                    long PD_DetailID = Convert.ToInt64(((JObject)item).GetValue("ProductDetailID"));                    
                    ProductDevDetailModel modelDetail = new ProductDevDetailModel();
                    modelDetail.PD_DETAIL_ID = PD_DetailID;
                    
                    modelDetail.RevisionData = new WorkflowHistory()
                    {
                        FormID = Convert.ToInt64(PD_DetailID),
                        FormTypeID = (int)Enums.MenuList.ProductDevelopment,
                        Action = (int)Enums.ActionType.Reject,
                        ActionBy = CurrentUser.USER_ID,
                        Role = (int)CurrentUser.UserRole
                    };
                    modelDetail.RevisionData.Comment = comment;

                    var detail = productDevelopmentService.FindProductDevDetail(PD_DetailID);

                    maildetail += "<tr>";
                    maildetail += "<td colspan='3'>&nbsp;<b>Detail Product " + index + "</b></td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Request No</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.REQUEST_NO + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Fa Code Old & Description</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.FA_CODE_OLD + " - " + detail.FA_CODE_OLD_DESCR + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Fa Code New & Description</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.FA_CODE_NEW + " - " + detail.FA_CODE_NEW_DESCR + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Hl Code</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.HL_CODE + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Market</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.ZAIDM_EX_MARKET.MARKET_DESC + "</td>";
                    maildetail += "</tr>";

                    ExecuteApprovalActionProduct(modelDetail, ReferenceKeys.ApprovalStatus.Edited, Enums.ActionType.Reject);
                }
                parameters.Add("pd_no", data.PD_NO);
                parameters.Add("detail", maildetail);
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy"));
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Rejected).REFF_VALUE);                
                parameters.Add("url_detail_product", Url.Action("DetailProduct", "BrandRegistrationTransaction", new { id = data.PD_ID }, this.Request.Url.Scheme));
                parameters.Add("url_edit_product", Url.Action("EditProductDevelopment", "BrandRegistrationTransaction", new { id = data.PD_ID }, this.Request.Url.Scheme));
                parameters.Add("remark", comment);
                //parameters.Add("remark", model.RevisionData.Comment);


                long mailcontentId = 0;
                mailcontentId = (int)ReferenceKeys.EmailContent.ProductDevelopmentRejected;

                var mailContent = refService.GetMailContent(mailcontentId, parameters);
                var reff = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
                var sendToId = reff.REFF_VALUE;
                var sendTo = data.CREATOR.EMAIL;

                SendMailApprovalActionProduct(ReferenceKeys.ApprovalStatus.Edited, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);               
                AddMessageInfo("Rejected Successfully.", Enums.MessageInfoType.Success);
                return Json("Item Product Rejected.");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Problem in Rejecting or Sending Email!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
           
        }
    
        #endregion

        #region Completed and Summary Reports
        public ActionResult CompletedDocumentProduct()
        {          
            var PDSummaryModel = new PDSummaryReportViewModel();
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.User || IsCreatorPRD(CurrentUser.USER_ID))
                {
                    var users = refService.GetAllUser();
                    var documents = GetProductList("", "", false, true, null);
                    PDSummaryModel = new PDSummaryReportViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        Filter = new ProductDevelopmentFilterModel(),
                        CreatorList = GlobalFunctions.GetCreatorList(),
                        PoaList = GetPoaList(refService.GetAllPOA()),
                        IsNotViewer = (IsCreatorPRD(CurrentUser.USER_ID)),
                        IsExciser = productDevelopmentService.IsAdminExciser(CurrentUser.USER_ID),
                        ProductDocuments = documents
                    };
                }
                else
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return View(PDSummaryModel);
        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(PDSummaryReportViewModel model)
        {
            model.ProductOpenDoc = GetProductListOpen(model.Filter.Creator, model.Filter.POA, false, true, null);

            return PartialView("_PDListTableOpen", model);
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(PDSummaryReportViewModel model)
        {
            model.ProductDocuments = GetProductList(model.Filter.Creator, model.Filter.POA, false, true, null);

            return PartialView("_PDListTable", model);
        }


        public ActionResult SummaryReportsProduct()
        {
            var PDSummaryModel = new PDSummaryReportViewModel();
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.User || IsCreatorPRD(CurrentUser.USER_ID))
                {
                    var users = refService.GetAllUser();
                    var documents = GetSummaryReportList("","", false, true, null);
                    PDSummaryModel = new PDSummaryReportViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        Filter = new ProductDevelopmentFilterModel(),
                        CreatorList = GlobalFunctions.GetCreatorList(),
                        PoaList = GetPoaList(refService.GetAllPOA()),
                        IsNotViewer = (IsCreatorPRD(CurrentUser.USER_ID)),
                        ProductDevelopmentDocuments = documents
                    };
                }
                else
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return View(PDSummaryModel);
        }

        [HttpPost]
        public PartialViewResult FilterSummaryReports(PDSummaryReportViewModel model)
        {
            model.ProductDevelopmentDocuments = GetSummaryReportList(model.Filter.Creator, model.Filter.POA, false, true, null);

            return PartialView("_PDSummaryReportTable", model);
           
        }

        public void ExportXlsSummaryReports(PDSummaryReportViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryReports(model.ExportModel);

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

        private string CreateXlsSummaryReports(ProductDevelopmentExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = GetSummaryReportList( modelExport.Filter.Creator, modelExport.Filter.POA,  false, true, modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcel(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;

                if (modelExport.PD_NO)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PD_NO);
                    iColumn = iColumn + 1;
                }
                if (modelExport.CreatedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Created_Date.ToString("dd MMMM yyyy"));
                    iColumn = iColumn + 1;
                }
                if (modelExport.Next_Action)
                {
                    slDocument.SetCellValue(iRow, iColumn, EnumHelper.GetDescription((Enum)Enum.Parse(typeof(Sampoerna.EMS.Core.Enums.ProductDevelopmentAction), data.Next_Action.ToString())));
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Request_No)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Request_No);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Bukrs)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Fa_Code_Old)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Fa_Code_Old);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Fa_Code_Old_Desc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Fa_Code_Old_Desc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Fa_Code_New)
                {
                    slDocument.SetCellValue(iRow, iColumn,data.Fa_Code_New);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Fa_Code_New_Desc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Fa_Code_New_Desc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Hl_Code)
                {
                    slDocument.SetCellValue(iRow, iColumn,data.Hl_Code);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Market_Id)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.MarketDesc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Werks)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PlantName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Status)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastStatus);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);
        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, ProductDevelopmentExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.PD_NO)
            {
                slDocument.SetCellValue(iRow, iColumn, "Product Development Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.CreatedDate)
            {
                slDocument.SetCellValue(iRow, iColumn,"Request Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.Next_Action)
            {
                slDocument.SetCellValue(iRow, iColumn, "Next Action");
                iColumn = iColumn + 1;
            }           

            if (modelExport.DetailExportModel.Request_No)
            {
                slDocument.SetCellValue(iRow, iColumn, "Request Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Bukrs)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Fa_Code_Old)
            {
                slDocument.SetCellValue(iRow, iColumn, "Fa Code Old");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Fa_Code_Old_Desc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Fa Code Old Description");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Fa_Code_New)
            {
                slDocument.SetCellValue(iRow, iColumn, "Fa Code New");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Fa_Code_New_Desc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Fa Code New Description");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Hl_Code)
            {
                slDocument.SetCellValue(iRow, iColumn, "Hl Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Market_Id)
            {
                slDocument.SetCellValue(iRow, iColumn, "Market");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Werks)
            {
                slDocument.SetCellValue(iRow, iColumn, "Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Status)
            {
                slDocument.SetCellValue(iRow, iColumn, "Status");
                iColumn = iColumn + 1;
            }
            return slDocument;

        }

        private string CreateXlsFileSummaryReports(SLDocument slDocument, int iColumn, int iRow)
        {
            SLStyle styleBorder = slDocument.CreateStyle();
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(1, 1, iRow - 1, iColumn - 1, styleBorder);

            var fileName = "ProductDevelopment " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";          
            var path = Path.Combine(Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["ProductDevelopmentPath"]), fileName);
            slDocument.SaveAs(path);
            return path;
        }

        private List<vwProductDevelopmentModel> GetSummaryReportList( string Creator, string POA, bool IsCompleted, bool IsAllStatus, ProductDevelopmentExportSummaryReportsViewModel modelExport)
        {
            try
            {
                var documents = new List<vwProductDevelopmentModel>();
                var data = productDevelopmentService.GetProductDetailView();
                if (data.Any())
                {
                    if ((modelExport != null) && (modelExport.DetailExportModel.Request_No || modelExport.DetailExportModel.Fa_Code_Old || modelExport.DetailExportModel.Fa_Code_New || modelExport.DetailExportModel.Hl_Code || modelExport.DetailExportModel.Is_Import || modelExport.DetailExportModel.Market_Id || modelExport.DetailExportModel.Werks))
                    {
                        documents = data.Select(s => new vwProductDevelopmentModel
                        {
                            Next_Action = s.NEXT_ACTION,                           
                            PD_NO = s.PD_NO,                          
                            Created_By = s.CREATED_BY,
                            Created_Date = s.CREATED_DATE,
                          
                            Request_No = s.REQUEST_NO,
                            Bukrs = s.COMPANY_NAME,
                            Fa_Code_Old = s.FA_CODE_OLD,
                            Fa_Code_Old_Desc = s.FA_CODE_OLD_DESCR,
                            Fa_Code_New =s.FA_CODE_NEW,
                            Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                            Hl_Code=s.HL_CODE,
                            Market_Id = s.MARKET_ID,
                            Werks= s.WERKS,
                            Is_Import=s.IS_IMPORT,
                            Modified_By = s.LASTMODIFIED_BY,
                            Modified_Date = s.LASTMODIFIED_DATE,
                            Approved_By = s.LASTAPPROVED_BY,
                            Approved_Date = s.LASTAPPROVED_DATE,
                            Approval_Status = s.LASTAPPROVED_STATUS,
                            MarketDesc = s.MARKET_DESC,
                            PlantName = s.NAME1,
                            CompanyName = s.COMPANY_NAME,
                            LastStatus = s.LASTAPPROVED_STATUS_VALUE  
                        }).ToList();
                    }
                    else// without detail
                    {
                        documents = data.Select(s => new vwProductDevelopmentModel
                        {                         
                            Next_Action = s.NEXT_ACTION,                            
                            PD_NO = s.PD_NO,                                                                                                                                                                                                                              
                            Created_By = s.CREATED_BY,
                            Created_Date = s.CREATED_DATE
                                                                                             
                        }).Distinct().ToList();

                    }
                }
                return documents;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return null;
            }
        }

        private List<ProductDevDetailModel> GetProductList(string Creator, string POA, bool IsCompleted, bool IsAllStatus, ProductDevelopmentExportSummaryReportsViewModel modelExport)
        {
            try
            {
                var documents = new List<ProductDevDetailModel>();
                var data = productDevelopmentService.GetProductDevDetail();
                if (data.Any())
                {               
                    documents = data.Select(s => new ProductDevDetailModel
                    {                                         
                        Request_No = s.REQUEST_NO,
                        Fa_Code_Old = s.FA_CODE_OLD,
                        Fa_Code_Old_Desc = s.FA_CODE_OLD_DESCR,
                        Fa_Code_New = s.FA_CODE_NEW,
                        Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                        Hl_Code = s.HL_CODE,
                        Market_Id = s.MARKET_ID,
                        Werks = s.WERKS,
                        Is_Import = s.IS_IMPORT,
                        Modified_By = s.LASTMODIFIED_BY,
                        Modified_Date = s.LASTMODIFIED_DATE,
                        Approved_By = s.LASTAPPROVED_BY,
                        Approved_Date = s.LASTAPPROVED_DATE,
                        StatusDesc = s.APPROVAL_STATUS.REFF_VALUE,
                        PD_ID = s.PD_ID,
                        PD_NO= s.PRODUCT_DEVELOPMENT.PD_NO,
                        next_action = s.PRODUCT_DEVELOPMENT.NEXT_ACTION,
                        createdBy = s.PRODUCT_DEVELOPMENT.CREATED_BY,
                        createdDate = s.PRODUCT_DEVELOPMENT.CREATED_DATE
                         
                    }).ToList();
                }                                   
                return documents;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return null;
            }
        }

        private List<ProductDevelopmentModel> GetProductListOpen(string Creator, string POA, bool IsCompleted, bool IsAllStatus, ProductDevelopmentExportSummaryReportsViewModel modelExport)
        {
            try
            {
                var documents = new List<ProductDevelopmentModel>();
                var data = productDevelopmentService.GetProductDevelopment().OrderByDescending(item=>item.PD_ID);
                if (data.Any())
                {
                    documents = data.Select(s => new ProductDevelopmentModel
                    {                        
                        Created_By= s.CREATED_BY,
                        Created_Date = s.CREATED_DATE,
                        Next_Action=s.NEXT_ACTION,
                        PD_NO= s.PD_NO,
                        PD_ID=s.PD_ID,
                        FirstName= s.CREATOR.FIRST_NAME,
                        LastName=s.CREATOR.LAST_NAME,
                        Email = s.CREATOR.EMAIL                  
                                                
                    }).OrderByDescending(o=>o.Created_Date).ToList();
                    if (Creator != "" && Creator != null && documents.Count() > 0)
                    {
                        documents = documents.Where(w => w.Created_By.Equals(Creator)).ToList();
                    }
                    if (POA != "" && POA != null && documents.Count() > 0)
                    {
                        documents = documents.Where(w => w.Created_By.Equals(POA)).ToList();
                    }
                }
                return documents;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return null;
            }
        }
        #endregion

        #endregion
   
        #region Helper Get Data
      

        [HttpPost]
        public JsonResult GetNPPBKCListByCompanyID(string companyId)
        {
            if (String.IsNullOrEmpty(companyId))
            {
                //GET All NPPBKC
                var NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll).ToList();

                if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                {
                    NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll).Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value)).ToList();
                }

                return Json(NppbkcIdList.Select(c => c.Text).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = _nppbkcbll.GetNppbkcsByCompany(companyId);

                if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                {
                    data = data.Where(x => CurrentUser.ListUserNppbkc.Contains(x.NPPBKC_ID)).ToList();
                }

                var NppbkcIdList = data.Select(x => x.NPPBKC_ID).ToList();

                return Json(NppbkcIdList, JsonRequestBehavior.AllowGet);
            }
        }

        List<WorkflowHistoryViewModel> GetWorkflowHistoryProduct(long id)
        {
            var submittedStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval);
            var itemDetail = productDevelopmentService.FindProductDevDetail(id);
            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.ProductDevelopment;
            var workflow = this._workflowHistoryBLL.GetByFormTypeAndFormId(workflowInput).OrderBy(item => item.WORKFLOW_HISTORY_ID);
            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

            WORKFLOW_HISTORY additional = new WORKFLOW_HISTORY();
            if (itemDetail.STATUS_APPROVAL == submittedStatus.REFF_ID)
            {
                var poaExciser = productDevelopmentService.GetAdminExciser().ToList();
                var accounts = "";
                foreach (var exciser in poaExciser)
                {
                    if (accounts == "")
                    {
                        accounts += exciser;
                    }
                    else
                    {
                        accounts += ", " + exciser;
                    }
                }

                additional.ACTION_BY = accounts;
                additional.ACTION = (int)Enums.ActionType.WaitingForApproval;
                additional.ROLE = (int)Enums.UserRole.POA;
                //additional.ACTION_DATE = _CRModel.LastModifiedDate;
                workflowHistory.Add(Mapper.Map<WorkflowHistoryViewModel>(additional));
            }
            return Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

        }
    
        public List<ProductDevDetailModel> GetProductDevelopmentDetail(long PD_ID)
        {
            try
            {
                var PDDetModel = new List<ProductDevDetailModel>();
                var data = productDevelopmentService.GetProductDetailByProductID(PD_ID);
                if (data.Any())
                {
                    PDDetModel = data.Select(s => new ProductDevDetailModel
                    {
                        Fa_Code_Old = s.FA_CODE_OLD,
                        Fa_Code_New = s.FA_CODE_NEW,
                        Hl_Code = s.HL_CODE,
                        Market_Id = s.MARKET_ID,
                        Fa_Code_Old_Desc= s.FA_CODE_OLD_DESCR,
                        Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                        Werks= s.WERKS,
                        Is_Import= s.IS_IMPORT,
                        Request_No= s.REQUEST_NO,
                        Bukrs = s.BUKRS
                        
                     
                    }).ToList();
                }
                return PDDetModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<ProductDevDetailModel>();
            }
        }

        private SelectList GetPoaList(IEnumerable<CustomService.Data.POA> poaList)
        {
            var query = from x in poaList
                        select new SelectItemModel()
                        {
                            ValueField = x.POA_ID,
                            TextField = x.PRINTED_NAME
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        [HttpPost]
        public JsonResult GetNppbkc(string id)
        {
            try
            {
                var nppbkc = refService.GetNppbkc(id);
                var mapped = MapNppbkcModel(nppbkc);
                var serialized = JsonConvert.SerializeObject(mapped);
                var obj = new JObject
                {
                    new JProperty("Success", true),
                    new JProperty("Data", JObject.Parse(serialized))
                };
                var objStr = obj.ToString();
                return Json(objStr);

            }
            catch (Exception ex)
            {
                return Json(new JObject()
                {
                    new JProperty("Success", false),
                    new JProperty("Message", ex.Message)
                });
            }

        }

        [HttpPost]
        public JsonResult GetPlant(string nppbkcId)
        {
            try
            {
                var plant = productDevelopmentService.FindPlantByNppbkcID(nppbkcId);
                var mapped = MapPlantModel(plant);
                var serialized = JsonConvert.SerializeObject(mapped);
                var obj = new JObject
                {
                    new JProperty("Success", true),
                    new JProperty("Data", JObject.Parse(serialized))
                };
                var objStr = obj.ToString();
                return Json(objStr);

            }
            catch (Exception ex)
            {
                return Json(new JObject()
                {
                    new JProperty("Success", false),
                    new JProperty("Message", ex.Message)
                });
            }

        }

        private SelectList GetNppbkcList(IEnumerable<CustomService.Data.MASTER_NPPBKC> nppbkcList)
        {
            var query = from x in nppbkcList
                        select new SelectItemModel()
                        {
                            ValueField = x.NPPBKC_ID,
                            TextField = x.NPPBKC_ID
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }
        
        [HttpPost]
        public JsonResult GetLastRecordPD()
        {
            var temp = productDevelopmentService.GetLastRecordPD();

            Int64 result;
            if (temp != null)
            {
                if (temp.PD_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.PD_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetLastRecordItem()
        {
            var temp = productDevelopmentService.GetLastRecordPDDetail();       
            
            Int64 result;
            if (temp != null)
            {
                if (temp.PD_DETAIL_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.PD_DETAIL_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetSupportDoc(long formId, string bukrs)
        {
            var tempList = productDevelopmentService.FindSupportDetail(formId, bukrs);
            var selectList = from s in tempList
                             select new SelectListItem
                             {
                                 Value = s.DOCUMENT_ID.ToString(),
                                 Text = s.SUPPORTING_DOCUMENT_NAME
                             };
            var nameList = new SelectList(selectList.GroupBy(p => p.Value).Select(g => g.First()), "Value", "Text");

            return Json(nameList);
        }

        [HttpPost]
        public JsonResult GetMaterialByPlant(string plant)
        {
            var data = productDevelopmentService.GetAllMaterialByPlant(plant);
            return Json(new SelectList(data, "WERKS", "STICKER_CODE"));
        }

        [HttpPost]
        public JsonResult GetMaterialUsedByPlant(string plant)
        {
            var data = productDevelopmentService.GetAllMaterialUsedByPlant(plant);
            return Json(new SelectList(data, "WERKS", "STICKER_CODE"));
        }


        [HttpPost]
        public JsonResult GetPlantByCompanyNonImport(string bukrs)
        {         
            var data = productDevelopmentService.FindPlantNonImport(bukrs);
            return Json(new SelectList(data, "NPPBKC_ID", "NAME1"));
        }

        [HttpPost]
        public JsonResult GetPlantByCompanyImport(string bukrs)
        {
            var data = productDevelopmentService.FindPlantImport(bukrs);
            return Json(new SelectList(data, "NPPBKC_IMPORT_ID", "NAME1"));
        }

        [HttpPost]
        public JsonResult GetCodeDescription(string code)
        {            
            var tempValue = productDevelopmentService.FindItemDescription(code);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.MATERIAL_DESC == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.MATERIAL_DESC.ToString();
                }
            }
            else
            {
                result = "";
            }
            return Json(result);
        }
        
        [HttpPost]
        public string GetPlantCodeByName(string namePlant)
        {
            var tempValue = productDevelopmentService.FindPlantDescriptionByName(namePlant);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.WERKS == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.WERKS.ToString();
                }
            }
            else
            {
                result = "";
            }
            return result;
        }


        public string GetCodeDescUpload(string code)
        {
            var tempValue = productDevelopmentService.FindItemDescription(code);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.MATERIAL_DESC == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.MATERIAL_DESC.ToString();
                }
            }
            else
            {
                result = "";
            }
            return result;
        }

        public string GetMarketDescUpload(string code)
        {
            var tempValue = productDevelopmentService.FindMarketDescription(code);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.MARKET_DESC == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.MARKET_DESC.ToString();
                }
            }
            else
            {
                result = "";
            }
            return result;
        }

        public string GetPlantDescUpload(string code)
        {
            var tempValue = productDevelopmentService.FindPlantDescription(code);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.NAME1 == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.NAME1.ToString();
                }
            }
            else
            {
                result = "";
            }
            return result;
        }

        public string GetCompanyDescUpload(string code)
        {
            var tempValue = productDevelopmentService.FindCompanyDescription(code);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.BUTXT == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.BUTXT.ToString();
                }
            }
            else
            {
                result = "";
            }
            return result;
        }


        public bool IsCreatorPRD(string userID)
        {
            var isCreatorPrd = false;
            isCreatorPrd = productDevelopmentService.IsCreatorPRD(userID);
            return isCreatorPrd;
        }
        #endregion

        #region Upload Part Product Development
        [HttpPost]
        public JsonResult UploadFilesProduct()
        {
            try
            {
                Int16 countItem = 0;
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                   
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {                        
                        countItem++;
                        var stream = fileContent.InputStream;                        
                        var fileName = fileContent.FileName;
                        var type = System.IO.Path.GetFileName(file);
                        var docNumber = Request.Form.Get("doc_number").Replace("/", "_");                        
                        var docId = (!type.Contains("other")) ? Convert.ToInt64(type) : new long?();
                        var docFileName = "";
                   
                        if (!type.Contains("other"))
                        {
                            var doc = refService.GetSupportingDocument(docId.Value);
                            docFileName = (doc != null) ? doc.SUPPORTING_DOCUMENT_NAME : "";
                        }
                        else
                            docFileName = type.Split('_').GetValue(1).ToString();

                        var isGovDoc = (Request.Form.AllKeys.Contains("gov_doc")) ? Convert.ToBoolean(Request.Form.Get("gov_doc")) : false;
                        var urlPath = "~/" + System.Configuration.ConfigurationManager.AppSettings["ProductDevelopmentPath"] + docNumber;
                        var path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["ProductDevelopmentPath"] + docNumber);

                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        path = System.IO.Path.Combine(path, fileName);
                        urlPath += "/" + fileName;

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        this.AddUploadedDocItem(countItem, docFileName, urlPath, docId);                        
                    }
                }
                productDevelopmentService.AddItemUpload(this.UploadedFilesItem);
                return Json("File Added Successfully.");
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Added Failed.");
            }           
        }

        private List<PRODUCT_DEVELOPMENT_UPLOAD> UploadedFilesItem;
        private List<long> RemovedFilesItem;
        private void AddUploadedDocItem(int counter, string fileName, string url, long? docId = null)
        {
            try
            {
                if (UploadedFilesItem == null)
                {
                    UploadedFilesItem = new List<PRODUCT_DEVELOPMENT_UPLOAD>();
                }                
                var doc = new PRODUCT_DEVELOPMENT_UPLOAD()
                {
                    IS_ACTIVE = false,
                    PATH_URL = url,
                    ITEM_ID =  counter,                                
                    DOCUMENT_ID = docId,
                    FILE_NAME = fileName
                };
                UploadedFilesItem.Add(doc);                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void RemoveUploadedFile(long fileId)
        {
            if (this.RemovedFilesItem == null)
                this.RemovedFilesItem = new List<long>();
            if (this.RemovedFilesItem.IndexOf(fileId) < 0)
                this.RemovedFilesItem.Add(fileId);
        }        

        public JsonResult GetOtherDocsProduct(long detailID)
        {
            try
            {                
                var data = productDevelopmentService.GetOtherDocsFile(detailID);
                var result = data.Select(x => new ProductDevelopmentUploadModel()
                {
                    File_ID = x.FILE_ID,
                    Item_ID= x.ITEM_ID,
                    Is_Active = x.IS_ACTIVE,
                    Path_Url = x.PATH_URL,
                    Upload_Date = x.UPLOAD_DATE,
                    File_Name = x.FILE_NAME
                });
                return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(ex);
            }
        }


        [HttpPost]
        public ActionResult ImportItems()
        {
            try
            {
                var FileImport = Request.Files[0];
                var ImportedItem = new List<ProductDevDetailModel>();
                var err = "";
                if (FileImport != null)
                {
                    var allowedExtensions = new[] { ".xls", ".xlsx" };
                    var extension = Path.GetExtension(FileImport.FileName);
                    if (allowedExtensions.Contains(extension))
                    {
                        var data = (new ExcelReader()).ReadExcel(FileImport);
                        if (data != null)
                        {
                            string[] ArrItemNotin;
                            List<string> ItemNotinList = new List<string>();
                            var ItemNotIn = Request.Form.Get("item_notin");
                            if (ItemNotIn != "")
                            {
                                ArrItemNotin = ItemNotIn.Split(',');
                                if (ArrItemNotin.Count() > 0)
                                {
                                    ItemNotinList = ArrItemNotin.ToList();
                                }
                            }

                            var count = productDevelopmentService.GetLastRecordPDDetail();

                            Int64 result;
                            if (count != null)
                            {
                                if (count.PD_DETAIL_ID == 0)
                                {
                                    result = 0;
                                }
                                else
                                {
                                    result = count.PD_DETAIL_ID;
                                }
                            }
                            else
                            {
                                result = 0;
                            }

                            long tempCounter = result;
                            long row = 0;
                            foreach (var datarow in data.DataRows)
                            {
                                
                                if (datarow != null)
                                {
                                    row = row + 1;
                                    tempCounter = tempCounter + 1;
                                    var reqNo = tempCounter.ToString("D10");
                                    var monthCurrent = DateTime.Now.Month;
                                    var yearCurrent = DateTime.Now.Year;
                                    var romanMonth = Utils.MonthHelper.ConvertToRomansNumeral(monthCurrent);

                                    var tempPlant = datarow[6];                                 
                                    var getPlantDesc = productDevelopmentService.FindPlantDescription(tempPlant);                                                                        
                                    var nppbkc = productDevelopmentService.FindNppbkcDetail(getPlantDesc.NPPBKC_ID);
                                    var mapPlant = productDevelopmentService.GetPlantCompanyByPlant(tempPlant);
                                    var company = productDevelopmentService.GetCompany(mapPlant.BUKRS);
                                    var v_requestNo = String.Format("{0}/{1}/{2}/{3}/{4}", reqNo, company.BUTXT_ALIAS, nppbkc.CITY_ALIAS, romanMonth, yearCurrent);
                                    var v_requestNoPartial = String.Format("{0}/{1}/{2}/{3}", company.BUTXT_ALIAS, nppbkc.CITY_ALIAS, romanMonth, yearCurrent);
                                    var v_appStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                                    var v_appStatusDesc = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_VALUE;

                                    // check company
                                    var v_company = datarow[1];
                                    var v_company_desc = GetCompanyDescUpload(datarow[1]);
                                    if (v_company == "")
                                    {
                                        err += "*Row " + row + ": Company cannot be empty <br/>";
                                    }

                                    // check fa code old
                                    var v_facode_old = datarow[2];
                                    var v_facodedesc_old = GetCodeDescUpload(v_facode_old);
                                    var checkFaOld = productDevelopmentService.IsUnderPlant(datarow[3], datarow[6]);
                                    if (checkFaOld == false)
                                    {
                                        err += "*Row " + row + ": Fa Code Old not under Plant already defined.<br/>";
                                    }

                                    // check fa code new
                                    var v_facode_new = datarow[3];                                  
                                    var v_facodedesc_new = GetCodeDescUpload(v_facode_new);
                                    var checkFaNew = productDevelopmentService.IsUnderPlant(datarow[3], datarow[6]);
                                    if (checkFaNew == false)
                                    {
                                        err += "*Row " + row + ": Fa Code New not under Plant already defined.<br/>";
                                    }

                                    // check hl code
                                    var v_hlCode = datarow[4];
                                    if (v_hlCode == "")
                                    {
                                        err += "*Row " + row + ": Hl Code cannot be empty <br/>";
                                    }

                                    // check market
                                    var v_market = datarow[5];
                                    var v_market_desc = GetMarketDescUpload(datarow[5]);
                                    if (v_market == "")
                                    {
                                        err += "*Row " + row + ": Market cannot be empty <br/>";
                                    }

                                    // check plant
                                    var v_plant = datarow[6];                                  
                                    var v_plant_desc = GetPlantDescUpload(datarow[6]);
                                    if (v_plant == "")
                                    {
                                        err += "*Row " + row + ": Plant cannot be empty <br/>";
                                    }
                                    var checkPlant = productDevelopmentService.IsUnderCompany(datarow[6], datarow[1]);
                                    if (checkPlant == false)
                                    {
                                        err += "*Row " + row + ": Plant not under Company already defined.<br/>";
                                    }

                                    // check import
                                    var v_import = Convert.ToBoolean(datarow[7]);
                                    
                                    var itemDet = new ProductDevDetailModel
                                    {
                                        Request_No = v_requestNo,
                                        Approval_Status = v_appStatus,
                                        Bukrs = v_company,
                                        Fa_Code_Old = v_facode_old,
                                        Fa_Code_Old_Desc = v_facodedesc_old,
                                        Fa_Code_New = v_facode_new,
                                        Fa_Code_New_Desc = v_facodedesc_new,                                        
                                        Hl_Code = v_hlCode,
                                        Market_Id = v_market,
                                        Werks = v_plant,
                                        Is_Import = v_import,
                                        Company_Desc = v_company_desc,
                                        Market_Desc = v_market_desc,
                                        Plant_Desc = v_plant_desc,
                                        App_Status_Desc = v_appStatusDesc, 
                                        Request_No_Partial = v_requestNoPartial
                                    };
                                    ImportedItem.Add(itemDet);
                                }
                            }                        
                        }
                        else
                        {
                            err = "* Data is empty";
                        }
                    }
                    else
                    {
                        err = "* File extension is not allowed.";
                    }
                }

                var dataAttr = new { data = ImportedItem, attribute = new { ErrorMessage = err } };
                return Json(dataAttr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public PartialViewResult UploadFileExcelProduct(HttpPostedFileBase prodExcelfile)
        {
            var data = (new ExcelReader()).ReadExcel(prodExcelfile);
            var model = new ProductDevelopmentViewModel() { DetailModel = new ProductDevDetailModel() };
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var uploadItem = new ProductDevDetailModel();

                    try
                    {
                        //uploadItem.Company.Name = datarow[0];
                        uploadItem.Approval_Status = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                        uploadItem.Bukrs = datarow[0];
                        uploadItem.Fa_Code_Old = datarow[1];
                        uploadItem.Fa_Code_Old_Desc = GetCodeDescUpload(uploadItem.Fa_Code_Old);
                        uploadItem.Fa_Code_New = datarow[2];
                        uploadItem.Fa_Code_New_Desc = GetCodeDescUpload(uploadItem.Fa_Code_New);
                        uploadItem.Hl_Code = datarow[3];
                        uploadItem.Market_Id = datarow[4];
                        //    uploadItem.Market.Market_Desc = GetMarketDescUpload(uploadItem.Market_Id);
                        uploadItem.Werks = datarow[5];
                        //      uploadItem.Plant.PlantName = GetPlantDescUpload(uploadItem.Werks);

                        //uploadItem.Is_Import =Convert.ToBoolean(datarow[6]);

                        model.ListProductDevDetail.Add(uploadItem);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            var input = Mapper.Map<List<ProductDevDetailModel>>(model.ListProductDevDetail);
            model.ListProductDevDetail = Mapper.Map<List<ProductDevDetailModel>>(input);

            return PartialView("_ProductListItem", model);
        }
        #endregion

        #region Helper Model
        public SKEPSupportingDocumentModel MapSupportingDocumentModelSKEP(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new SKEPSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BrandRegSupportingDocumentModel MapSupportingDocumentModelBrand(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new BrandRegSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProductDevSupportingDocumentModel MapSupportingDocumentModelProduct(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new ProductDevSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = (entity.COMPANY != null) ? new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    } : null,
                    FileList = (entity.FILE_UPLOAD != null) ? AutoMapper.Mapper.Map<List<FileUploadModel>>(entity.FILE_UPLOAD).ToList() : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ProductDevSupportingDocumentModel MapSupportingDocumentModelProductView(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new ProductDevSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = (entity.COMPANY != null) ? new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    }:null,           
                    FileListUpload = (entity.PRODUCT_DEVELOPMENT_UPLOAD != null) ? AutoMapper.Mapper.Map<List<ProductDevelopmentUploadModel>>(entity.PRODUCT_DEVELOPMENT_UPLOAD).ToList() : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public ProductDevDetailModel MapProductDetailItem(CustomService.Data.PRODUCT_DEVELOPMENT_DETAIL entity)
        //{
        //    try
        //    {
        //        return new ProductDevDetailModel()
        //        {
        //            PD_DETAIL_ID = entity.PD_DETAIL_ID,
        //            Fa_Code_Old = entity.FA_CODE_OLD,
        //            Fa_Code_New = entity.FA_CODE_NEW,
        //            Hl_Code = entity.HL_CODE,                  
        //            Fa_Code_Old_Desc = entity.FA_CODE_OLD_DESCR,
        //            Fa_Code_New_Desc = entity.FA_CODE_NEW_DESCR,
        //            Is_Import = entity.IS_IMPORT,
        //            PD_ID= entity.PD_ID,
        //            Request_No = entity.REQUEST_NO,
        //            Bukrs = entity.BUKRS,

        //            Company = new CompanyModel()
        //            {
        //                Name = entity.T001.BUTXT,
        //                Id = entity.T001.BUKRS
        //            },

        //            Market = new MarketModel()
        //            {
        //                Market_Id = entity.ZAIDM_EX_MARKET.MARKET_ID,
        //                Market_Desc = entity.ZAIDM_EX_MARKET.MARKET_DESC
        //            },
     
        //            Plant = new T001WModel()
        //            {
        //                NAME1 = entity.T001W.NAME1                        
        //            }    
                                                                 
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        private PlantGeneralModel MapPlantModel(CustomService.Data.MASTER_PLANT plant)
        {
            try
            {
                return new PlantGeneralModel()
                {
                    IdPlant = plant.WERKS,
                    Name = plant.NAME1,
                    Address = plant.ADDRESS
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private NppbkcGeneralModel MapNppbkcModel(CustomService.Data.MASTER_NPPBKC nppbkc)
        {
            try
            {
                return new NppbkcGeneralModel()
                {
                    Id = nppbkc.NPPBKC_ID,
                    Region = nppbkc.REGION,
                    Address = String.Format("{0}, {1}", nppbkc.ADDR1, nppbkc.ADDR2),
                    City = nppbkc.CITY,
                    CityAlias = nppbkc.CITY_ALIAS,
                    KppbcId = nppbkc.KPPBC_ID,
                    Company = (nppbkc.COMPANY != null) ? new CompanyModel()
                    {
                        Id = nppbkc.COMPANY.BUKRS,
                        Name = nppbkc.COMPANY.BUTXT,
                        Alias = nppbkc.COMPANY.BUTXT_ALIAS,
                        Npwp = nppbkc.COMPANY.NPWP
                    } : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private POAGeneralModel MapPoaModel(CustomService.Data.POA poa)
        {
            try
            {
                return new POAGeneralModel()
                {
                    Id = poa.POA_ID,
                    Name = String.Format("{0} {1}", poa.USER_LOGIN.FIRST_NAME, poa.USER_LOGIN.LAST_NAME),
                    Address = poa.POA_ADDRESS,
                    Position = poa.TITLE
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}