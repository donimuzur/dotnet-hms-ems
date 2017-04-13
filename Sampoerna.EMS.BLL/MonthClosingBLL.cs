using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.LinqExtensions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class MonthClosingBLL : IMonthClosingBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MONTH_CLOSING> _repository;
        private IPlantBLL _plantBll;

        private string includeTables = "T001W";

        public MonthClosingBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<MONTH_CLOSING>();
            _plantBll = new PlantBLL(_uow, _logger);
        }

        public List<MonthClosingDto> GetList(MonthClosingGetByParam param)
        {
            Expression<Func<MONTH_CLOSING, bool>> queryFilter = PredicateHelper.True<MONTH_CLOSING>();

            if (param.Month > 0)
            {
                queryFilter = queryFilter.And(c => c.CLOSING_DATE.Value.Month == param.Month);
            }

            if (param.Year > 0)
            {
                queryFilter = queryFilter.And(c => c.CLOSING_DATE.Value.Year == param.Year);
            }

            var monthClosingList = _repository.Get(queryFilter, null, includeTables).ToList();

            return Mapper.Map<List<MonthClosingDto>>(monthClosingList);
        }


        public bool Save(MonthClosingDto item)
        {
            var result = false;

            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                MONTH_CLOSING model;

                if (item.MonthClosingId > 0)
                {
                    //update
                    model = _repository.Get(c => c.MONTH_CLOSING_ID == item.MonthClosingId).FirstOrDefault();

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    Mapper.Map<MonthClosingDto, MONTH_CLOSING>(item, model);
                }
                else
                {
                    var activePlant = _plantBll.GetActivePlant();
                    var monthFlag = item.ClosingDate.ToString("MMyyyy");

                    foreach (var plant in activePlant)
                    {
                        var newData = new MonthClosingDto();
                        newData = item;
                        newData.PlantId = plant.WERKS;

                        var newModel = new MONTH_CLOSING();
                        newModel = Mapper.Map<MONTH_CLOSING>(newData);
                        newModel.MONTH_FLAG = monthFlag;
                        _repository.InsertOrUpdate(newModel);
                    }
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


        public MonthClosingDto GetDataByParam(MonthClosingGetByParam param)
        {
            Expression<Func<MONTH_CLOSING, bool>> queryFilter = PredicateHelper.True<MONTH_CLOSING>();

            if (!string.IsNullOrEmpty(param.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == param.PlantId);
            }
            if (param.ClosingDate != null)
            {
                queryFilter = queryFilter.And(c => c.CLOSING_DATE <= param.ClosingDate.Value
                                    && c.CLOSING_DATE.Value.Month == param.ClosingDate.Value.Month
                                    && c.CLOSING_DATE.Value.Year == param.ClosingDate.Value.Year);
            }
            if (param.DisplayDate != null)
            {
                queryFilter = queryFilter.And(c => c.CLOSING_DATE.Value.Month == param.DisplayDate.Value.Month
                                    && c.CLOSING_DATE.Value.Year == param.DisplayDate.Value.Year);
            }

            var dbData = _repository.Get(queryFilter).FirstOrDefault();

            var mapResult = Mapper.Map<MonthClosingDto>(dbData);

            return mapResult;
        }


        public MonthClosingDto GetById(long id)
        {
            var dbData = _repository.Get(c => c.MONTH_CLOSING_ID == id).FirstOrDefault();

            var mapResult = Mapper.Map<MonthClosingDto>(dbData);

            return mapResult;
        }
    }
}
