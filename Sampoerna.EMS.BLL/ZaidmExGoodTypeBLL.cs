using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExGoodTypeBLL : IZaidmExGoodTypeBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repository;
        private IGenericRepository<EX_GROUP_TYPE> _repositoryGroup;

        public ZaidmExGoodTypeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
            _repositoryGroup = _uow.GetGenericRepository<EX_GROUP_TYPE>();
        }

        public ZAIDM_EX_GOODTYP GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_GOODTYP> GetAll()
        {
            return _repository.Get().ToList();
        }

        public List<ZAIDM_EX_GOODTYP> GetAllChildName()
        {
            return _repository.Get().ToList();
        }

        public void SaveGroup(List<EX_GROUP_TYPE> listGroupTypes)
        {
            foreach (var listGroupType in listGroupTypes)
            {
                _repositoryGroup.InsertOrUpdate(listGroupType);
            }
            _uow.SaveChanges();
        }

        public EX_GROUP_TYPE GetGroupTypeByName(string name)
        {
            return _repositoryGroup.Get(g => g.GROUP_NAME == name).FirstOrDefault();
        }

        public List<EX_GROUP_TYPE> GetGroupTypesByName(string name)
        {
            return _repositoryGroup.Get(g => g.GROUP_NAME == name, null, "ZAIDM_EX_GOODTYP").ToList();
        }

        public List<EX_GROUP_TYPE> GetAllGroup()
        {
            return _repositoryGroup.Get().ToList();
        }
    }
}
