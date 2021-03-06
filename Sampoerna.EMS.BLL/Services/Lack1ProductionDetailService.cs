﻿using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack1ProductionDetailService : ILack1ProductionDetailService
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK1_PRODUCTION_DETAIL> _repository;

        public Lack1ProductionDetailService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<LACK1_PRODUCTION_DETAIL>();
        }
        public void DeleteByLack1Id(int? lack1Id)
        {
            var sql = "";
            if (!lack1Id.HasValue)
            {
                sql = "delete from LACK1_PRODUCTION_DETAIL where LACK1_ID IS NULL";

                _repository.ExecuteSql(sql);
            }
            else
            {
                sql = "delete from LACK1_PRODUCTION_DETAIL where LACK1_ID = " + lack1Id;

                _repository.ExecuteQuery(sql);
            }
            
            //var dataToDelete = _repository.Get(c => c.LACK1_ID == lack1Id);
            //if (dataToDelete != null)
            //{
            //    foreach (var item in dataToDelete.ToList())
            //    {
            //        _repository.Delete(item);
            //    }
            //}
        }
        public void DeleteDataList(IEnumerable<LACK1_PRODUCTION_DETAIL> listToDelete)
        {
            if (listToDelete != null)
            {
                foreach (var item in listToDelete.ToList())
                {
                    _repository.Delete(item);
                }
            }
        }
    }
}
