using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using AutoMapper;

namespace Sampoerna.EMS.BLL
{
    public class LACK10ItemBLL : ILACK10ItemBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK10_ITEM> _repository;

        private string includeTables = "LACK10";

        public LACK10ItemBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK10_ITEM>();
        }


        public void DeleteByLack10Id(long lack10Id)
        {
            var dataToDelete = _repository.Get(c => c.LACK10_ID == lack10Id);
            if (dataToDelete != null)
            {
                foreach (var lack10Item in dataToDelete.ToList())
                {
                    _repository.Delete(lack10Item);
                }
            }
        }
    }
}
