using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ReversalController : BaseController
    {
        private IReversalBLL _reversalBll;
        private Enums.MenuList _mainMenu;

        public ReversalController(IPageBLL pageBll, IReversalBLL reversalBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _reversalBll = reversalBll;
            _mainMenu = Enums.MenuList.CK4C;
        }

        #region Index
        #endregion
    }
}