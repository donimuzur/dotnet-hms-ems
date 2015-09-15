using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{

    public class PBCK7AndPBCK3BLL : IPBCK7And3BLL
    {
        private ILogger _looger;
        private IGenericRepository<PBCK3_PBCK7> _repository;
        private IUnitOfWork _uow;
        private IBACK1BLL _back1Bll;

        private string includeTable = "BACK1";

        public PBCK7AndPBCK3BLL(IUnitOfWork uow, ILogger logger)
        {
            _looger = logger;
            _uow = uow;
            _back1Bll = new BACK1BLL(_uow, _looger);
            _repository = _uow.GetGenericRepository<PBCK3_PBCK7>();

        }

        public List<Pbck7AndPbck3Dto> GetAllByParam(Pbck7AndPbck3Input input)
        {
            Expression<Func<PBCK3_PBCK7, bool>> queryFilter = PredicateHelper.True<PBCK3_PBCK7>();
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBCK_ID == input.NppbkcId);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty(input.Poa))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Pbck7Date)))
            {
                var dt = Convert.ToDateTime(input.Pbck7Date);
                queryFilter = queryFilter.And(c => c.PBCK7_DATE == dt);
            }
            if (!string.IsNullOrEmpty((input.Pbck3Date)))
            {
                var dt = Convert.ToDateTime(input.Pbck3Date);
                queryFilter = queryFilter.And(c => c.PBCK3_DATE == dt);
            }

            Func<IQueryable<PBCK3_PBCK7>, IOrderedQueryable<PBCK3_PBCK7>> orderBy = null;
            if (!string.IsNullOrEmpty(input.ShortOrderColum))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK3_PBCK7>(input.ShortOrderColum));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTable);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            var mapResult = Mapper.Map<List<Pbck7AndPbck3Dto>>(dbData.ToList());

            return mapResult;
        }

        public void Insert(Pbck7AndPbck3Dto pbck7AndPbck3Dto)
        {
            var dataToAdd = Mapper.Map<PBCK3_PBCK7>(pbck7AndPbck3Dto);
            _repository.InsertOrUpdate(dataToAdd);
        }
    }


}
