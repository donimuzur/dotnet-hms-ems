using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.Contract;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BLL.Test
{
    [TestClass]
    public class Lack2BLLTests
    {


        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK2> _repository;
        private ILACK2BLL _bll;

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = _uow.GetGenericRepository<LACK2>();
            _bll = new LACK2BLL(_uow, _logger);
            _uow.GetGenericRepository<LACK2>().ReturnsForAnyArgs(_repository);
            BLLMapper.Initialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _uow = null;
            _repository = null;
            _bll = null;
        }

        [TestMethod]
        public void GetAll_WhenCalled_RecivesGoodInput()
        {
            //Arrange 
            Lack2GetByParamInput input = new Lack2GetByParamInput
            {
                NppbKcId = "1",
                PlantId = "1",
                Poa = "TestPoa",
                SortOrderColumn = "TestSOC",
                SubmissionDate = DateTime.Now.ToString(),
                Creator = "TestUser"
            };

            Lack2GetByParamInput emptyInput = new Lack2GetByParamInput();

            Lack2Dto item = new Lack2Dto
            {
                Lack2Id = 1,
                LevelPlantCity = "TestCity",
                LevelPlantId = "1",
                ModifiedBy = "TestUser"
            };

            LACK2 lack = new LACK2
            {
                APPROVED_BY = "TestUser",
                APPROVED_DATE = DateTime.Now,
                BUKRS = "222",
                BUTXT = "Test",
                LACK2_ID = 1,
                LEVEL_PLANT_CITY = "TestCity"
            };

            _repository.Get().ReturnsForAnyArgs(new List<LACK2> { lack, lack });


            //Act
            var result = _bll.GetAll(input);
            var resul2 = _bll.GetAll(emptyInput);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resul2);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(resul2.Count == 2);
        }
    }
}
