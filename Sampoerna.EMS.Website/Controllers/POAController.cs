using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.POA;

namespace Sampoerna.EMS.Website.Controllers
{
    public class POAController : BaseController
    {

        private IZaidmExPOAMapBLL _poaMapBll;
        private POABLL _poaBll;
        private IUserBLL  _userBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IPOASKBLL _poaskbll;
        private Enums.MenuList _mainMenu;
        public POAController(IPageBLL pageBLL, IZaidmExPOAMapBLL poadMapBll, POABLL poaBll, IUserBLL userBll, IChangesHistoryBLL changesHistoryBll
            ,IPOASKBLL poaskbll)
            : base(pageBLL, Enums.MenuList.POA)
        {
            _poaMapBll = poadMapBll;
            _poaBll = poaBll;
            _userBll = userBll;
            _changesHistoryBll = changesHistoryBll;
            _poaskbll = poaskbll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /POA/
        public ActionResult Index()
        {
            var poa = new POAViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<POAViewDetailModel>>(_poaBll.GetAll())
            };

            ViewBag.Message = TempData["message"];
            return View("Index", poa);
        }

        public ActionResult Create()
        {

            var poa = new POAFormModel();
            poa.MainMenu = _mainMenu;
            poa.CurrentMenu = PageInfo;
            poa.Users = GlobalFunctions.GetCreatorList();
            return View(poa);
        }

        [HttpPost]
        public ActionResult Create(POAFormModel model)
        {
           
                try
                {
                    var poa = AutoMapper.Mapper.Map<POA>(model.Detail);
                    poa.POA_ID = model.Detail.UserId;
                    poa.CREATED_BY = CurrentUser.USER_ID;
                    poa.CREATED_DATE = DateTime.Now;
                    poa.IS_ACTIVE = true;
                    if (model.Detail.PoaSKFile != null)
                    {
                        foreach (var sk in model.Detail.PoaSKFile)
                        {
                            if (sk != null)
                            {
                                var poa_sk = new POA_SK();
                                var filenamecheck = sk.FileName;
                                if (filenamecheck.Contains("\\"))
                                {
                                    poa_sk.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                                }
                                else {
                                    poa_sk.FILE_NAME = sk.FileName;
                                }
                                
                                poa_sk.FILE_PATH = SaveUploadedFile(sk, poa.ID_CARD);
                                poa.POA_SK.Add(poa_sk);
                              
                            }
                        }
                    }
                    
                    _poaBll.Save(poa);
                   
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                        );
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                         );
                    return RedirectToAction("Index");
                }
                
            

