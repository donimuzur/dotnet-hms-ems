using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
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
            var werks = results.Where(x => x.PlantWerks == "ID01");

            //assert
            Assert.AreEqual(0, companyCode1616.Count());
            Assert.AreEqual(0, werks.Count());
        }
        
        [TestMethod]
        public void getProduction_WhenDataFound_ChcekCount()
        {
            //arrange 
            var dailyProd = FakeStuffs.GetDailyProductionList();
            var input = new ProductionGetByParamInput();

            //act
            _repository.Get().ReturnsForAnyArgs(dailyProd);
            var result = _productionBll.GetAllByParam(input);

            //assert
            Assert.AreEqual(dailyProd.Count(), result.Count);

        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void getProduction_WhenDataNotFound_ThrowExceptions()
        {
            //arrange
            var input = new ProductionGetByParamInput();
            _repository.Get().ReturnsForAnyArgs(n => null);
            try
            {
                //act
                _productionBll.GetAllByParam(input);
            }
            catch (BLLException ex)
            {

                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;

            }

        }


    }
}
