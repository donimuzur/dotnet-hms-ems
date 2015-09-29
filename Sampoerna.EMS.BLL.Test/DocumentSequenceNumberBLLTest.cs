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
        private IGenericRepository<T001K> _t001KReporRepository;
        private IDocumentSequenceNumberBLL _bll;

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = _uow.GetGenericRepository<DOC_NUMBER_SEQ>();
            _t001KReporRepository = _uow.GetGenericRepository<T001K>();
            _bll = new DocumentSequenceNumberBLL(_uow, _logger);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _uow = null;
            _repository = null;
            _t001KReporRepository = null;
            _bll = null;
        }

        [TestMethod]
        public void GenerateNumber_WhenValidInput_GenerateDocumentNumber()
        {
            //arr
            var docNumberSeq = new DOC_NUMBER_SEQ()
            {
                MONTH = 1,
                YEAR = 2015,
                DOC_NUMBER_SEQ_LAST = 1
            };

            var t001k = new T001K()
            {
                T001 = new T001() { BUTXT = "Philip Morris Indonesia", BUTXT_ALIAS = "HMS-E", BUKRS = "3006" },
                T001W = new T001W()
                {
                    ZAIDM_EX_NPPBKC = new ZAIDM_EX_NPPBKC() { CITY_ALIAS = "SBY" }
                }
            };

            var input = new GenerateDocNumberInput()
            {
                Month = 1,
                Year = 1,
                NppbkcId = "1"
            };

            _repository.Get().ReturnsForAnyArgs(new List<DOC_NUMBER_SEQ>() { docNumberSeq });
            _t001KReporRepository.Get().ReturnsForAnyArgs(new List<T001K> { t001k });

            string expectedResult = (docNumberSeq.DOC_NUMBER_SEQ_LAST + 1).ToString("0000000000") + "/" + t001k.T001.BUTXT_ALIAS + "/" + t001k.T001W.ZAIDM_EX_NPPBKC.CITY_ALIAS + "/" + MonthHelper.ConvertToRomansNumeral(input.Month) + "/" + input.Year.ToString();

            //act
            var result = _bll.GenerateNumber(input);

            //assert
            Assert.AreEqual(expectedResult, result);
        }

    }
}
