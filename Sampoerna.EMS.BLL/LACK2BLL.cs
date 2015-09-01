using System.Security.Cryptography;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class LACK2BLL : ILACK2BLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK2> _repository;
        private IGenericRepository<LACK2_ITEM> _repositoryItem;
      
        private IMonthBLL _monthBll;
        private IUserBLL _userBll;
        private IUnitOfMeasurementBLL _uomBll;

        private string includeTables = "MONTH";

        public LACK2BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK2>();
            _repositoryItem = _uow.GetGenericRepository<LACK2_ITEM>();
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
        }

        /// <summary>
        /// Gets all of the data for LACK2 Table by entered parameters
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<Lack2Dto> GetAll(Lack2GetByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            if (!string.IsNullOrEmpty((input.SubmissionDate)))
            {
                var dt = Convert.ToDateTime(input.SubmissionDate);
                DateTime dt2 = DateTime.ParseExact("07/01/2015", "MM/dd/yyyy", CultureInfo.InvariantCulture);
                queryFilter = queryFilter.And(c => dt2.Date.ToString().Contains(c.SUBMISSION_DATE.ToString()));
            }

            Func<IQueryable<LACK2>, IOrderedQueryable<LACK2>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK2>(input.SortOrderColumn));

            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<Lack2Dto>>(dbData.ToList());

            return mapResult;
        }

        /// <summary>
        /// Gets all LACK2 Documents with status Completed
        /// </summary>
        /// <returns></returns>
        public List<Lack2Dto> GetAllCompleted()
        {
            return Mapper.Map<List<Lack2Dto>>(_repository.Get(x => x.STATUS == (int)Enums.DocumentStatus.Completed, null, includeTables));
        }

        /// <summary>
        /// Gets all LACK2 COMPLETED Documents by parameters
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<Lack2Dto> GetAllCompletedByParam(Lack2GetByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            if(input.Status != null || input.Status != 0)
            {
                queryFilter = queryFilter.And(c => c.STATUS == (int)input.Status);
            }
            if (!string.IsNullOrEmpty((input.SubmissionDate)))
            {
                var dt = Convert.ToDateTime(input.SubmissionDate);
                DateTime dt2 = DateTime.ParseExact("07/01/2015", "MM/dd/yyyy", CultureInfo.InvariantCulture);
                queryFilter = queryFilter.And(c => dt2.Date.ToString().Contains(c.SUBMISSION_DATE.ToString()));
            }

            Func<IQueryable<LACK2>, IOrderedQueryable<LACK2>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK2>(input.SortOrderColumn));

            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<Lack2Dto>>(dbData.ToList());

            return mapResult;
        }

        /// <summary>
        /// Gets Lack2 by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Lack2Dto</returns>
        public Lack2Dto GetById(int id)
        {
            return Mapper.Map<Lack2Dto>(_repository.GetByID(id));
        }

        public Lack2Dto GetByIdAndItem(int id)
        {
            var data = _repositoryItem.Get(x => x.LACK2_ID == id, null, "LACK2, CK5");
            var lack2dto = new Lack2Dto();
            lack2dto = data.Select(x => Mapper.Map<Lack2Dto>(x.LACK2)).FirstOrDefault();;
            lack2dto.Items = data.Select(x => Mapper.Map<Lack2ItemDto>(x)).ToList();;
            return lack2dto;
        }

        /// <summary>
        /// Inserts a LACK2 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Lack2Dto Insert(Lack2Dto item)
        {

            if (item == null)
            {
                throw new Exception("Invalid data entry !");
            }

            LACK2 model = new LACK2();
            MONTH month = new MONTH();

            month = _monthBll.GetMonth(item.PeriodMonth);
            var user = _userBll.GetUserById(item.CreatedBy);

            model = AutoMapper.Mapper.Map<LACK2>(item);
            model.MONTH = month;
            model.USER = user;
            try
            {
                _repository.InsertOrUpdate(model);
                _uow.SaveChanges();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return item;
        }
    }
}
