using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class UserPlantMapBLL : IUserPlantMapBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER_PLANT_MAP> _repository;
        private string _includeTables = "USER, T001W";
        public UserPlantMapBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<USER_PLANT_MAP>();
           
        }


        public void Save(USER_PLANT_MAP userPlantMap)
        {
            _repository.InsertOrUpdate(userPlantMap);
            _uow.SaveChanges();
        }

        public List<USER_PLANT_MAP> GetAll()
        {
            return _repository.Get(null, null, _includeTables).ToList();
        }

        public USER_PLANT_MAP GetById(int id)
        {
            return _repository.Get(p => p.USER_PLANT_MAP_ID == id, null, _includeTables).FirstOrDefault();
        }
    }
}
