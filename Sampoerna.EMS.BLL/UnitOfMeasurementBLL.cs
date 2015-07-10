﻿using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class UnitOfMeasurementBLL : IUnitOfMeasurementBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<UOM> _repository;
        
        public UnitOfMeasurementBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<UOM>();
        }
        public UOM GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<UOM> GetAll()
        {
            return _repository.Get().ToList();
        }

        public string GetUomNameById(int? id)
        {
            var dbData = _repository.GetByID(id);

            if (dbData == null)
                return string.Empty;

            return dbData.UOM_NAME;
        }

        public bool IsUomNameExist(string name)
        {
            var dbData = _repository.Get(u => u.UOM_NAME.Equals(name)).FirstOrDefault();
            return dbData != null;
        }

        public UOM GetUomByName(string name)
        {
            return _repository.Get(c => c.UOM_NAME == name, null, null).FirstOrDefault();
        }
    }
}
