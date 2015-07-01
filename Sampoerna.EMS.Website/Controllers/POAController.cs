using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
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
        private IZaidmExPOABLL _poaBll;
        private IUserBLL _userBll;
        private IChangesHistoryBLL _changesHistoryBll;
        public POAController(IPageBLL pageBLL, IZaidmExPOAMapBLL poadMapBll, IZaidmExPOABLL poaBll, IUserBLL userBll, IChangesHistoryBLL changesHistoryBll
            )
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _poaMapBll = poadMapBll;
            _poaBll = poaBll;
            _userBll = userBll;
            _changesHistoryBll = changesHistoryBll;
        }

        //
        // GET: /POA/
        public ActionResult Index()
        {
            var poa = new POAViewModel
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<POAViewDetailModel>>(_poaBll.GetAll())
            };

            ViewBag.Message = TempData["message"];
            return View("Index", poa);
        }

        public ActionResult Create()
        {

            var poa = new POAFormModel();
            poa.MainMenu = Enums.MenuList.MasterData;
            poa.CurrentMenu = PageInfo;
            poa.Users = GlobalFunctions.GetCreatorList();
            return View(poa);
        }

        [HttpPost]
        public ActionResult Create(POAFormModel model)
        {
           
                try
                {
                    var poa = AutoMapper.Mapper.Map<ZAIDM_EX_POA>(model.Detail);
                    poa.IS_FROM_SAP = false;
                    _poaBll.Save(poa);
                    TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData[Constans.SubmitType.Save] = ex.Message;
                    return View();
                }
                
            

            return RedirectToAction("Create");
            
            
        }

        public ActionResult Edit(int id)
        {
            var poa = _poaBll.GetById(id);


            if (poa == null)
            {

                return HttpNotFound();
            }

            if (poa.IS_DELETED == true)
            {
                return RedirectToAction("Detail", "POA", new {id = poa.POA_ID});
            }
            var model = new POAFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            var detail = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
            
            model.Managers = detail.Manager == null ? GlobalFunctions.GetCreatorList() : GlobalFunctions.GetCreatorList(detail.Manager.USER_ID);
            model.Users = detail.User == null? GlobalFunctions.GetCreatorList(): GlobalFunctions.GetCreatorList(detail.User.USER_ID); 
            model.Detail = detail;
            return View(model);
        }
        private void SetChanges(POAViewDetailModel origin, ZAIDM_EX_POA poa)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("TITLE", (origin.Title == null ? true : origin.Title.Equals(poa.TITLE)));
            changesData.Add("USER", (origin.UserId == null ? true : origin.UserId.Equals(poa.USER_ID)));
            changesData.Add("MANAGER", (origin.ManagerId == null ? true : origin.ManagerId.Equals(poa.MANAGER_ID)));
            changesData.Add("PHONE", (origin.PoaPhone == null ? true : origin.PoaPhone.Equals(poa.POA_PHONE)));
            changesData.Add("EMAIL", (origin.Email == null ? true : origin.Email.Equals(poa.EMAIL)));
            changesData.Add("ADDRESS", (origin.PoaAddress == null ? true : origin.PoaAddress.Equals(poa.POA_ADDRESS)));
            changesData.Add("ID_CARD", (origin.PoaIdCard == null ? true : origin.PoaIdCard.Equals(poa.POA_ID_CARD))); ;

            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.POA;
                    changes.FORM_ID = poa.POA_ID;
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
                            changes.OLD_VALUE = origin.UserId == null ? null : origin.UserId.ToString();
                            changes.NEW_VALUE = poa.USER_ID == null ? null : poa.USER_ID.ToString(); ;
                            break;
                        case "MANAGER":
                            changes.OLD_VALUE = origin.ManagerId == null ? null : origin.ManagerId.ToString();
                            changes.NEW_VALUE = poa.MANAGER_ID == null ? null : poa.MANAGER_ID.ToString(); ;
                            break;
                        case "PHONE":
                            changes.OLD_VALUE = origin.PoaPhone;
                            changes.NEW_VALUE = poa.POA_PHONE;
                            break;
                        case "EMAIL":
                            changes.OLD_VALUE = origin.Email;
                            changes.NEW_VALUE = poa.EMAIL;
                            break;
                        case "ADDRESS":
                            changes.OLD_VALUE = origin.PoaAddress;
                            changes.NEW_VALUE = poa.POA_ADDRESS;
                            break;
                        case "ID_CARD":
                            changes.OLD_VALUE = origin.PoaIdCard;
                            changes.NEW_VALUE = poa.POA_ID_CARD;
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
                var origin = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
                 AutoMapper.Mapper.Map(model.Detail, poa);
                 SetChanges(origin,poa);
               
                _poaBll.Update(poa);
                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                return RedirectToAction("Index");
            }

            catch
            {
                return View();
            }

        }
        
        public ActionResult Detail(int id)
        {
            var poa = _poaBll.GetById(id);
            if (poa == null)
            {
                return HttpNotFound();
            }
            var changeHistoryList = _changesHistoryBll.GetByFormTypeId(Enums.MenuList.POA);
           
            var model = new POAFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            var detail = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
            model.Users = GlobalFunctions.GetCreatorList();
            model.Detail = detail;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
            return View(model);

        }

        public ActionResult Delete(int id)
        {
            try
            {
                _poaBll.Delete(id);
                TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
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
            var id = Convert.ToInt32(userId);
            return Json(_userBll.GetUserById(id));
        }

       
    }
}