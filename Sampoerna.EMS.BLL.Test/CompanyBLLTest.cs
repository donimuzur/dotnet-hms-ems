using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

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
            _repository = _uow.GetGenericRepository<T1001>();
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
            var companyData = FakeStuffs.GetCompany();
            _repository.Get().ReturnsForAnyArgs(companyData);
            _repository.GetQuery().ReturnsForAnyArgs(companyData as IQueryable<T1001>);
           
           //act
            var results = _companyBll.GetMasterData();

           //assert
           Assert.AreEqual(companyData.Count(), results.Count);


        }

    }
}
