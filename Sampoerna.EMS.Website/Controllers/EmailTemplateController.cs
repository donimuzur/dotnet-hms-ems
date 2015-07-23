using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.EmailTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class EmailTemplateController : BaseController
    {

        private IEmailTemplateBLL _emailTemplateBll;
        private Enums.MenuList _mainMenu;
        private IChangesHistoryBLL _changeBLL;


        public EmailTemplateController(IPageBLL pageBLL, IEmailTemplateBLL emailBLL) : base(pageBLL, Enums.MenuList.Settings) {
            _emailTemplateBll = emailBLL;
            _mainMenu = Enums.MenuList.Settings;
        }

        //
        // GET: /EmailTemplate/
        public ActionResult Index()
        {
            var model = new EmailTemplateIndexModel();
            model.Details = AutoMapper.Mapper.Map <List<EmailTemplateModel>>(_emailTemplateBll.getAllEmailTemplates());
            model.MainMenu = _mainMenu;
            return View("Index",model);
        }

        //
        // GET: /EmailTemplate/Details/5
        public ActionResult Details(int id)
        {
            var data = AutoMapper.Mapper.Map<EmailTemplateModel>(_emailTemplateBll.getEmailTemplateById(id));
            data.CurrentMenu = PageInfo;
            data.MainMenu = _mainMenu;
            return View("Details",data);
        }

        //
        // GET: /EmailTemplate/Create
        public ActionResult Create()
        {
            var model = new EmailTemplateModel();
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;
            return View("Create",model);
        }

        //
        // POST: /EmailTemplate/Create
        [HttpPost]
        public ActionResult Create(EmailTemplateModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var data = AutoMapper.Mapper.Map<EMAIL_TEMPLATE>(model);

                data.CREATED_DATE = DateTime.Now;
                    _emailTemplateBll.Save(data);
                    TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                    return RedirectToAction("Index");
                
                
                
            }
            catch(Exception ex)
            {
                return View(model);
            }
        }

        //
        // GET: /EmailTemplate/Edit/5
        public ActionResult Edit(int id)
        {
            var model = AutoMapper.Mapper.Map<EmailTemplateModel>(_emailTemplateBll.getEmailTemplateById(id));
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;
            return View("Edit", model);
        }

        //
        // POST: /EmailTemplate/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, EmailTemplateModel model)
        {
            try
            {
                // TODO: Add update logic here
                var data = _emailTemplateBll.getEmailTemplateById(id);
                data.BODY = model.EmailTemplateBody;
                data.TEMPLATE_NAME = model.EmailTemplateName;
                data.SUBJECT = model.EmailTemplateSubject;
                _emailTemplateBll.Save(data);
                TempData[Constans.SubmitType.Update] = Constans.SubmitMessage.Updated;
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View(model);
            }
        }

        

        //
        // POST: /EmailTemplate/Delete/5
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
    }
}
