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
    public class MonthClosingBLL : IMonthClosingBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MONTH_CLOSING> _repository;
        private IPlantBLL _plantBll;

        public MonthClosingBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<MONTH_CLOSING>();
            _plantBll = new PlantBLL(_uow, _logger);
        }

        public List<MonthClosingDto> GetList()
        {
            var monthClosingList = _repository.Get().ToList();

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
    }
}
