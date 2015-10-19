using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Test
{
    [TestClass]
    public class ProductionBLLTest
    {
        private IGenericRepository<PRODUCTION> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IProductionBLL _productionBll;

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = _uow.GetGenericRepository<PRODUCTION>();
            _productionBll = new ProductionBLL(_logger, _uow);

            BLLMapper.Initialize();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            _logger = null;
            _productionBll = null;
            _repository = null;
            _uow = null;
        }

        [TestMethod]
        public void IsCorrectRowResult()
        {
            //act
            var results = _productionBll.GetAllProduction();
            var companyCode1616 = results.Where(x => x.CompanyCode == "1616");

            //assert
            Assert.AreEqual(0, companyCode1616.Count());
        }

        [TestMethod]
        public void GetById_Test()
        {
            var productionGetId = new ProductionDto();

        }
    }
}
