using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class DocumentSequenceController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IDocumentSequenceNumberBLL _docBLL;
        public DocumentSequenceController(IPageBLL pageBll, IDocumentSequenceNumberBLL docBLL) : base(pageBll, Enums.MenuList.Settings) { 
            _mainMenu = Enums.MenuList.Settings;
            _docBLL = docBLL;
            
        }
        //
        // GET: /DocumenSequence/
        public ActionResult Index()
        {
            var model = new DocumentSequenceListModel();
            model.Details = AutoMapper.Mapper.Map<List<DocumentSequenceModel>>(_docBLL.GetDocumentSequenceList());
            model.MainMenu = Enums.MenuList.Settings;
            model.CurrentMenu = PageInfo;
            return View(model);
        }

        
    }
}
