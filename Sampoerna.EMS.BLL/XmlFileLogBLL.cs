using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class XmlFileLogBLL : IXmlFileLogBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<XML_LOGS> _repository;


        public XmlFileLogBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<XML_LOGS>();
        }


        public List<XML_LOGSDto> GetXmlLogByParam(GetXmlLogByParamInput input)
        {
            Expression<Func<XML_LOGS, bool>> queryFilter = PredicateHelper.True<XML_LOGS>();

            if (input.DateFrom.HasValue)
            {
                input.DateFrom = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                queryFilter = queryFilter.And(c => c.LAST_ERROR_TIME >= input.DateFrom);
            }

            if (input.DateTo.HasValue)
            {
                input.DateFrom = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
                queryFilter = queryFilter.And(c => c.LAST_ERROR_TIME <= input.DateTo);
            }


            Func<IQueryable<XML_LOGS>, IOrderedQueryable<XML_LOGS>> orderByFilter = n => n.OrderByDescending(z => z.LAST_ERROR_TIME);
            
            var result = _repository.Get(queryFilter, orderByFilter, "").ToList();

            return Mapper.Map<List<XML_LOGSDto>>(result.ToList());


        }

    }
}
