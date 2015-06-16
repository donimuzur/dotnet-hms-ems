using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Website;
using Sampoerna.EMS.Website.Controllers;
using Sampoerna.EMS.Website.Models;
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
            EMSWebsiteMapper.Initialize();

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
        public void IsCorrectRowResult()
        {

            //act
            var results = _companyBll.GetMasterData();
            var companyCode102 = results.Where(x => x.BUKRS == "102");
            //assert
            Assert.AreEqual(1, companyCode102.Count());

        }

        [TestMethod]
        public void IsCorrectDate()
        {

            //act
            var results = _companyBll.GetMasterData();
            var companyCode102 = results.Where(x => x.BUKRS == "102");

            DateTime? value = new DateTime();
            foreach (var data in companyCode102)
            {
                value = data.CREATED_DATE;
            }
            //assert
            Assert.AreEqual(Convert.ToDateTime("2015-06-29 10:55:58.143"), value);

        }

      
    }
}
