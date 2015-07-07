using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExNPPBKCBLL : IZaidmExNPPBKCBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _repository;
        private string includeTables = "ZAIDM_EX_KPPBC,T1001,C1LFA1";
        private IChangesHistoryBLL _changesHistoryBll;

        public ZaidmExNPPBKCBLL(IUnitOfWork uow, ILogger logger, IChangesHistoryBLL changesHistoryBll)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
            _changesHistoryBll = changesHistoryBll;
        }

        public ZAIDM_EX_NPPBKC GetById(long id)
        {
            //return _repository.Get(id);
            return _repository.Get(c => c.NPPBKC_ID == id, null, includeTables).FirstOrDefault();
        }

        public ZAIDM_EX_NPPBKC GetDetailsById(long id)
        {
            return _repository.Get(c => c.NPPBKC_ID == id, null, "T1001, ZAIDM_EX_KPPBC").FirstOrDefault();
        }

        public List<ZAIDM_EX_NPPBKC> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }
        
        public void Save(ZAIDM_EX_NPPBKC nppbkc)
        {
            if (nppbkc.NPPBKC_ID != 0)
            {
                //update
                _repository.Update(nppbkc);
            }
            else
            {
                //Insert
                _repository.Insert(nppbkc);
            }

            try
            {
                _uow.SaveChanges();

            }
            catch (Exception exception)
            {
                _logger.Error(exception);

            }
            
        }


        public void Delete(int id)
        {
            var existingNppbkc = GetById(id);
            existingNppbkc.IS_DELETED = true;
            _repository.Update(existingNppbkc);
            _uow.SaveChanges();
        }


        public void Update(ZAIDM_EX_NPPBKC nppbkc)
        {
            try
            {
                _repository.Update(nppbkc);
                _uow.SaveChanges();
            }
            catch (Exception)
            {
                _uow.RevertChanges();
                throw;
            }
        }

        public string GetCityByNppbkcId(long nppBkcId)
        {
            var dbData = _repository.GetByID(nppBkcId);
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData.CITY;

        }

        public string GetCeOfficeCodeByNppbcId(long nppBkcId)
        {

            var dbData = _repository.Get(n => n.NPPBKC_ID == nppBkcId, null, "ZAIDM_EX_KPPBC").FirstOrDefault();
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData.ZAIDM_EX_KPPBC.KPPBC_NUMBER;

        }
    }
}
