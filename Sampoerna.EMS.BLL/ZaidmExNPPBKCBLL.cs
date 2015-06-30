using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExNPPBKCBLL : IZaidmExNPPBKCBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _repository;
        private string includeTables = "ZAIDM_EX_KPPBC,T1001,C1LFA1";

        public ZaidmExNPPBKCBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
        }

        public ZAIDM_EX_NPPBKC GetById(long id)
        {
            //return _repository.Get(id);
            return _repository.Get(c => c.NPPBKC_ID == id, null, includeTables).FirstOrDefault();
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
    }
}
