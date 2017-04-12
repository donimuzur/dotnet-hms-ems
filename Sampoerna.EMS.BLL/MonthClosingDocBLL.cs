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
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.LinqExtensions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class MonthClosingDocBLL : IMonthClosingDocBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MONTH_CLOSING_DOCUMENT> _repository;

        public MonthClosingDocBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<MONTH_CLOSING_DOCUMENT>();
        }

        public bool Save(List<MonthClosingDocDto> item)
        {
            var result = false;

            try
            {
                var model = Mapper.Map<List<MONTH_CLOSING_DOCUMENT>>(item);

                foreach(var data in model)
                {
                    _repository.InsertOrUpdate(data);
                }

                _uow.SaveChanges();

                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }
    }
}
