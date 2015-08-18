using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Sampoerna.EMS.Website;
using Sampoerna.EMS.Website.Code;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Test
{
    [TestClass]
    public class GlobalFunctionTest
    {
        private IPOABLL _poabll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IMonthBLL _monthBll;
        private ISupplierPortBLL _supplierPortBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IUnitOfWork _uow;
        private IGenericRepository<POA> _repositoryPoa;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _repositoryNppbkc;
        private IGenericRepository<MONTH> _repositoryMonth;
        private IGenericRepository<SUPPLIER_PORT> _repositorySupplier;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repositoryGoodsType;
        private ILogger _logger;
       
        [TestInitialize]
        public void SetUp()
        {
            EMSWebsiteMapper.Initialize();
            BLLMapper.Initialize();
             _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
          

        }
         [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _uow = null;
            _repositoryPoa = null;
            _poabll = null;
             _nppbkcbll = null;
             _monthBll = null;
        }

        [TestMethod]
        public void GetPoaAllTest()
        {
            var poaFake = FakeStuffs.GetPOA();
            _repositoryPoa = Substitute.For<IGenericRepository<POA>>();
            _uow.GetGenericRepository<POA>().ReturnsForAnyArgs(_repositoryPoa);
             _poabll = new POABLL(_uow, _logger);
           
            _repositoryPoa.Get().ReturnsForAnyArgs(poaFake);
            var actualResult = GlobalFunctions.GetPoaAll(_poabll);
            var firstItemText = actualResult.ToList()[0].Text;
            var firstItemValue = actualResult.ToList()[0].Value;
            Assert.AreEqual(firstItemText, poaFake.ToList()[0].PRINTED_NAME);
            Assert.AreEqual(firstItemValue, poaFake.ToList()[0].POA_ID);

        }

        [TestMethod]
        public void GetNppbkcAllTest()
        {
            _repositoryNppbkc = Substitute.For<IGenericRepository<ZAIDM_EX_NPPBKC>>();
            _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>().ReturnsForAnyArgs(_repositoryNppbkc);
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, _logger);
            var nppbkcFake = FakeStuffs.GetNppbkc();
            _repositoryNppbkc.Get().ReturnsForAnyArgs(nppbkcFake);
            var actualResult = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
         
            var firstItemText = actualResult.ToList()[0].Text;
            var firstItemValue = actualResult.ToList()[0].Value;
            Assert.AreEqual(firstItemText, nppbkcFake.ToList()[0].NPPBKC_ID);
            Assert.AreEqual(firstItemValue, nppbkcFake.ToList()[0].NPPBKC_ID);
        }

        [TestMethod]
        public void GetMonthAllTest()
        {
            _repositoryMonth = Substitute.For<IGenericRepository<MONTH>>();
            _uow.GetGenericRepository<MONTH>().ReturnsForAnyArgs(_repositoryMonth);
            _monthBll = new MonthBLL(_uow, _logger);
            var monthFake = FakeStuffs.GetMonths();
            _repositoryMonth.Get().ReturnsForAnyArgs(monthFake);
            var actualResult = GlobalFunctions.GetMonthList(_monthBll);

            var firstItemText = actualResult.ToList()[0].Text;
            var firstItemValue = actualResult.ToList()[0].Value;
            Assert.AreEqual(firstItemText, monthFake.ToList()[0].MONTH_NAME_ENG);
            Assert.AreEqual(firstItemValue, monthFake.ToList()[0].MONTH_ID.ToString());
        }

        [TestMethod]
        public void GetSupplierPortAll()
        {
            _repositorySupplier = Substitute.For<IGenericRepository<SUPPLIER_PORT>>();
            _uow.GetGenericRepository<SUPPLIER_PORT>().ReturnsForAnyArgs(_repositorySupplier);
            _supplierPortBll = new SupplierPortBLL(_uow, _logger);
            var supplierPortFake = FakeStuffs.GetSupplierPorts();
            _repositorySupplier.Get().ReturnsForAnyArgs(supplierPortFake);
            var actualResult = GlobalFunctions.GetSupplierPortList(_supplierPortBll);

            var firstItemText = actualResult.ToList()[0].Text;
            var firstItemValue = actualResult.ToList()[0].Value;
            Assert.AreEqual(firstItemText, supplierPortFake.ToList()[0].PORT_NAME);
            Assert.AreEqual(firstItemValue, supplierPortFake.ToList()[0].SUPPLIER_PORT_ID.ToString());
        }

        [TestMethod]
        public void GetGoodsTypeTest()
        {
            _repositoryGoodsType = Substitute.For<IGenericRepository<ZAIDM_EX_GOODTYP>>();
            _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>().ReturnsForAnyArgs(_repositoryGoodsType);
            _goodTypeBll = new ZaidmExGoodTypeBLL(_uow, _logger);
            var goodTypeFake = FakeStuffs.GetGoodTypes();
            _repositoryGoodsType.Get().ReturnsForAnyArgs(goodTypeFake);
            var actualResult = GlobalFunctions.GetGoodTypeList(_goodTypeBll);

            var firstItemText = actualResult.ToList()[0].Text;
            var firstItemValue = actualResult.ToList()[0].Value;
            Assert.AreEqual(firstItemText, goodTypeFake.ToList()[0].EXC_GOOD_TYP + "-" + goodTypeFake.ToList()[0].EXT_TYP_DESC);
            Assert.AreEqual(firstItemValue, goodTypeFake.ToList()[0].EXC_GOOD_TYP);
        }
    }
}
