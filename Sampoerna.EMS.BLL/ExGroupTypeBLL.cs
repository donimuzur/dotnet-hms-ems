using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ExGroupTypeBLL : IExGroupTypeBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<EX_GROUP_TYPE> _repository;

        private IGenericRepository<EX_GROUP_TYPE_DETAILS> _repositoryDetail;
        private string includeTables = "EX_GROUP_TYPE_DETAILS, EX_GROUP_TYPE_DETAILS.ZAIDM_EX_GOODTYP";
        public ExGroupTypeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = uow.GetGenericRepository<EX_GROUP_TYPE>();
            _repositoryDetail = _uow.GetGenericRepository<EX_GROUP_TYPE_DETAILS>();
        }


        public void SaveGroup(List<EX_GROUP_TYPE> listGroupTypes)
        {
            foreach (var listGroupType in listGroupTypes)
            {
                _repository.InsertOrUpdate(listGroupType);
            }
            _uow.SaveChanges();
        }

        public void UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName)
        {
            //delete first

            var dbGroup = _repository.Get(a => a.GROUP_NAME == groupName, null, includeTables).ToList();
            
           

            //insert the data
            foreach (var listGroupType in listGroupTypes)
            {
                _repository.InsertOrUpdate(listGroupType);
            }
            _uow.SaveChanges();
        }

        public EX_GROUP_TYPE GetGroupTypeByName(string name)
        {
            return _repository.Get(g => g.GROUP_NAME == name).FirstOrDefault();
        }

        public EX_GROUP_TYPE GetById(int id)
        {
            return _repository.Get(p=>p.EX_GROUP_TYPE_ID == id, null, includeTables).FirstOrDefault();
        }

        public List<EX_GROUP_TYPE> GetGroupTypesByName(string name)
        {
            return _repository.Get(g => g.GROUP_NAME == name, null, includeTables).ToList();
        }

        public List<EX_GROUP_TYPE> GetAll()
        {
            
            return _repository.Get(null, null, includeTables).OrderBy(x=>x.GROUP_NAME).ToList();
        }

        public List<string> GetGoodTypeByGroup(int groupid)
        {
            return _repositoryDetail.Get(p => p.EX_GROUP_TYPE_ID == groupid, null, "ZAIDM_EX_GOODTYP").Select(p => p.ZAIDM_EX_GOODTYP.EXT_TYP_DESC).ToList(); 
        }

        public void DeleteDetails(EX_GROUP_TYPE_DETAILS details)
        {
            _repositoryDetail.Delete(details);
        }

        public bool IsGroupNameExist(string name)
        {
            var dbGroup = _repository.Get(g => g.GROUP_NAME == name).FirstOrDefault();
            if (dbGroup == null)
                return false;

            return true;
        }

        public void InsertDetail(EX_GROUP_TYPE_DETAILS detail)
        {
            _repositoryDetail.Insert(detail);
            _uow.SaveChanges();
        }

      
    }
}