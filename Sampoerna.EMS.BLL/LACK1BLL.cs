using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class LACK1BLL : ILACK1BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK1> _repository;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;

        private string includeTables = "MONTH, UOM";

        public LACK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK1>();
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
        }

        
        public List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            return GetLack1Data(queryFilter, input.SortOrderColumn);
        }
        

        public List<Lack1Dto> GetListByNpbkcParam(Lack1GetListByNppbkcParam input)
        {
            throw new NotImplementedException();
        }

        public List<Lack1Dto> GetListByPlantParam(Lack1GetListByPlantParam input)
        {
            throw new NotImplementedException();
        }

        private Expression<Func<LACK1, bool>> ProcessQueryFilter(Lack1GetByParamInput input)
        {
            Expression<Func<LACK1, bool>> queryFilter = PredicateHelper.True<LACK1>();
            if (!string.IsNullOrEmpty(input.NppbKcId))
            {
                queryFilter = queryFilter.And(c => )
            }
        }

    }
}
