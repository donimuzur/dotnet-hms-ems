using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Settings;
using System.Configuration;
using System.Web.Configuration;
using Sampoerna.EMS.Contract;

namespace Sampoerna.EMS.Website.Controllers
{
    public class GlobalSettingsController : BaseController
    {

        public GlobalSettingsController(IPageBLL pageBLL) : base(pageBLL,Enums.MenuList.Settings){

        }
        //
        // GET: /GlobalSettings/
        public ActionResult Index()
        {
            var model = new GlobalSettingModel();

            model.UseBackDate = (bool.TrueString.ToLower() == ConfigurationManager.AppSettings.Get("UseBackdate").ToLower());
            HttpRuntimeSection section = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
            model.MainMenu = Enums.MenuList.Settings;
            model.CurrentMenu = PageInfo;

            model.FileSizeType = FileSizeType.KB;
            model.SizeType = fillSizeTypeList();
            model.UploadFileSize = section.MaxRequestLength;
            
            //model.SizeType
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(GlobalSettingModel model)
        {
            Configuration config =  WebConfigurationManager.OpenWebConfiguration("~");
            config.AppSettings.Settings["UseBackdate"].Value = model.UseBackDate.ToString();
            //model.UseBackDate = (bool.TrueString.ToLower() == config.AppSettings.Settings["UseBackdate"].Value.ToLower());
            HttpRuntimeSection section = (HttpRuntimeSection)config.GetSection("system.web/httpRuntime");

            if (model.FileSizeType == FileSizeType.MB)
            {
                section.MaxRequestLength = (int)(model.UploadFileSize * 1024f);
            }
            else {
                section.MaxRequestLength = model.UploadFileSize;
            }
            
            
            config.Save(ConfigurationSaveMode.Modified);
            //model.SizeType = fillSizeType();
            //model.UploadFileSize = section.MaxRequestLength;
            return RedirectToAction("Index");
        }

        private SelectList fillSizeTypeList()
        {
            var values = from FileSizeType e in FileSizeType.GetValues(typeof(FileSizeType))
                         select new { Id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name");
            //model.SizeType = new SelectList(new List<FileSizeType>());
        }
    }
}