            return RedirectToAction("Create");
            
            
        }

        public ActionResult Edit(string id)
        {
            var poa = _poaBll.GetById(id);


            if (poa == null)
            {

                return HttpNotFound();
            }

            
            var model = new POAFormModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            var detail = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
            
            model.Managers = detail.Manager == null ? GlobalFunctions.GetCreatorList() : GlobalFunctions.GetCreatorList(detail.Manager.USER_ID);
            model.Users = detail.User == null? GlobalFunctions.GetCreatorList(): GlobalFunctions.GetCreatorList(detail.User.USER_ID); 
            model.Detail = detail;
            
            return View(model);
        }
        private void SetChanges(POAViewDetailModel origin, POA poa)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("TITLE", origin.Title == poa.TITLE);
            changesData.Add("USER", origin.UserId == poa.LOGIN_AS);
            changesData.Add("MANAGER", origin.ManagerId == poa.MANAGER_ID);
            changesData.Add("PHONE", origin.PoaPhone == poa.POA_PHONE);
            changesData.Add("EMAIL", origin.Email == poa.POA_EMAIL);
            changesData.Add("ADDRESS", origin.PoaAddress == poa.POA_ADDRESS);
            changesData.Add("ID CARD", origin.PoaIdCard == poa.ID_CARD) ;
            changesData.Add("PRINTED NAME", origin.PoaPrintedName == poa.PRINTED_NAME);
            changesData.Add("IS ACTIVE", origin.Is_Active.Equals(poa.IS_ACTIVE)); 

            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.POA;
                    changes.FORM_ID = poa.POA_ID.ToString();
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = CurrentUser.USER_ID;
                    changes.MODIFIED_DATE = DateTime.Now;
                    switch (listChange.Key)
                    {
                        case "TITLE":
                            changes.OLD_VALUE = origin.Title;
                            changes.NEW_VALUE = poa.TITLE;
                            break;
                        case "USER":
                            changes.OLD_VALUE = origin.UserId == null ? null : _userBll.GetUserById(origin.UserId).USER_ID;
                            changes.NEW_VALUE = string.IsNullOrEmpty(poa.LOGIN_AS) == true ? null : poa.LOGIN_AS;
                            break;
                        case "MANAGER":
                            changes.OLD_VALUE = origin.ManagerId;
                            changes.NEW_VALUE = poa.MANAGER_ID;
                            break;
                        case "PHONE":
                            changes.OLD_VALUE = origin.PoaPhone;
                            changes.NEW_VALUE = poa.POA_PHONE;
                            break;
                        case "EMAIL":
                            changes.OLD_VALUE = origin.Email;
                            changes.NEW_VALUE = poa.POA_EMAIL;
                            break;
                        case "ADDRESS":
                            changes.OLD_VALUE = origin.PoaAddress;
                            changes.NEW_VALUE = poa.POA_ADDRESS;
                            break;
                        case "ID CARD":
                            changes.OLD_VALUE = origin.PoaIdCard;
                            changes.NEW_VALUE = poa.ID_CARD;
                            break;
                        case "PRINTED NAME":
                            changes.OLD_VALUE = origin.PoaPrintedName;
                            changes.NEW_VALUE = poa.PRINTED_NAME;
                            break;
                        case "IS ACTIVE":
                            changes.OLD_VALUE = origin.Is_Active;
                            changes.NEW_VALUE = poa.IS_ACTIVE.ToString();
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                    

                }
            }
            



        } 
    
        [HttpPost]
        public ActionResult Edit(POAFormModel model)
        {
            try
            {
                var poaId = model.Detail.PoaId;
                var poa = _poaBll.GetById(poaId);
                if (model.Detail.PoaSKFile != null)
                {
                    foreach (var sk in model.Detail.PoaSKFile)
                    {
                        if (sk != null)
                        {
                            var poa_sk = new POA_SK();
                            var filenamecheck = sk.FileName;
                            if (filenamecheck.Contains("\\"))
                            {
                                poa_sk.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }
                            else
                            {
                                poa_sk.FILE_NAME = sk.FileName;
                            }
                            poa_sk.FILE_PATH = SaveUploadedFile(sk, poa.ID_CARD);
                            poa_sk.POA_ID = poaId;
                            _poaskbll.Save(poa_sk);
                        }
                    }
                }
                var origin = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
                 AutoMapper.Mapper.Map(model.Detail, poa);
                 SetChanges(origin,poa);
              
                _poaBll.Save(poa);
                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success
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
            var poa = _poaBll.GetById(id);
            if (poa == null)
            {
                return HttpNotFound();
            }
            var changeHistoryList = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.POA,id);
           
            var model = new POAFormModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            var detail = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
            model.Users = GlobalFunctions.GetCreatorList();
            model.Managers = GlobalFunctions.GetCreatorList();
            model.Detail = detail;

            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);

            var origin = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
            AutoMapper.Mapper.Map(model.Detail, poa);
            SetChanges(origin, poa);

            return View(model);

        }

        public ActionResult Delete(string id)
        {
            try
            {
                _poaBll.Delete(id);
                TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Updated;
            }
            catch (Exception ex)
            {
                TempData[Constans.SubmitType.Delete] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult GetUser(string userId)
        {
           
            return Json(_userBll.GetUserById(userId));
        }

        [HttpPost]
        public JsonResult RemoveSk(int skid)
        {

            return Json(_poaskbll.RemovePoaSk(skid));
        }

        private string SaveUploadedFile(HttpPostedFileBase file, string PoaIdCard)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            ////initialize folders in case deleted by an test publish profile
            //if (!Directory.Exists(Server.MapPath(Constans.PoaSK)))
            //    Directory.CreateDirectory(Server.MapPath(Constans.PoaSK));

            sFileName = Constans.UploadPath + Path.GetFileName( PoaIdCard + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

    }
}