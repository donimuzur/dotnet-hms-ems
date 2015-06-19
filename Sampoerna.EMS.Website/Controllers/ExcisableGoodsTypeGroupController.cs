using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.GOODSTYPE;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ExcisableGoodsTypeGroupController : BaseController
    {

        private IZaidmExGoodTypeBLL _goodsTypeGroupBLL;
        private IMasterDataBLL _masterDataBll;

        public ExcisableGoodsTypeGroupController(IZaidmExGoodTypeBLL goodsTypeGroupBLL, IMasterDataBLL masterData, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _goodsTypeGroupBLL = goodsTypeGroupBLL;
            _masterDataBll = masterData;   
        }
        //
        // GET: /ExcisableGoodsTypeGroup/
        public ActionResult Index()
        {

            var goodsTypeGroup = new GoodsTypeGroupViewModel();
            goodsTypeGroup.MainMenu = Enums.MenuList.MasterData;
            goodsTypeGroup.CurrentMenu = PageInfo;

            goodsTypeGroup.Details = _goodsTypeGroupBLL.GetAll().Select(AutoMapper.Mapper.Map<DetailsGoodsTypGroup>).ToList();
            
            return View("Index", goodsTypeGroup);
        }
	}
}