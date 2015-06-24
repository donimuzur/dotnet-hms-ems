using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Test
{
    [TestClass]
    public class HeaderFooterBLLTest
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<HEADER_FOOTER> _repository;
        private IHeaderFooterBLL _bll;

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = _uow.GetGenericRepository<HEADER_FOOTER>();
            _bll = new HeaderFooterBLL(_uow, _logger);
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
        public void GetDetailsById_WhenDataExists_ReturnData()
        {
            //arr
            var headerFooterData = new HEADER_FOOTER()
            {
                HEADER_FOOTER_ID = 1,
                COMPANY_ID = 1,
                T1001 = new T1001() { COMPANY_ID = 1, BUKRS = "1001", BUKRSTXT = "HMS-E" },
                HEADER_FOOTER_FORM_MAP = new List<HEADER_FOOTER_FORM_MAP>
            {
                new HEADER_FOOTER_FORM_MAP()
                {
                    HEADER_FOOTER_ID = 1,
                    FORM_TYPE_ID = 1,
                    IS_FOOTER_SET = true,
                    IS_HEADER_SET = false
                },
                new HEADER_FOOTER_FORM_MAP()
                {
                    HEADER_FOOTER_ID = 1,
                    FORM_TYPE_ID = 2,
                    IS_FOOTER_SET = true,
                    IS_HEADER_SET = true
                }
            }
            };

            _repository.Get().ReturnsForAnyArgs(new List<HEADER_FOOTER>() { headerFooterData });

            //act

            var result = _bll.GetDetailsById(1);

            //assert
            Assert.AreEqual(headerFooterData.HEADER_FOOTER_ID, result.HEADER_FOOTER_ID);
            Assert.AreEqual(headerFooterData.HEADER_FOOTER_FORM_MAP.Count, result.HeaderFooterMapList.Count);
        }

        [TestMethod]
        public void GetAll_WhenDataExists_CheckCount()
        {
            //arr
            var headerFooterData = new HEADER_FOOTER()
            {
                HEADER_FOOTER_ID = 1,
                COMPANY_ID = 1,
                T1001 = new T1001() { COMPANY_ID = 1, BUKRS = "1001", BUKRSTXT = "HMS-E" },
                HEADER_FOOTER_FORM_MAP = new List<HEADER_FOOTER_FORM_MAP>
            {
                new HEADER_FOOTER_FORM_MAP()
                {
                    HEADER_FOOTER_ID = 1,
                    FORM_TYPE_ID = 1,
                    IS_FOOTER_SET = true,
                    IS_HEADER_SET = false
                },
                new HEADER_FOOTER_FORM_MAP()
                {
                    HEADER_FOOTER_ID = 1,
                    FORM_TYPE_ID = 2,
                    IS_FOOTER_SET = true,
                    IS_HEADER_SET = true
                }
            }
            };

            _repository.Get().ReturnsForAnyArgs(new List<HEADER_FOOTER>() { headerFooterData });

            //act

            var result = _bll.GetAll();

            //assert
            Assert.AreEqual(1, result.Count);
        }

    }
}
