using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class UserBLL : IUserBLL
    {
        
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER> _repository;
        private string includeTables = "USER1, USER2, USER_GROUP";

        public UserBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<USER>();
        }

        public List<USER> GetUsers(UserInput input)
        {

            Expression<Func<USER, bool>> queryFilter = PredicateHelper.True<USER>();

            if (!string.IsNullOrEmpty(input.UserName))
            {
                queryFilter = queryFilter.And(s => s.USERNAME.Contains(input.UserName));
            }

            if (!string.IsNullOrEmpty(input.FirstName))
            {
                queryFilter = queryFilter.And(s => s.FIRST_NAME.Contains(input.FirstName));
            }

            if (!string.IsNullOrEmpty(input.LastName))
            {
                queryFilter = queryFilter.And(s => s.LAST_NAME.Contains(input.LastName));
            }

            if (input.IsActive.HasValue)
            {
                queryFilter = queryFilter.And(s => s.IS_ACTIVE == input.IsActive);
            }

            if (input.ManagerId.HasValue)
            {
                queryFilter = queryFilter.And(s => s.MANAGER_ID.HasValue && s.MANAGER_ID.Value == input.ManagerId.Value);
            }
            
            Func<IQueryable<USER>, IOrderedQueryable<USER>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<USER>(input.SortOrderColumn)) as IOrderedQueryable<USER>;
            }

            var rc = _repository.Get(queryFilter, orderBy, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return rc.ToList();

        }

        public List<UserTree> GetUserTree()
        {
            Expression<Func<USER, bool>> queryFilter = PredicateHelper.True<USER>();
            var users = _repository.Get(queryFilter, null, includeTables);
            if(users == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<List<UserTree>>(users);
        }

        public UserTree GetUserTreeByUserID(int userID)
        {
            var user = _repository.Get(c => c.USER_ID == userID, null, includeTables).FirstOrDefault();
            if(user == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            return Mapper.Map<UserTree>(user);
        }

        public Login GetLogin(string userName)
        {
            return Mapper.Map<Login>(_repository.Get(c => c.USERNAME == userName, null, includeTables).FirstOrDefault());
        }



        public USER GetUserById(int id)
        {
            return _repository.Get(p => p.USER_ID == id).FirstOrDefault();
        }
    }
}
