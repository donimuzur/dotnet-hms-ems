using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class Pbck1ProdConverterBLL : IPbck1ProdConverterBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<PBCK1_PROD_CONVERTER> _repository;

        public Pbck1ProdConverterBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1_PROD_CONVERTER>();
        }

        public void DeleteByPbck1Id(long pbck1Id)
        {
            var dataToDelete = _repository.Get(c => c.PBCK1_ID.HasValue && c.PBCK1_ID.Value == pbck1Id);
            if (dataToDelete != null)
            {
                foreach (var pbck1ProdConverter in dataToDelete.ToList())
                {
                    _repository.Delete(pbck1ProdConverter);
                }
            }
        }
    }
}
