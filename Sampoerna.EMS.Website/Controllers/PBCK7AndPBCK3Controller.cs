﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using iTextSharp.text.pdf.qrcode;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.PBCK7AndPBCK3;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK7AndPBCK3Controller : BaseController
    {
        private IPBCK7And3BLL _pbck7AndPbck7And3Bll;
        private IBACK1BLL _back1Bll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poaBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IPlantBLL _plantBll;
        private IBrandRegistrationBLL _brandRegistration;
        private IDocumentSequenceNumberBLL _documentSequenceNumberBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IWorkflowBLL _workflowBll;
        public PBCK7AndPBCK3Controller(IPageBLL pageBll, IPBCK7And3BLL pbck7AndPbck3Bll, IBACK1BLL back1Bll,
            IPOABLL poaBll, IZaidmExNPPBKCBLL nppbkcBll, IWorkflowBLL workflowBll, IWorkflowHistoryBLL workflowHistoryBll, IDocumentSequenceNumberBLL documentSequenceNumberBll, IBrandRegistrationBLL brandRegistrationBll, IPlantBLL plantBll)
            : base(pageBll, Enums.MenuList.PBCK7)
        {
            _pbck7AndPbck7And3Bll = pbck7AndPbck3Bll;
            _back1Bll = back1Bll;
            _mainMenu = Enums.MenuList.PBCK7;
            _poaBll = poaBll;
            _nppbkcBll = nppbkcBll;
            _plantBll = plantBll;
            _brandRegistration = brandRegistrationBll;
            _documentSequenceNumberBll = documentSequenceNumberBll;
            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
        }

        #region Index PBCK7

        //
        // GET: /PBCK7/
        public ActionResult Index()
        {
            var data = InitPbck7ViewModel(new Pbck7IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck7Type = Enums.Pbck7Type.Pbck7List,

                Detail =
                    Mapper.Map<List<DataListIndexPbck7>>(_pbck7AndPbck7And3Bll.GetAllByParam(new Pbck7AndPbck3Input()))
            });

            return View("Index", data);
        }

        #endregion

        private Pbck7IndexViewModel InitPbck7ViewModel(Pbck7IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;

        }

        [HttpPost]
        public PartialViewResult FilterPbck7Index(Pbck7IndexViewModel model)
        {
            var input = Mapper.Map<Pbck7AndPbck3Input>(model);
            input.Pbck7AndPvck3Type = Enums.Pbck7Type.Pbck7List;
            if (input.Pbck7Date != null)
            {
                input.Pbck7Date = Convert.ToDateTime(input.Pbck7Date).ToString();
            }



            var dbData = _pbck7AndPbck7And3Bll.GetAllByParam(input);

            var result = Mapper.Map<List<DataListIndexPbck7>>(dbData);

            var viewModel = new Pbck7IndexViewModel();

            viewModel.Detail = result;

            return PartialView("_Pbck7TableIndex", viewModel);
        }



        #region PBCK3

        public ActionResult ListPbck3Index()
        {
            var data = InitPbck3ViewModel(new Pbck3IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck3Type = Enums.Pbck7Type.Pbck3List,

                Detail =
                    Mapper.Map<List<DataListIndexPbck3>>(_pbck7AndPbck7And3Bll.GetAllByParam(new Pbck7AndPbck3Input()))
            });

            return View("ListPbck3Index", data);
        }

        private Pbck3IndexViewModel InitPbck3ViewModel(Pbck3IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return (model);
        }

        [HttpPost]
        public PartialViewResult FilterPbck3Index(Pbck3IndexViewModel model)
        {
            var input = Mapper.Map<Pbck7AndPbck3Input>(model);
            input.Pbck7AndPvck3Type = Enums.Pbck7Type.Pbck3List;
            if (input.Pbck3Date != null)
            {
                input.Pbck3Date = Convert.ToDateTime(input.Pbck3Date).ToString();
            }


            var dbData = _pbck7AndPbck7And3Bll.GetAllByParam(input);
            var result = Mapper.Map<List<DataListIndexPbck3>>(dbData);

            var viewModel = new Pbck3IndexViewModel();

            viewModel.Detail = result;

            return PartialView("_Pbck3TableIndex", viewModel);

        }

        #endregion

        #region Json

        [HttpPost]
        public JsonResult PoaAndPlantListPartialPbck7(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() {PoaList = listPoa, PlantList = listPlant};

            return Json(model);
        }

        [HttpPost]
        public JsonResult PoaAndPlantListPartialPbck3(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() {PoaList = listPoa, PlantList = listPlant};

            return Json(model);
        }

        #endregion

    #region Create

        public ActionResult Create()
        {
            var model = new Pbck7Pbck3CreateViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View("Create", InitialModel(model));
        }

        #endregion

        public void GetDetailPbck7(Pbck7AndPbck3Dto existingData)
        {
            existingData.Back1Dto = _pbck7AndPbck7And3Bll.GetBack1ByPbck7(existingData.Pbck7Id);
            existingData.Pbck3Dto = _pbck7AndPbck7And3Bll.GetPbck3ByPbck7Id(existingData.Pbck7Id);

            if (existingData.Pbck3Dto != null)
            {
                existingData.Back3Dto = _pbck7AndPbck7And3Bll.GetBack3ByPbck3Id(existingData.Pbck3Dto.Pbck3Id);
                existingData.Ck2Dto = _pbck7AndPbck7And3Bll.GetCk2ByPbck3Id(existingData.Pbck3Dto.Pbck3Id);
            }
            if (existingData.Back1Dto == null)
                existingData.Back1Dto = new Back1Dto();
            if (existingData.Pbck3Dto == null)
                existingData.Pbck3Dto = new Pbck3Dto();
            if (existingData.Back3Dto == null)
                existingData.Back3Dto = new Back3Dto();
            if (existingData.Ck2Dto == null)
                existingData.Ck2Dto = new Ck2Dto();
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            var existingData = _pbck7AndPbck7And3Bll.GetPbck7ById(id);
            GetDetailPbck7(existingData);
           
          
            var model = Mapper.Map<Pbck7Pbck3CreateViewModel>(existingData);
            return View("Edit", InitialModel(model));
        }
        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            var existingData = _pbck7AndPbck7And3Bll.GetPbck7ById(id);
            GetDetailPbck7(existingData);
            var model = Mapper.Map<Pbck7Pbck3CreateViewModel>(existingData);
            return View("Detail", InitialModel(model));
        }

        public void SaveBack3(Pbck7Pbck3CreateViewModel model)
        {
            var existingData = _pbck7AndPbck7And3Bll.GetPbck3ByPbck7Id(model.Id);
            if (existingData != null)
            {

                var back3Dto = new Back3Dto();
                if (model.DocumentsPostBack3 != null)
                {
                    back3Dto.Back3Document = new List<BACK3_DOCUMENT>();
                    foreach (var sk in model.DocumentsPostBack3)
                    {
                        if (sk != null)
                        {
                            var document = new BACK3_DOCUMENT();
                            var filenamecheck = sk.FileName;
                            if (filenamecheck.Contains("\\"))
                            {
                                document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }
                            else
                            {
                                document.FILE_NAME = sk.FileName;
                            }

                            document.FILE_PATH = SaveUploadedFile(sk, model.Back3Dto.Back3Number.Trim().Replace('/', '_'));
                            back3Dto.Back3Document.Add(document);

                        }
                    }
                }

                back3Dto.Back3Number = model.Back3Dto.Back3Number;
                back3Dto.Back3Date = model.Back3Dto.Back3Date;
                back3Dto.Pbck3ID = existingData.Pbck3Id;

                _pbck7AndPbck7And3Bll.InsertBack3(back3Dto);
                var ck2Dto = SaveCk2(model, existingData.Pbck3Id);
                if (existingData.Pbck3Status == Enums.DocumentStatus.GovApproved)
                {
                    existingData.Pbck3Status = Enums.DocumentStatus.Completed;
                   _pbck7AndPbck7And3Bll.InsertPbck3(existingData);
                   CreateXml(ck2Dto, model.NppbkcId, existingData.Pbck3Number);
                    
                    
                }
            }

        }


        public void CreateXml(Ck2Dto ck2, string nppbckId, string pbck3Number)
        {
            var pbck4xmlDto = new Pbck4XmlDto();
            pbck4xmlDto.NppbckId = nppbckId;
            pbck4xmlDto.CompType = "CK-2";
            pbck4xmlDto.PbckNo = pbck3Number;
            pbck4xmlDto.CompnDate = ck2.Ck2Date;
            pbck4xmlDto.CompnValue = ck2.Ck2Value.HasValue? ck2.Ck2Value.ToString() : null;
            pbck4xmlDto.CompNo = ck2.Ck2Number;
            var xmlwriter = new XMLReader.XmlPBCK4DataWriter();
            xmlwriter.CreatePbck4Xml(pbck4xmlDto);
        }

        public Ck2Dto SaveCk2(Pbck7Pbck3CreateViewModel model, int pbck3Id)
        {
            

                var ck2Dto = new Ck2Dto();
                if (model.DocumentsPostCk2 != null)
                {
                    ck2Dto.Ck2Document = new List<CK2_DOCUMENT>();
                    foreach (var sk in model.DocumentsPostCk2)
                    {
                        if (sk != null)
                        {
                            var document = new CK2_DOCUMENT();
                            var filenamecheck = sk.FileName;
                            if(filenamecheck.Contains("\\"))
                            {
                                document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }
                            else
                            {
                                document.FILE_NAME = sk.FileName;
                            }

                            document.FILE_PATH = SaveUploadedFile(sk, model.Back3Dto.Back3Number.Trim().Replace('/', '_'));
                            ck2Dto.Ck2Document.Add(document);

                        }
                    }
                }

                ck2Dto.Ck2Number = model.Ck2Dto.Ck2Number;
                ck2Dto.Ck2Date = model.Ck2Dto.Ck2Date;
                ck2Dto.Pbck3ID = pbck3Id;
                _pbck7AndPbck7And3Bll.InsertCk2(ck2Dto);

            return ck2Dto;

        }

        public void SavePbck3(Pbck7Pbck3CreateViewModel model)
        {
            var existingData = _pbck7AndPbck7And3Bll.GetPbck7ById(model.Id);
            GetDetailPbck7(existingData);
           
            if (existingData != null)
            {
               var pbck3 = new Pbck3Dto();
              
                if (existingData.Pbck3Dto != null)
                {
                    pbck3 = existingData.Pbck3Dto;
                    pbck3.Pbck3Date = model.Pbck3Dto.Pbck3Date;
                    if (model.Pbck3Dto.Pbck3GovStatus != null)
                    {
                        if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.WaitingGovApproval)
                        {
                            pbck3.Pbck3Status = Enums.DocumentStatus.GovApproved;
                        }
                    }
                    else if (model.IsSaveSubmitPbck3)
                    {
                        if (model.Pbck3Dto.Pbck3Status != Enums.DocumentStatus.Completed)
                        {
                            if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Draft)
                            {
                                pbck3.Pbck3Status = Enums.DocumentStatus.WaitingForApproval;

                            }
                        }
                       
                    }
                   
                   

                }
                else
                {
                    

                        pbck3.Pbck3Status = Enums.DocumentStatus.Draft;

                        var inputDoc = new GenerateDocNumberInput();
                        inputDoc.Month = model.Pbck3Dto.Pbck3Date.Value.Month;
                        inputDoc.Year = model.Pbck3Dto.Pbck3Date.Value.Year;
                        inputDoc.NppbkcId = existingData.NppbkcId;
                        pbck3.CreateDate = DateTime.Now;
                        pbck3.CreatedBy = CurrentUser.USER_ID;
                        pbck3.Pbck7Id = existingData.Pbck7Id;
                        pbck3.Pbck3Date = model.Pbck3Dto.Pbck3Date;
                        pbck3.Pbck3Number = _documentSequenceNumberBll.GenerateNumberNoReset(inputDoc);
                    
                   

                    
                }
                _pbck7AndPbck7And3Bll.InsertPbck3(pbck3);
                
                
            }
        }

        public void SaveBack1(Pbck7Pbck3CreateViewModel model)
        {
            var existingData = _pbck7AndPbck7And3Bll.GetPbck7ById(model.Id);
            if (existingData != null)
            {
               
                var back1Dto = new Back1Dto();
                if (model.DocumentsPostBack1 != null)
                {
                    back1Dto.Documents = new List<BACK1_DOCUMENT>();
                    foreach (var sk in model.DocumentsPostBack1)
                    {
                        if (sk != null)
                        {
                            var document = new BACK1_DOCUMENT();
                            var filenamecheck = sk.FileName;
                            if (filenamecheck.Contains("\\"))
                            {
                                document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }
                            else
                            {
                                document.FILE_NAME = sk.FileName;
                            }
                           
                            document.FILE_PATH = SaveUploadedFile(sk, model.Back1Dto.Back1Number.Trim().Replace('/', '_'));
                            back1Dto.Documents.Add(document);

                        }
                    }
                }

                back1Dto.Back1Number = model.Back1Dto.Back1Number;
                back1Dto.Back1Date = model.Back1Dto.Back1Date;
                back1Dto.Pbck7Id = existingData.Pbck7Id;
               
                _pbck7AndPbck7And3Bll.InsertBack1(back1Dto);
                if (existingData.Pbck7Status == Enums.DocumentStatus.GovApproved)
                {
                    existingData.Pbck7Status = Enums.DocumentStatus.Completed;
                    //prevent error when update pbck7
                    existingData.UploadItems = null;
                    _pbck7AndPbck7And3Bll.InsertPbck7(existingData);
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pbck7Pbck3CreateViewModel model)
        {

            if (model.Pbck7Status == Enums.DocumentStatus.GovApproved)
            {
                if (!string.IsNullOrEmpty(model.Back1Dto.Back1Number) && model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Draft)
                {
                    SaveBack1(model);
                    return RedirectToAction("Index");
                }
            }
            if (model.Pbck7Status == Enums.DocumentStatus.Completed)
            {
                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.GovApproved)
                {
                    SaveBack3(model);
                  
                }
                else
                {
                    SavePbck3(model);
                }
                return RedirectToAction("Index");
            }

            var item = AutoMapper.Mapper.Map<Pbck7AndPbck3Dto>(model);
            
            if (item.CreatedBy != CurrentUser.USER_ID)
            {
                return RedirectToAction("Detail", new {id = item.Pbck7Id});
            }
            var exItems = new Pbck7ItemUpload[item.UploadItems.Count];
            item.UploadItems.CopyTo(exItems);
            item.UploadItems = new List<Pbck7ItemUpload>();
            foreach (var items in exItems)
            {
                if (items.Id == 0)
                {
                    item.UploadItems.Add(items);
                }
            }

            
            if (item.Pbck7GovStatus == Enums.DocumentStatusGov.PartialApproved)
            {
                item.Pbck7Status = Enums.DocumentStatus.GovApproved;
            }
            if (item.Pbck7GovStatus == Enums.DocumentStatusGov.FullApproved)
            {
                item.Pbck7Status = Enums.DocumentStatus.GovApproved;
            }
            if (item.Pbck7GovStatus == Enums.DocumentStatusGov.Rejected)
            {
                item.Pbck7Status = Enums.DocumentStatus.GovRejected;
            }
            if (model.IsSaveSubmit)
            {
                if (item.Pbck7Status != Enums.DocumentStatus.Completed)
                {
                    if (item.Pbck7Status == Enums.DocumentStatus.Draft)
                    {
                        item.Pbck7Status = Enums.DocumentStatus.WaitingForApproval;
                    }
                }

            }
            item.ModifiedBy = CurrentUser.USER_ID;
            item.ModifiedDate = DateTime.Now;
            var plant = _plantBll.GetId(item.PlantId);
            item.PlantCity = plant.ORT01;
            item.PlantName = plant.NAME1;
            _pbck7AndPbck7And3Bll.InsertPbck7(item);
            if(model.IsSaveSubmit)
            {
                AddMessageInfo("Submit Success", Enums.MessageInfoType.Success);
            }
            else
            {
                AddMessageInfo("Update Success", Enums.MessageInfoType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pbck7Pbck3CreateViewModel model)
        {
            var modelDto = Mapper.Map<Pbck7AndPbck3Dto>(model);
            modelDto.CreatedBy = CurrentUser.USER_ID;
            modelDto.CreateDate = DateTime.Now;
            modelDto.Pbck7Status = Enums.DocumentStatus.Draft;
            var plant = _plantBll.GetId(model.PlantId);
            modelDto.PlantName = plant.NAME1;
            modelDto.PlantCity = plant.ORT01;
            var inputDoc = new GenerateDocNumberInput();
            inputDoc.Month = modelDto.Pbck7Date.Month;
            inputDoc.Year = modelDto.Pbck7Date.Year;
            inputDoc.NppbkcId = modelDto.NppbkcId;
            modelDto.Pbck7Number = _documentSequenceNumberBll.GenerateNumberNoReset(inputDoc);
         

            try
            {
                _pbck7AndPbck7And3Bll.InsertPbck7(modelDto);
            }
            catch (Exception ex)
            {
               AddMessageInfo(ex.ToString(), Enums.MessageInfoType.Error);
            }
          
            return RedirectToAction("Index");
        }

        private Pbck7Pbck3CreateViewModel InitialModel(Pbck7Pbck3CreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.NppbkIdList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormId = model.Id;
            workflowInput.FormNumber = model.Pbck7Number;
            workflowInput.DocumentStatus = model.Pbck7Status;
            workflowInput.NPPBKC_Id = model.NppbkcId;
            workflowInput.FormType = Enums.FormType.PBCK7;
            ;
            model.WorkflowHistoryPbck7 = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            workflowInput.FormId = model.Pbck3Dto.Pbck3Id;
            workflowInput.FormNumber = model.Pbck3Dto.Pbck3Number;
            workflowInput.DocumentStatus = model.Pbck3Dto.Pbck3Status;
            workflowInput.NPPBKC_Id = model.NppbkcId;
            workflowInput.FormType = Enums.FormType.PBCK3;

            model.WorkflowHistoryPbck3 = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput)); ;
            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Pbck7Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = model.CreatedBy,
                CurrentUser = CurrentUser.USER_ID,
                CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                DocumentNumber = model.Pbck7Number,
                NppbkcId = model.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;
            model.AllowEditAndSubmit = CurrentUser.USER_ID == model.CreatedBy;
            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }
            if (model.Pbck7Status == Enums.DocumentStatus.Completed)
            {
                model.AllowPrintDocument = true;
            }

            if (model.Pbck7Status == Enums.DocumentStatus.Completed)
            {
                //validate approve and reject
                var inputPbck3 = new WorkflowAllowApproveAndRejectInput
                {
                    DocumentStatus = model.Pbck3Dto.Pbck3Status,
                    FormView = Enums.FormViewType.Detail,
                    UserRole = CurrentUser.UserRole,
                    CreatedUser = model.CreatedBy,
                    CurrentUser = CurrentUser.USER_ID,
                    CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                    DocumentNumber = model.Pbck3Dto.Pbck3Number,
                    NppbkcId = model.NppbkcId
                };

                ////workflow

                model.AllowApproveAndRejectPbck3 = _workflowBll.AllowApproveAndReject(inputPbck3);
                model.AllowEditAndSubmitPbck3 = CurrentUser.USER_ID == model.CreatedBy;
                if (!model.AllowApproveAndRejectPbck3)
                {
                    model.AllowGovApproveAndRejectPbck3 = _workflowBll.AllowGovApproveAndReject(inputPbck3);
                    model.AllowManagerRejectPbck3 = _workflowBll.AllowManagerReject(inputPbck3);
                }
                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Completed)
                {
                    model.AllowPrintDocumentPbck3 = true;
                }
            }



            return (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(Pbck7Pbck3CreateViewModel model)
        {

            if (model.ActionType == "Approve")
            {
                return RedirectToAction("Approve", new { id = model.Id });
            }
            else if (model.ActionType == "ApprovePbck3")
            {
                return RedirectToAction("ApprovePbck3", new { id = model.Id });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Approve(int id)
        {
            var urlBuilder =
                   new System.UriBuilder(Request.Url.AbsoluteUri)
                   {
                       Path = Url.Action("Detail", "PBCK7AndPBCK3", new { id = id }),
                       Query = null,
                   };

            Uri uri = urlBuilder.Uri;
            if (uri != Request.UrlReferrer)
                return HttpNotFound();
            var item = _pbck7AndPbck7And3Bll.GetPbck7ById(id);
            
            var statusPbck7 = item.Pbck7Status;
            if (statusPbck7 != Enums.DocumentStatus.Completed)
            {
                if (statusPbck7 == Enums.DocumentStatus.WaitingForApproval)
                {
                    item.Pbck7Status = Enums.DocumentStatus.WaitingForApprovalManager;
                    item.ApprovedBy = CurrentUser.USER_ID;
                    item.ApprovedDate = DateTime.Now;
                }
                else if (statusPbck7 == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    item.Pbck7Status = Enums.DocumentStatus.WaitingGovApproval;
                    item.ApprovedByManager = CurrentUser.USER_ID;
                    item.ApprovedDateManager = DateTime.Now;
                }

                else if (statusPbck7 == Enums.DocumentStatus.WaitingGovApproval)
                {
                    item.Pbck7Status = Enums.DocumentStatus.GovApproved;
                }
            }
            

            item.UploadItems = null;
            _pbck7AndPbck7And3Bll.InsertPbck7(item);
            return RedirectToAction("Index");
        }


        public ActionResult ApprovePbck3(int id)
        {
            var urlBuilder =
                   new System.UriBuilder(Request.Url.AbsoluteUri)
                   {
                       Path = Url.Action("Detail", "PBCK7AndPBCK3", new { id = id }),
                       Query = null,
                   };

            Uri uri = urlBuilder.Uri;
            if (uri != Request.UrlReferrer)
                return HttpNotFound();
            var item = _pbck7AndPbck7And3Bll.GetPbck3ByPbck7Id(id);

            var statusPbck3 = item.Pbck3Status;
            if (statusPbck3 != Enums.DocumentStatus.Completed)
            {
                if (statusPbck3 == Enums.DocumentStatus.WaitingForApproval)
                {
                    item.Pbck3Status = Enums.DocumentStatus.WaitingForApprovalManager;
                    item.ApprovedBy = CurrentUser.USER_ID;
                    item.ApprovedDate = DateTime.Now;
                }
                else if (statusPbck3 == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    item.Pbck3Status = Enums.DocumentStatus.WaitingGovApproval;
                    item.ApprovedByManager = CurrentUser.USER_ID;
                    item.ApprovedDateManager = DateTime.Now;
                }

                else if (statusPbck3 == Enums.DocumentStatus.WaitingGovApproval)
                {
                    item.Pbck3Status = Enums.DocumentStatus.GovApproved;
                }
            }
            

            
            _pbck7AndPbck7And3Bll.InsertPbck3(item);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile, string plantId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<Pbck7ItemUpload>();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var item = new Pbck7ItemUpload();
                    item.FaCode = datarow[0];
                    item.Pbck7Qty = Convert.ToDecimal(datarow[1]);
                    item.Back1Qty = Convert.ToDecimal(datarow[2]);
                    item.FiscalYear = Convert.ToInt32(datarow[3]);
                    item.ExciseValue = Convert.ToDecimal(datarow[4]);
                    try
                    {
                        var existingBrand = _brandRegistration.GetByIdIncludeChild(plantId, item.FaCode);
                        if (existingBrand != null)
                        {
                            item.Brand = existingBrand.BRAND_CE;
                            item.SeriesValue =  existingBrand.ZAIDM_EX_SERIES.SERIES_VALUE;
                            item.ProdTypeAlias = existingBrand.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS;
                            item.Content = Convert.ToInt32(existingBrand.BRAND_CONTENT);
                            item.Hje = existingBrand.HJE_IDR;
                            item.Tariff = existingBrand.TARIFF;
                        }
                    }
                    catch (Exception)
                    {


                    }
                    finally
                    {
                        model.Add(item);
                    }

                   
                }
            }
            return Json(model);
        }

        private string SaveUploadedFile(HttpPostedFileBase file, string back1Num)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";


            sFileName = Constans.UploadPath + System.IO.Path.GetFileName("BACK1_" + back1Num + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + System.IO.Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

    }

}