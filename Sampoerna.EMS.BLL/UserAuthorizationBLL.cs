using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.BusinessObject.DTOs;
using AutoMapper;

namespace Sampoerna.EMS.BLL
{
    public class UserAuthorizationBLL : IUserAuthorizationBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER_BROLE> _repository;
        private IGenericRepository<BROLE_MAP> _repositoryRoleMap;
       
        public UserAuthorizationBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = uow.GetGenericRepository<USER_BROLE>();
            _repositoryRoleMap = uow.GetGenericRepository<BROLE_MAP>();
        }
        public List<UserAuthorizationDto> GetAll()
        {
            var dtData = _repository.Get().ToList();
            return Mapper.Map<List<UserAuthorizationDto>>(dtData);
        }

        public UserAuthorizationDto GetById(string id)
        {
            return Mapper.Map<UserAuthorizationDto>(_repository.Get(c => c.BROLE == id, null, "BROLE_MAP, BROLE_MAP.USER, PAGE_MAP, PAGE_MAP.PAGE").FirstOrDefault());
        }

        public List<BRoleDto> GetAllBRole()
        {
            var dtData = _repository.Get().ToList();
            return Mapper.Map<List<BRoleDto>>(dtData);
        }

        public List<int?> GetAuthPages(string userid)
        {
            var pages = new List<int?>();
            var broleMaps = _repositoryRoleMap.Get(x => x.MSACCT == userid);
            foreach (var broleMap in broleMaps)
            {
                var brole = GetById(broleMap.BROLE);
                foreach (var page in brole.PageMaps)
                {
                    pages.Add(page.Page.Id);
                }
            }
            return pages;
        }
    }
}
