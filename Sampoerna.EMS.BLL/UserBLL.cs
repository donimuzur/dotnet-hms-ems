using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class UserBLL : IUserBLL
    {
        
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER> _repository;

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
            
            return _repository.Get(queryFilter, orderBy, string.Empty).ToList();

        }

        public List<UserTree> GetUserTree()
        {

            var users = _repository.Get().ToList();
            var usersTree = new List<UserTree>();

            foreach (var user in users)
            {
                var tree = Mapper.Map<UserTree>(user);

                if (tree.MANAGER_ID.HasValue)
                {
                    tree.Manager = _repository.Get(p => p.USER_ID == tree.MANAGER_ID).FirstOrDefault();
                }

                tree.Employees = _repository.Get(p => p.MANAGER_ID != null).Where(x => x.MANAGER_ID.Equals(tree.USER_ID)).ToList();
                usersTree.Add(tree);

            }
            return usersTree;
        }

        public UserTree GetUserTreeByUserID(int userID)
        {
            var user = _repository.GetByID(userID);
            
            if (user == null)
                return null;

            var tree = Mapper.Map<UserTree>(user);

            if (tree.MANAGER_ID.HasValue)
            {
                tree.Manager = _repository.Get(p => p.USER_ID == user.MANAGER_ID).FirstOrDefault();
            }

            tree.Employees = _repository.Get(p => p.MANAGER_ID != null).Where(x => x.MANAGER_ID.Equals(tree.USER_ID)).ToList();
            return tree;
        }

    }
}
