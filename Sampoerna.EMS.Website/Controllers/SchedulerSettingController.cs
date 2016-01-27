using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Website.Models.SchedulerSetting;
using System.Configuration;

namespace Sampoerna.EMS.Website.Controllers
{
    public class SchedulerSettingController : BaseController
    {
        private ISchedulerSettingBLL _schedulerSettingBll;
        //
        // GET: /SchedulerSetting/
        public SchedulerSettingController(IPageBLL pageBll,ISchedulerSettingBLL schedulerBll) : base(pageBll, Enums.MenuList.SchedulerSettings)
        {
            _schedulerSettingBll = schedulerBll;

            var fileName = ConfigurationManager.AppSettings.Get("SchedulerPath");
            _schedulerSettingBll.SetXmlFile(fileName);
        }

        public ActionResult Index()
        {

            var data = _schedulerSettingBll.GetMinutesCron();

            var model = Mapper.Map<SchedulerSettingModel>(data);
            model.MainMenu = Enums.MenuList.Settings;
            model.CurrentMenu = PageInfo;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(SchedulerSettingModel model)
        {
            var data = Mapper.Map<SchedulerSetting>(model);

            try
            {
                _schedulerSettingBll.Save(data);
                AddMessageInfo("Success updating scheduler setting.", Enums.MessageInfoType.Success);
            }
            catch (BLLException ex)
            {
                AddMessageInfo(string.Format("Error updating Scheduler Setting : {0}." , ex.Message), Enums.MessageInfoType.Error);
            }
            
            return RedirectToAction("Index");
        }
    }
}