using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Test
{
    [TestClass]
    public class DocumentSequenceNumberBLLTest
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<DOC_NUMBER_SEQ> _repository;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _nppbkcRepository;
        private IDocumentSequenceNumberBLL _bll;

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = _uow.GetGenericRepository<DOC_NUMBER_SEQ>();
            _nppbkcRepository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
            _bll = new DocumentSequenceNumberBLL(_uow, _logger);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _uow = null;
            _repository = null;
            _nppbkcRepository = null;
            _bll = null;
        }

        [TestMethod]
        public void GenerateNumber_WhenValidInput_GenerateDocumentNumber()
        {
            //arr
            var docNumberSeq = new DOC_NUMBER_SEQ()
            {
                MONTH = 1, YEAR = 2015, DOC_NUMBER_SEQ_LAST = 1
            };

            var nppbkc = new ZAIDM_EX_NPPBKC()
            {
                NPPBKC_ID = "1", CITY_ALIAS = "MLG",
                T001 = new T001() { BUTXT = "HMS-E", BUKRS = "3006" }
            };

            var input = new GenerateDocNumberInput()
            {
                Month = 1, Year = 1, NppbkcId = 1
            };

            _repository.Get().ReturnsForAnyArgs(new List<DOC_NUMBER_SEQ>() { docNumberSeq });
            _nppbkcRepository.GetByID(input.NppbkcId).Returns(nppbkc);

            string expectedResult = (docNumberSeq.DOC_NUMBER_SEQ_LAST + 1).ToString("00000") + "/" + nppbkc.T001.BUTXT + "/" + nppbkc.CITY_ALIAS + "/" + MonthHelper.ConvertToRomansNumeral(input.Month) + "/" + input.Year.ToString();

            //act
            var result = _bll.GenerateNumber(input);

            //assert
            Assert.AreEqual(expectedResult, result);
        }

    }
}
