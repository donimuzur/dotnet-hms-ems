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
    public class UserBLLTest
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER> _repository;
        private IUserBLL _bll;

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _uow = Substitute.For<IUnitOfWork>();
            _repository = _uow.GetGenericRepository<USER>();
            _bll = new UserBLL(_uow, _logger);
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
        
        [TestMethod, ExpectedException(typeof(BLLException))]   
        public void GetUsers_WhenDataNotFound_ThrowExceptions()
        {
            //arrange
            var input = new UserInput();
            _repository.Get().ReturnsForAnyArgs(n => null);
            try
            {
                //act
                _bll.GetUsers(input);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void GetUsers_WhenDataFound_CheckCount()
        {
            //arrange
            var users = FakeStuffs.GetGenericUserStubs();
            var input = new UserInput();

            //act
            _repository.Get().ReturnsForAnyArgs(users);
            var results = _bll.GetUsers(input);

            //assert
            Assert.AreEqual(users.Count(), results.Count);
        }

        [TestMethod, ExpectedException(typeof (BLLException))]
        public void GetUserTree_WhenDataNotFound_ThrowException()
        {
            //arrange
            _repository.Get().ReturnsForAnyArgs(n => null);
            try
            {
                //act
                _bll.GetUserTree();
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void GetUserTree_WhenDataFound_CheckCount()
        {
            //arrange
            var users = FakeStuffs.GetGenericUserStubs();
            _repository.Get().ReturnsForAnyArgs(users);

            //assert
            var results = _bll.GetUserTree();

            //act
            Assert.AreEqual(users.Count(), results.Count);
        }

        [TestMethod, ExpectedException(typeof (BLLException))]
        public void GetUserTreeByUserID_WhenDataNotFound_ThrowExpcetion()
        {
            //arrange
            var input = 0;
            _repository.Get(c => c.USER_ID == input).Returns(n => null);

            try
            {
                //act
                _bll.GetUserTreeByUserID(input);
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
