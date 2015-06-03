using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class UserBLL : IUserBLL
    {

        private IGenericRepository<USER> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

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

        public USER GetById(int id)
        {
            return _repository.GetByID(id);
        }

    }
}
