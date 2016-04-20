﻿using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack1IncomeDetailService : ILack1IncomeDetailService
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK1_INCOME_DETAIL> _repository;

        public Lack1IncomeDetailService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<LACK1_INCOME_DETAIL>();
        }
        public void DeleteByLack1Id(int? lack1Id)
        {
            
            if (!lack1Id.HasValue)
            {
                var dataToDelete = _repository.Get(c => !c.LACK1_ID.HasValue).Select(x=> x.LACK1_INCOME_ID).ToList();
                if (dataToDelete.Any())
                {
                    foreach (var item in dataToDelete)
                    {
                        _repository.Delete(item);
                    }
                }
            }
            else
            {
                var sql = "delete from LACK1_INCOME_DETAIL where LACK1_ID = " + lack1Id;
                _repository.ExecuteSql(sql);
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

        public void DeleteDataList(IEnumerable<LACK1_INCOME_DETAIL> listToDelete)
        {
            if (listToDelete != null)
            {
                foreach (var item in listToDelete.ToList())
                {
                    _repository.Delete(item);
                }
            }

        }

        public List<LACK1_INCOME_DETAIL> GetLack1IncomeDetailByCk5Id(long ck5Id)
        {
            return _repository.Get(c => c.CK5_ID == ck5Id, null, "LACK1,LACK1.MONTH").ToList();
        }

    }
}
