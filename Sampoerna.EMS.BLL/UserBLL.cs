using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.LinqExtensions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class UserBLL : IUserBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER> _repository;
        private POABLL _poabll;
        private IUserPlantMapBLL _userPlantMapBll;

        public UserBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<USER>();

           _poabll = new POABLL(_uow, _logger);
            _userPlantMapBll = new UserPlantMapBLL(_uow, _logger);
        }

        public List<USER> GetUsers(UserInput input)
        {

            Expression<Func<USER, bool>> queryFilter = PredicateHelper.True<USER>();

            if (!string.IsNullOrEmpty(input.UserName))
            {
                queryFilter = queryFilter.And(s => s.USER_ID.Contains(input.UserName));
            }

            if (!string.IsNullOrEmpty(input.FirstName))
            {
                queryFilter = queryFilter.And(s => s.FIRST_NAME.Contains(input.FirstName));
            }

            if (!string.IsNullOrEmpty(input.LastName))
            {
                queryFilter = queryFilter.And(s => s.LAST_NAME.Contains(input.LastName));
            }

          

            
          
            Func<IQueryable<USER>, IOrderedQueryable<USER>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<USER>(input.SortOrderColumn)) as IOrderedQueryable<USER>;
            }

            var rc = _repository.Get(queryFilter, orderBy);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return rc.ToList();

        }

        public List<USER> GetUsers()
        {
            return _repository.Get().ToList();
        }

        public List<UserTree> GetUserTree()
        {
            Expression<Func<USER, bool>> queryFilter = PredicateHelper.True<USER>();
            var users = _repository.Get(queryFilter, null, null);
            if (users == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<List<UserTree>>(users);
        }

      

        public Login GetLogin(string userName)
        {
            return Mapper.Map<Login>(_repository.Get(x=> x.USER_ID == userName && (!x.IS_ACTIVE.HasValue || x.IS_ACTIVE != 0)).FirstOrDefault());
        }


        public USER GetUserById(string id)
        {
            return _repository.GetByID(id);
        }


        public List<USER> GetUsersByListId(List<string> useridlist)
        {
            var data = _repository.Get(obj => useridlist.Contains(obj.USER_ID),null,"").ToList();

            return data;
        }


        //public List<UserDto> GetListUserRoleByUserId(string userId)
        //{
        //    var userRole = _poabll.GetUserRole(userId);

        //    var listUser = _repository.Get();

        //    var filterResult = new List<USER>();

        //    foreach (var user in listUser)
        //    {
        //        var role = _poabll.GetUserRole(user.USER_ID);

        //        if (userRole == role)
        //            filterResult.Add(user);
        //    }

        //    return Mapper.Map<List<UserDto>>(filterResult);
        //}


        public void SaveUser(USER user)
        {
            
                var data = _repository.GetByID(user.USER_ID);

                data.IS_MASTER_DATA_APPROVER = user.IS_MASTER_DATA_APPROVER;
                _repository.InsertOrUpdate(data);

                _uow.SaveChanges();
            
            
        }

        public bool IsUserMasterApprover(string userId)
        {
            return _repository.Get(x => x.IS_MASTER_DATA_APPROVER.HasValue && x.IS_MASTER_DATA_APPROVER.Value && x.USER_ID == userId && x.IS_ACTIVE.HasValue && x.IS_ACTIVE == 1, null, "").Any();
        }

        public List<UserDto> GetListUserRoleByUserId(string userId)
        {
            var userRole = _poabll.GetUserRole(userId);

            List<string> listPlantUserFrom;
            List<string> listPlantUserTo;

            if (userRole == Enums.UserRole.POA)
                listPlantUserFrom = _poabll.GetPoaPlantByPoaId(userId);
            else if (userRole == Enums.UserRole.User)
                listPlantUserFrom = _userPlantMapBll.GetByUserId(userId).Select(c => c.PLANT_ID).ToList();
            else
                listPlantUserFrom = new List<string>();

            var listUser = _repository.Get();

            var filterResult = new List<USER>();

            foreach (var user in listUser)
            {
                var role = _poabll.GetUserRole(user.USER_ID);

                if (userRole == role)
                {
                    if (role == Enums.UserRole.POA)
                    {
                        //get list plant from poa_map
                      
                        listPlantUserTo = _poabll.GetPoaPlantByPoaId(user.USER_ID);
                        //foreach (var plantUserTo in listPlantUserTo)
                        //{
                        //    foreach (var plantUserFrom in listPlantUserFrom)
                        //    {
                        //        if (plantUserFrom == plantUserTo)
                        //            filterResult.Add(user);
                        //    }
                        //}
                        
                    }
                    else if (role == Enums.UserRole.User)
                    {
                        //get list plant from user_plant map
                      
                        listPlantUserTo = _userPlantMapBll.GetByUserId(user.USER_ID).Select(c => c.PLANT_ID).ToList();
                     }
                    else 
                        listPlantUserTo = new List<string>();

                    foreach (var plantUserTo in listPlantUserTo)
                    {
                        foreach (var plantUserFrom in listPlantUserFrom)
                        {
                            if (plantUserFrom == plantUserTo)
                                filterResult.Add(user);
                        }
                    }

                }
            }

            filterResult = filterResult.Where(c=>c.USER_ID != userId).DistinctBy(c => c.USER_ID).ToList();

            return Mapper.Map<List<UserDto>>(filterResult);
        }

        public List<USER> GetControllers()
        {
            var controllersList = new List<USER>();

            var data = _repository.Get(x => (!x.IS_ACTIVE.HasValue || x.IS_ACTIVE != 0) && x.BROLE_MAP.Any(y => y.ROLEID == Enums.UserRole.Controller && y.MSACCT.ToLower() == x.USER_ID.ToLower()), null, "BROLE_MAP").ToList();

            
            controllersList.AddRange(data);
            return controllersList;
        }
    }
}
