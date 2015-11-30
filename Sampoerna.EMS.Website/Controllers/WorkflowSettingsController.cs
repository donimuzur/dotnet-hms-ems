using Newtonsoft.Json;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class WorkflowSettingsController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IUserBLL _userBLL;
        private IEmailTemplateBLL _emailTemplateBLL;
        private IWorkflowSettingBLL _workflowSettingBLL;
        public WorkflowSettingsController(IPageBLL pageBLL, IUserBLL userBLL,IEmailTemplateBLL emailTemplateBLL, IWorkflowSettingBLL workflowSettingBLL) : base(pageBLL, Enums.MenuList.Settings) {
            _mainMenu = Enums.MenuList.Settings;
            _pageBLL = pageBLL;
            _userBLL = userBLL;
            _emailTemplateBLL = emailTemplateBLL;
            _workflowSettingBLL = workflowSettingBLL;
        }
        //
        // GET: /WorkflowSettings/
        public ActionResult Index()
        {
            var model = new WorkflowSettingListModel();
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;
            model.Details = AutoMapper.Mapper.Map<List<WorkflowDetails>>(_pageBLL.GetModulePages());
            return View(model);
        }

        //
        // GET: /WorkflowSettings/Details/5
        public ActionResult Details(int id)
        {
            var model = AutoMapper.Mapper.Map<WorkflowDetails>(_pageBLL.GetPageByID(id));
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;
            model.Details = AutoMapper.Mapper.Map<List<WorkflowMappingDetails>>(_workflowSettingBLL.GetAllByFormId(id));
            return View(model);
            
        }

        

        //
        // GET: /WorkflowSettings/Edit/5
        public ActionResult Edit(int id)
        {
            var model = AutoMapper.Mapper.Map<WorkflowDetails>(_pageBLL.GetPageByID(id));
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;
            model.Details = AutoMapper.Mapper.Map<List<WorkflowMappingDetails>>(_workflowSettingBLL.GetAllByFormId(id));
            return View(model);
        }

        //
        // POST: /WorkflowSettings/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult SaveForkflowMapping(int? actionid,int? formid, List<WorkflowUsers> worflowusers, WorkflowMappingDetails mappingdata) {
            try
            {
                var datausers = worflowusers;
                //var datausers = JsonConvert.DeserializeObject<List<WorkflowUsers>>(worflowusers);
                var data = AutoMapper.Mapper.Map<WORKFLOW_STATE>(mappingdata);
                if (actionid.HasValue) {
                    //data.ACTION_ID = actionid.Value;
                }
                List<string> useridlist = new List<string>();
                foreach (var user in datausers)
                {
                    useridlist.Add(user.User_Id);
                }
                //data.WORKFLOW_STATE_USERS = _userBLL.GetUsersByListId(useridlist);
                _workflowSettingBLL.SaveWorkflowState(data);

                JsonResult json = new JsonResult();
                json.ContentType = "Application/json";
                json.Data = data;
                return json;
            }
            catch {
                //JsonResult json = new JsonResult();
                //json.Data = "error";
                return null;
            }
            
        }

        //
        // GET: /WorkflowSettings/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WorkflowSettings/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WorkflowSettings/GetMapping/5
        public ActionResult GetMapping(int formid,int? stateid) {
            WorkflowMappingDetails model;
            if (stateid.HasValue && stateid.Value > 0)
            {
                var wfstate = _workflowSettingBLL.GetAllById(stateid.Value);
                var page = _pageBLL.GetPageByID(formid);
                model = new WorkflowMappingDetails();
                model.Form_Id = page.PAGE_ID;
                model.Modul = page.MENU_NAME;
                //model.StateMappingId = wfstate.ACTION_ID.ToString();
                //model.State = wfstate.ACTION_NAME;
                model.ListUser = AutoMapper.Mapper.Map<List<WorkflowUsers>>(wfstate.WORKFLOW_STATE_USERS.ToList());
                //model.
                //model.
            }
            else {
                model = new WorkflowMappingDetails();
                var page = _pageBLL.GetPageByID(formid);
                model.Form_Id = page.PAGE_ID;
                model.Modul = page.MENU_NAME;
            }
            
            model.UserSelectList = new SelectList(_userBLL.GetUsers(), "EMAIL", "USER_ID");
            model.EmailTemplateList = new SelectList(_emailTemplateBLL.getAllEmailTemplates(), "EMAIL_TEMPLATE_ID", "TEMPLATE_NAME");
            return PartialView("GetMappingPartial",model);
        }
    }
}
