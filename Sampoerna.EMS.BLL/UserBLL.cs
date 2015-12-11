﻿using System;
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
using Enums = Sampoerna.EMS.Core.Enums;

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

        
    }
}