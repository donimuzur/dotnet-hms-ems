using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class UserPlantMapBLL : IUserPlantMapBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER_PLANT_MAP> _repository;

        private IUserPlantMapService _userPlantService;

        public UserPlantMapBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _userPlantService = new UserPlantMapService(_uow, _logger);
            _repository = _uow.GetGenericRepository<USER_PLANT_MAP>();
        }
        
        public void Save(USER_PLANT_MAP userPlantMap)
        {
            _userPlantService.Save(userPlantMap);
            _uow.SaveChanges();
        }

        public List<USER_PLANT_MAP> GetAll()
        {
            return _userPlantService.GetAll();
        }

        public List<UserPlantMapDto> GetAllOrderByUserId()
        {
            var dbData = _userPlantService.GetAll().OrderBy(a=>a.USER_ID).ToList();

            return Mapper.Map<List<UserPlantMapDto>>(dbData);
        }

        public UserPlantMapDto GetById(int id)
        {
            var dbData = _userPlantService.GetById(id);
            //return _repository.Get(p => p.USER_PLANT_MAP_ID == id, null, _includeTables).FirstOrDefault();
            return Mapper.Map<UserPlantMapDto>(dbData);
        }

        public List<USER_PLANT_MAP> GetByUserId(string id)
        {
            //return _repository.Get(p => p.USER_ID == id, null, _includeTables).ToList();
            return _userPlantService.GetByUserId(id);
        }

        public USER_PLANT_MAP GetByUserIdAndPlant(string userid, string plantid)
        {
            //return _repository.Get(p => p.USER_ID == userid && p.PLANT_ID == plantid, null, _includeTables).FirstOrDefault();
            return _userPlantService.GetByUserIdAndPlant(userid, plantid);
        }

        public void Delete(int id)
        {
            _userPlantService.Delete(id);
            _uow.SaveChanges();
        }

        public List<ZAIDM_EX_NPPBKCCompositeDto> GetAuthorizedNppbkc(UserPlantMapGetAuthorizedNppbkc input)
        {
            return Mapper.Map<List<ZAIDM_EX_NPPBKCCompositeDto>>(_userPlantService.GetAuthorizedNppbkc(input));
        }

        public List<T001WCompositeDto> GetAuthorizdePlant(UserPlantMapGetAuthorizedPlant input)
        {
            return Mapper.Map<List<T001WCompositeDto>>(_userPlantService.GetAuthorizdePlant(input));
        }

        public List<string> GetPlantByUserId(string id)
        {
            var list = new List<string>();

            var data = _userPlantService.GetByUserId(id);

            foreach (var item in data)
            {
                list.Add(item.PLANT_ID);
            }

            return list;
        }

        public void Active(UserPlantMapDto isActive)
        {
            var activeUser = GetById(isActive.Id);
            if (activeUser.IsActive == "True")
            {
                activeUser.IsActive = "False";
            }
            else
            {
                activeUser.IsActive = "True";
            }
            
            var dbData = Mapper.Map<USER_PLANT_MAP>(activeUser);
            _repository.Update(dbData);
            _uow.SaveChanges();
        }

        public List<string> GetNppbkcByUserId(string id)
        {
            var list = new List<string>();

            var data = _userPlantService.GetByUserId(id);

            foreach (var item in data)
            {
                list.Add(item.T001W.NPPBKC_ID);
            }

            return list;
        }
    }
}
