using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class POAService : IPoaService
    {
        private IGenericRepository<POA> _repository;
        private IGenericRepository<BROLE_MAP> _broleMapRepository;
        private IGenericRepository<USER> _userRepository; 

        private ILogger _logger;
        
        private string includeTables = "POA_MAP, USER, USER1, POA_SK";

        public POAService(IUnitOfWork uow, ILogger logger)
        {
            
            _logger = logger;
            _repository = uow.GetGenericRepository<POA>();
            _broleMapRepository = uow.GetGenericRepository<BROLE_MAP>();
            _userRepository = uow.GetGenericRepository<USER>();

        }
        public POA GetById(string id)
        {
            return _repository.Get(p => p.POA_ID == id, null, includeTables).FirstOrDefault();
        }

        public void Save(POA poa)
        {
            _repository.InsertOrUpdate(poa);


        }

        public List<POA> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public Core.Enums.UserRole GetUserRole(string userId)
        {
            var role = _broleMapRepository.Get(x => x.MSACCT.ToUpper() == userId).Select(x => x.ROLEID).FirstOrDefault();

            if (role.HasValue)
            {
                return role.Value;
            }
            else
            {
                var poa = GetAll();
                var manager = _broleMapRepository.Get(x => x.USER_BROLE.BROLE_DESC.Contains("POA_MANAGER")).Select(x => x.MSACCT.ToUpper()).ToList();
                if (manager.Contains(userId.ToUpper()))
                    return Core.Enums.UserRole.Controller;

                if (poa.Any(zaidmExPoa => zaidmExPoa.LOGIN_AS == userId))
                    return Core.Enums.UserRole.POA;


                return Core.Enums.UserRole.User;
            }
        }


        public USER GetUserById(string userId)
        {
            return _userRepository.GetByID(userId);
        }

        public List<USER> GetMasterApprovers()
        {
            var controllersList = new List<USER>();

            var data = _userRepository.Get(x => (!x.IS_ACTIVE.HasValue || x.IS_ACTIVE != 0) && x.IS_MASTER_DATA_APPROVER.HasValue && x.IS_MASTER_DATA_APPROVER.Value, null, "").ToList();


            controllersList.AddRange(data);
            return controllersList;
        } 
    }
}
