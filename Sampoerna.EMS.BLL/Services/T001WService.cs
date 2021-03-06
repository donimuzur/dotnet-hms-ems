﻿using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class T001WService : IT001WService
    {
        private IGenericRepository<T001W> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public T001WService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<T001W>();
        }

        public T001W GetById(string werks)
        {
            return _repository.Get(x=> x.WERKS == werks,null,"T001K,T001K.T001").FirstOrDefault();
        }

        public List<T001W> GetByNppbkcId(string nppbkcId)
        {
            return _repository.Get(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && c.NPPBKC_ID == nppbkcId).ToList();
        }

        public T001W GetMainPlantByNppbkcId(string nppbkcId)
        {
            return
                _repository.Get(
                    c =>
                        !string.IsNullOrEmpty(c.NPPBKC_ID) && c.NPPBKC_ID == nppbkcId && c.IS_MAIN_PLANT.HasValue &&
                        c.IS_MAIN_PLANT.Value,null,"T001K,T001K.T001").FirstOrDefault();
        }

        public List<T001W> GetAll()
        {
            return _repository.Get().ToList();
        }


        public List<T001W> GetByRange(string begining, string end)
        {
            return
                _repository.Get(c => string.Compare(c.WERKS, begining) >= 0 && string.Compare(c.WERKS, end) <= 0)
                    .ToList();
        }
    }
}
