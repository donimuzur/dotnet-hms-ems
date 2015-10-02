using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Back1Services : IBack1Services
    {
        private IGenericRepository<BACK1> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string includeTables = "";

        public Back1Services(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<BACK1>();
        }

        public void SaveBack1ByCk5Id(SaveBack1ByCk5IdInput input)
        {
            var dbBack1 = _repository.Get(c => c.CK5_ID == input.Ck5Id).FirstOrDefault() ?? new BACK1();

            dbBack1.CK5_ID = input.Ck5Id;
            dbBack1.BACK1_NUMBER = input.Back1Number;
            dbBack1.BACK1_DATE = input.Back1Date;

            _repository.InsertOrUpdate(dbBack1);

        }

        public BACK1 GetBack1ByCk5Id(long ck5Id)
        {
            var dbBack1 = _repository.Get(c => c.CK5_ID == ck5Id).FirstOrDefault();

            return dbBack1;

        }
    }
}
