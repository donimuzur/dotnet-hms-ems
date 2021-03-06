﻿using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack1TrackingService : ILack1TrackingService
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK1_TRACKING> _repository;
        private IGenericRepository<LACK1_CALCULATION_DETAIL> _repoCalculation;
        private IGenericRepository<LACK1_PERIOD_SUMMARY> _repoPeriodSummary;

        public Lack1TrackingService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<LACK1_TRACKING>();
            _repoCalculation = _uow.GetGenericRepository<LACK1_CALCULATION_DETAIL>();
            _repoPeriodSummary = _uow.GetGenericRepository<LACK1_PERIOD_SUMMARY>();
        }

        public void DeleteByLack1Id(int? lack1Id)
        {
            var sql = "";
            if (!lack1Id.HasValue)
            {
                sql = "delete from LACK1_TRACKING where LACK1_ID is NULL";

                _repository.ExecuteSql(sql);
            }
            else
            {
                sql = "delete from LACK1_TRACKING where LACK1_ID = " + lack1Id;

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


        public void DeleteCalculationDetails(int? lack1Id)
        {
            var sql = "";
            if (!lack1Id.HasValue)
            {
                sql = "delete from LACK1_CALCULATION_DETAIL where LACK1_ID is NULL";

                _repoCalculation.ExecuteSql(sql);
            }
            else
            {
                sql = "delete from LACK1_CALCULATION_DETAIL where LACK1_ID = " + lack1Id;

                _repoCalculation.ExecuteQuery(sql);
            }
        }

        public void DeletePeriodSummary(int? lack1Id)
        {
            var sql = "";
            if (!lack1Id.HasValue)
            {
                sql = "delete from LACK1_PERIOD_SUMMARY where LACK1_ID is NULL";

                _repoPeriodSummary.ExecuteSql(sql);
            }
            else
            {
                sql = "delete from LACK1_PERIOD_SUMMARY where LACK1_ID = " + lack1Id;

                _repoPeriodSummary.ExecuteQuery(sql);
            }
        }

        

        public void DeleteDataList(IEnumerable<LACK1_TRACKING> listToDelete)
        {
            if (listToDelete != null)
            {
                foreach (var item in listToDelete)
                {
                    _repository.Delete(item);
                }
            }
        }

        public List<long> GetMovement201FromTracking()
        {
            var usage201 = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Usage201);
            var data = _repository.Get(x => x.INVENTORY_MOVEMENT.MVT == usage201, null,
                "INVENTORY_MOVEMENT").Select(x => x.INVENTORY_MOVEMENT_ID != null ? x.INVENTORY_MOVEMENT_ID.Value : 0).ToList();
            return data;
        }   
    }
}
