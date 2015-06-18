using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExPOABLL : IZaidmExPOABLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_POA> _repository;
        private string includeTables = "ZAIDM_POA_MAP";
        private IGenericRepository<USER> _repositoryUser; 

        public ZaidmExPOABLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_POA>();
            _repositoryUser = _uow.GetGenericRepository<USER>();
        }

        public ZAIDM_EX_POA GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<ZaidmExPOAOutput> GetAll()
        {
            var repoZaidmExPOA = _repository.Get().ToList();
            var repoUser = _repositoryUser.Get().ToList();

            var result = repoZaidmExPOA.Join(repoUser,
                poa => poa.USER_ID,
                USER => USER.USER_ID,
                (poa, USER) => new ZaidmExPOAOutput 
                {
                    PoaIdCard = poa.POA_ID_CARD,
                    UserName = USER.USERNAME,
                    PoaPrintedName= poa.POA_PRINTED_NAME,
                    PoaAddress = poa.POA_ADDRESS,
                    PoaPhone = poa.POA_PHONE,
                    Title = poa.TITLE
                   
                    
                }).ToList();

            return result;

            //return _repository.Get(null, null, includeTables).ToList();
        }

    }
}
