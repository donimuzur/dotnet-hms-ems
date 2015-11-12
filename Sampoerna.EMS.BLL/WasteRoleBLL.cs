using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class WasteRoleBLL : IWasteRoleBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<WASTE_ROLE> _repository;
        private string _includeTables = "T001W, USER";
        public WasteRoleBLL(IUnitOfWork uow, ILogger logger)
        {
           _uow = uow;
           _logger = logger;
           _repository = _uow.GetGenericRepository<WASTE_ROLE>();
        }

        public List<WasteRoleDto> GetAllData(bool isIncludeTables = true)
        {
            string include = "";
            if (isIncludeTables)
                include = _includeTables;
            var listData = _repository.Get(null, null, include).ToList();

            return Mapper.Map<List<WasteRoleDto>>(listData);
        }
    }
}
