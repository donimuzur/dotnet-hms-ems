using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Website.Controllers;
using Sampoerna.EMS.Website.Test.Controller;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.Website.Test
{
    [TestClass]
    public class CompanyBLLTest
    {
        private IGenericRepository<T1001> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private ICompanyBLL _companyBll;

        [TestInitialize]
        public void SetUp()
        {
            EMSWebsiteMapper.Initialize();

            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = Substitute.For<IGenericRepository<T1001>>();
            var companyData = FakeStuffWeb.GetCompany();
            _uow.GetGenericRepository<T1001>().ReturnsForAnyArgs(_repository);
            _repository.GetQuery().ReturnsForAnyArgs(companyData.AsQueryable());

            _companyBll = new CompanyBLL(_uow, _logger);

        }

        [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _uow = null;
            _repository = null;
            _companyBll = null;
        }

        [TestMethod]
        public void IsReturnCorrectView()
        {
            var controller = new CompanyController(_companyBll);
            var result = controller.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void IsReturnCorrectViewAction()
        {
            CompanyController controller = new CompanyController(_companyBll);
            ActionResult result = controller.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            //var controller = new CompanyController(_companyBll);
            //var result = (RedirectToRouteResult)controller.Index();
            //Assert.AreEqual("Index", SubstituteExtensions.Returns(result.RouteValues));

            //CompanyController controller = new CompanyController(_companyBll);
            //ActionResult result = controller.Index();
            //Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            //RedirectToRouteResult routeResult = result as RedirectToRouteResult;
            //Assert.AreEqual(routeResult.RouteValues, "Index");
        }
    }
}
