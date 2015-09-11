using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class T001KBLL : IT001KBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<T001K> _repository;
        private string includeTables = "T001";

        public T001KBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T001K>();
        }

        public T001KDto GetByBwkey(string bwkey)
        {
            var dbData = _repository.Get(c => c.BWKEY == bwkey, null, includeTables).FirstOrDefault();
            return Mapper.Map<T001KDto>(dbData);
        }

        public T001KDto GetByNppbkcIdAndMainPlant(string nppbkcId)
        {
            includeTables += ", T001W, T001W.ZAIDM_EX_NPPBKC";
            var dbData = _repository.Get(c => c.T001W.NPPBKC_ID == nppbkcId && c.T001W.IS_MAIN_PLANT.HasValue && c.T001W.IS_MAIN_PLANT.Value, null, includeTables).FirstOrDefault();
            return Mapper.Map<T001KDto>(dbData);
        }

        public List<T001WDto> GetPlantByCompany(string companyId, bool isReverse = false)
        {
            includeTables = "T001W";
            IEnumerable<T001K> dbData;
            if (!isReverse)
            {
                dbData = _repository.Get(c => c.BUKRS == companyId, null, includeTables);
            }
            else {
                dbData = _repository.Get(c => c.BUKRS != companyId, null, includeTables);
            }
            
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return Mapper.Map<List<T001WDto>>(dbData.Select(x=>x.T001W));
        }

        public List<T001Dto> GetCompanyByPlant(string plantId)
        {
            includeTables = "T001";
            var dbData = _repository.Get(c => c.BWKEY == plantId, null, includeTables);
            if (dbData == null)
            {
                throw  new BLLException((ExceptionCodes.BLLExceptions.DataNotFound));
            }
            return Mapper.Map<List<T001Dto>>(dbData.Select(x=>x.T001));

        }
    }
}
