using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.Contract;
namespace Sampoerna.EMS.BLL.Test
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
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = Substitute.For<IGenericRepository<T1001>>();
            var companyData = FakeStuffs.GetCompany();
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
        public void GetMasterData()
        {
            //arrange
           
           //act
            var results = _companyBll.GetMasterData();
            var companyCode102 = results.Where(x => x.BUKRS == "102");
           //assert
            Assert.AreEqual(1, companyCode102.Count());


        }

    }
}
