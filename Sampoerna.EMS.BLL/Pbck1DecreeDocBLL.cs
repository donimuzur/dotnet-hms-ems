using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class Pbck1DecreeDocBLL : IPbck1DecreeDocBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<PBCK1_DECREE_DOC> _repository;

        public Pbck1DecreeDocBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1_DECREE_DOC>();
        }

        public void DeleteByPbck1Id(long pbck1Id)
        {
            var dataToDelete = _repository.Get(c => c.PBCK1_ID == pbck1Id);
            if (dataToDelete != null)
            {
                foreach (var item in dataToDelete.ToList())
                {
                    _repository.Delete(item);
                }
            }
        }

        public int RemoveDoc(long Id)
        {
            try
            {
                _repository.Delete(Id);
                _uow.SaveChanges();
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;

        }
    }
}
